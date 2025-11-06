using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// NOTE: There is a high chance that multiple lines of code will be separated from this script and placed into an "AI Brain" script

public enum AlienState
{
    SCOUT,
    SUSPICIOUS,
    CHASE
}

[RequireComponent(typeof(NavMeshAgent))]
public class AlienStateMachine : MonoBehaviour
{
    /*
     * lineOfSightThreshold - The closer this value is to one, the narrower the FOV of the alien is
     * raycastLayersToIgnore - Layers the alien can see through when looking for a player
     * nodeLayer - A reference to the layer in which nodes are assigned
     */

    [Header("Setup")]
    public float lineOfSightThreshold;
    public LayerMask playerLayer;
    public LayerMask nodeLayer;

    /*
     * This variable is specifically here to allow you to tweak how much the alien prefers to visit highly probable nodes.
     * A temperature of one means that nodes are chosen specifically on their likelihood.
     * A temperature of less than one means that higher probabilities get more preferred than lower probabilities.
     * A temperature of more than one means the lower probabilities get preferred more than high probabilities.
     * 
     * This cannot be zero or any number below. It must strictly be a positive number.
     */
    [Min(0.001f)]
    public float temperature = 1f;

    /*********************************************************************************************************************/
    
    [Header("Path Padding")]

    [Min(0f)] 
    public float maxPathPadding = 3f;
    [Min(0f)] 
    public float minPathPadding = 1f;

    /*********************************************************************************************************************/

    private Transform player;
    private NodeManager nodeManager;
    private NavMeshAgent agent;

    // NOTE: lineOfSightAngle is a range that goes from -1 for completely behind the alien, 0 for perpendicular to the alien, 1 for perfectly in front of the alien, and everything in between
    [Header("DEBUG")]
    public AlienState currentState;

    // This stores the time when an alien's state begins
    // This gets set to 0 right before switching states
    [SerializeField] private float initTime;

    // Check to see if the alien can see the player
    [SerializeField] private bool canSeePlayer;

    // Dot product to see if the player is in line of sight
    [SerializeField] private float lineOfSight;

    // Current node that the alien is exploring
    [SerializeField] public Node currentNode;

    // List of nodes that the alien will ignore during the suspicious state
    [SerializeField] private List<GameObject> nodesToIgnore;

    // List of points that the alien will travel through
    [SerializeField] public Queue<Vector3> pointsToFollow;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        nodeManager = GameObject.FindWithTag("NodeManager").GetComponent<NodeManager>();
        agent = GetComponent<NavMeshAgent>();

        initTime = 0;
        currentState = AlienState.SCOUT;
        currentNode = AlienBrain.MostLikelyNode(nodeManager, temperature);

        nodesToIgnore = new List<GameObject>();
        pointsToFollow = new Queue<Vector3>();

        // This code is to enable manual control as to how the agent visually rotates
        agent.updateRotation = false;
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogError("Alien is missing a reference to the player. Please attach the player object in the inspector or else it would not work.");
            return;
        }

        // These methods check if the player is in view and sets the alien state accordingly
        UpdatePlayerInAlienFOV();
        EvaluateAlienSuspicion();
        
        // PURELY DEBUG - scrap this after testing
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Vector3 eventPosition = AlienBrain.MostLikelyNode(nodeManager, 0.3f).transform.position;
            InvokeSuspiciousEvent(eventPosition, 999f);

            Debug.DrawLine(eventPosition, transform.position, Color.magenta, 2f);
        }

        // Each method should have a condition that allows the alien to switch states

        // NOTE TO SELF: this sucks actually, ill replace it with a coroutine later (and a lock to stop it from constantly starting a new coroutine)
        // NOTE TO NOTE TO SELF: fuck you for even suggesting that
        // NOTE TO NOTE TO NOTE TO SELF: yeah no im keeping this
        switch (currentState)
        {
            case AlienState.SCOUT:
                scoutState();
                break;
            case AlienState.SUSPICIOUS:
                susState();
                break;
            case AlienState.CHASE:
                chaseState();
                break;
        }

        // Manually set the rotation of the alien to its velocity (I didn't like how the NavMeshAgent smooths out the rotation)
        if (agent.velocity != Vector3.zero)
            transform.rotation = Quaternion.Euler(0, Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(agent.velocity), 20f * Time.deltaTime).eulerAngles.y, 0);
    }

    public void scoutState()
    {
        // If the alien reaches its destination, find a new node to explore based on where the player would most likely be
        NavMeshHit hit;
        NavMesh.SamplePosition(agent.transform.position, out hit, 10, 1);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (pointsToFollow.Count == 0)
            {
                // If you get the same node, try again for a different one
                Node newNode = AlienBrain.MostLikelyNode(nodeManager, temperature);
                while (newNode == currentNode)
                {
                    newNode = AlienBrain.MostLikelyNode(nodeManager, temperature);
                }
                currentNode = newNode;

                List<Vector3> newPath = CalculatePaddedPathToNode(currentNode);
                foreach (Vector3 point in newPath)
                {
                    pointsToFollow.Enqueue(point);
                }
            }
            else
            {
                agent.SetDestination(pointsToFollow.Dequeue());
            }
        }
    }

    public void susState()
    {
        initTime += Time.deltaTime;

        // If the alien reaches its destination, find an adjacent node to explore that you haven't visited yet
        NavMeshHit hit;
        NavMesh.SamplePosition(agent.transform.position, out hit, 10, 1);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (pointsToFollow.Count == 0)
            {
                Node nextNodeToExplore = AlienBrain.PickAdjacentNodeToExplore(currentNode, nodeLayer, nodesToIgnore);
                currentNode = nextNodeToExplore;
                List<Vector3> newPath = CalculatePaddedPathToNode(currentNode);

                foreach (Vector3 point in newPath)
                {
                    pointsToFollow.Enqueue(point);
                }

                for (int i = 1; i < newPath.Count; i++)
                {
                    Debug.DrawLine(newPath[i - 1], newPath[i], Color.red, 10f);
                }
            }
            else
            {
                agent.SetDestination(pointsToFollow.Dequeue());
            }
        }

        // If more than 30 seconds pass without the alien finding the player, go back to regular scouting
        if (initTime >= 30)
        {
            Debug.Log("No more suspicious activity, scouting once again");
            initTime = 0;
            currentState = AlienState.SCOUT;

            // Clears all ignored nodes from the previous suspicious state
            nodesToIgnore.Clear();

            // Clears all nodes from the scout state
            pointsToFollow.Clear();
        }
    }

    public void chaseState()
    {
        if (canSeePlayer)
        {
            initTime = 0;
        }
        else
        {
            initTime += Time.deltaTime;
        }

        // If the alien loses direct line of sight for over a second, go to the suspicious state
        // Otherwise, keep following the player
        if (initTime > 1)
        {
            Debug.Log("Lost line of sight for more than a second, stay suspicious around the last known player location");
            initTime = 0;
            currentState = AlienState.SUSPICIOUS;

            currentNode = ClosestNodeToPoint(player.position);
            nodesToIgnore.Clear();
            pointsToFollow.Clear();

            List<Vector3> newPath = CalculatePaddedPathToNode(currentNode);
            foreach (Vector3 point in newPath)
            {
                pointsToFollow.Enqueue(point);
            }

            agent.SetDestination(pointsToFollow.Dequeue());
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    // Returns a random position around a node to keep pathfinding a bit more interesting
    private Vector3 RandomPositionAtCurrentNode(float range)
    {
        Vector3 randomPoint = currentNode.transform.position + (UnityEngine.Random.insideUnitSphere * range);
        NavMeshHit hit;
        while (!NavMesh.SamplePosition(randomPoint, out hit, range, 1)
            || Vector3.Distance(agent.transform.position, hit.position) < range)
        {
            randomPoint = currentNode.transform.position + (UnityEngine.Random.insideUnitSphere * range);
        }

        return hit.position;
    }

    // Sets the alien's state based on whether the player is in view and if they are too close
    private void EvaluateAlienSuspicion()
    {
        // Check that the alien has direct line of sight and isn't currently chasing anyone
        if (canSeePlayer && lineOfSight > lineOfSightThreshold && currentState != AlienState.CHASE)
        {
            nodesToIgnore.Clear();
            pointsToFollow.Clear();

            initTime = 0;
            if (Vector3.Distance(transform.position, player.position) < 15f)
            {
                // If the player is too close to the player, begin chasing them.
                Debug.Log("Player was definitely seen. Alien is now chasing.");
                currentState = AlienState.CHASE;
            }
            else
            {
                // Explore the area where the alien thinks the player is
                currentNode = ClosestNodeToPoint(player.position);
                List<Vector3> newPath = CalculatePaddedPathToNode(currentNode);

                foreach (Vector3 point in newPath)
                {
                    pointsToFollow.Enqueue(point);
                }

                agent.SetDestination(pointsToFollow.Dequeue());
                currentState = AlienState.SUSPICIOUS;
            }
        }
    }

    // Updates the lineOfSight and canSeePlayer variables
    private void UpdatePlayerInAlienFOV()
    {
        lineOfSight = Vector3.Dot(transform.forward.normalized, (player.transform.position - transform.position).normalized);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, playerLayer))
        {
            canSeePlayer = (hit.collider.gameObject == player.gameObject);
        }
    }

    // Gets the closest Node given a specific point
    public Node ClosestNodeToPoint(Vector3 position)
    {
        Collider[] nearbyNodes = Physics.OverlapSphere(position, 10f, nodeLayer);
        Node closestNode = nearbyNodes[0].GetComponent<Node>();

        float currDistance;
        float shortestDistance = Vector3.Distance(nearbyNodes[0].transform.position, position);
        foreach (Collider collider in nearbyNodes)
        {
            currDistance = Vector3.Distance(collider.transform.position, position);
            if (currDistance < shortestDistance)
            {
                shortestDistance = currDistance;
                closestNode = collider.GetComponent<Node>();
            }
        }

        return closestNode;
    }

    public List<Vector3> CalculatePaddedPathToNode(Node currentNode)
    {
        // Calculate a regular path to the node
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(RandomPositionAtCurrentNode(currentNode.range), path);

        // Make a copy of that path (since we cannot NavMeshPaths directly)
        List<Vector3> newPath = new List<Vector3>(path.corners);

        for (int i = 1; i < newPath.Count; i++)
        {
            Debug.DrawLine(newPath[i - 1], newPath[i], Color.red, 20f);
        }
        newPath.RemoveAt(0);

        // Modify the copy, pushing the points away from the edge of the NavMesh if they are too close
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            NavMeshHit edge;
            NavMesh.FindClosestEdge(newPath[i], out edge, 1);

            if (Vector3.Distance(edge.position, newPath[i]) < maxPathPadding)
            {
                newPath[i] += edge.normal * (maxPathPadding / Mathf.Clamp(Vector3.Distance(edge.position, newPath[i]), minPathPadding, maxPathPadding));
            }

            if (i > 0)
                Debug.DrawLine(newPath[i - 1], newPath[i], Color.green, 20f);
        }

        return newPath;
    }

    public void InvokeSuspiciousEvent(Vector3 eventPosition, float audibleRange)
    {
        if (Vector3.Distance(transform.position, eventPosition) < audibleRange)
        {
            nodesToIgnore.Clear();
            pointsToFollow.Clear();

            initTime = 0;
            currentNode = ClosestNodeToPoint(eventPosition);
            List<Vector3> newPath = CalculatePaddedPathToNode(currentNode);

            foreach (Vector3 point in newPath)
            {
                pointsToFollow.Enqueue(point);
            }

            agent.SetDestination(pointsToFollow.Dequeue());
            currentState = AlienState.SUSPICIOUS;
        }
    }

    void OnDrawGizmos()
    {
        // Gizmo for visualizing the forward vector of the alien
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Ray(transform.position, transform.forward));

        // Gizmo for visualizing line of sight
        if (lineOfSight > lineOfSightThreshold)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        if (player != null)
            Gizmos.DrawLine(transform.position, player.transform.position);

        Gizmos.color = Color.yellow;
        if (currentNode != null)
        {
            Gizmos.DrawCube(currentNode.transform.position, Vector3.one * 2);
            //Gizmos.DrawWireSphere(currentNode.transform.position, currentNode.range + 10f);
        }
    }
}


