using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.AI;

public enum AlienState : byte
{
    SCOUT,
    SUSPICIOUS,
    CHASE
}

[RequireComponent(typeof(NavMeshAgent))]
public class AlienStateMachine : MonoBehaviour
{
    private Transform player;
    private NodeManager nodeManager;
    private NavMeshAgent agent;
    public static AlienStateMachine instance;
    public bool inServerRoom;

    /*********************************************************************************************************************/

    [Header("Setup")]
    public float lineOfSightThreshold;
    public float suspiciousStateMaxTimeLength = 20f;
    public float chaseTimeUntilGiveUp = 1f;

    public LayerMask playerLayer;
    public LayerMask nodeLayer;
    public LayerMask susNodeLayer;

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

    [Header("DEBUG")]
    public AlienState currentState;

    [SerializeField] private float timeInState;
    [SerializeField] private bool canSeePlayer;
    [SerializeField] private float lineOfSightDotProduct;
    [SerializeField] public Node currentNode;
    [SerializeField] private List<GameObject> nodesToIgnore;        // Specific to suspicious state
    [SerializeField] public Queue<Vector3> pointsToFollow;          // Specific to suspicious state

    /*********************************************************************************************************************/

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        nodeManager = NodeManager.instance;
        agent = GetComponent<NavMeshAgent>();

        nodesToIgnore = new List<GameObject>();
        pointsToFollow = new Queue<Vector3>();

        ClearStateData();
        currentState = AlienState.SCOUT;
        currentNode = AlienBrain.MostLikelyNode(nodeManager, temperature);

        agent.updateRotation = false;
        agent.isStopped = false;
        inServerRoom = false;
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Alien is missing a reference to the player. Please attach the player object in the inspector or else it would not work.");
            return;
        }

        // These methods check if the player is in view and sets the alien state accordingly
        UpdatePlayerInAlienFOV();
        HandleAlienSuspicion();

        // NOTE TO SELF: this sucks actually, ill replace it with a coroutine later (and a lock to stop it from constantly starting a new coroutine)
        // NOTE TO NOTE TO SELF: fuck you for even suggesting that
        // NOTE TO NOTE TO NOTE TO SELF: yeah no im keeping this
        if (!agent.isStopped)
        {
            switch (currentState)
            {
                case AlienState.SCOUT:
                    if (inServerRoom)
                        ServerRoomState();
                    else
                        ScoutState();
                    break;
                case AlienState.SUSPICIOUS:
                    SuspiciousState();
                    break;
                case AlienState.CHASE:
                    ChaseState();
                    break;
            }
        }

        // Manually set the rotation of the alien to its velocity (I didn't like how the NavMeshAgent smooths out the rotation)
        if (agent.velocity != Vector3.zero)
            transform.rotation = Quaternion.Euler(0, Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(agent.velocity), 20f * Time.deltaTime).eulerAngles.y, 0);
    }

    public void ScoutState()
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

    public void SuspiciousState()
    {
        timeInState += Time.deltaTime;

        // If the alien reaches its destination, find an adjacent node to explore that you haven't visited yet
        NavMeshHit hit;
        NavMesh.SamplePosition(agent.transform.position, out hit, 10f, NavMesh.AllAreas);
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
            }
            agent.SetDestination(pointsToFollow.Dequeue());
        }

        // If more than 30 seconds pass without the alien finding the player, go back to regular scouting
        if (timeInState >= suspiciousStateMaxTimeLength)
        {
            Debug.Log("No more suspicious activity, scouting once again");
            ClearStateData();
            StartCoroutine(HandleStateTransition(1f));
            currentState = AlienState.SCOUT;
        }
    }

    public void ChaseState()
    {
        if (canSeePlayer)
        {
            timeInState = 0f;
        }
        else
        {
            timeInState += Time.deltaTime;
        }

        // If the alien loses direct line of sight for over a second, go to the suspicious state
        // Otherwise, keep following the player
        if (timeInState > chaseTimeUntilGiveUp)
        {
            ClearStateData();
            currentState = AlienState.SUSPICIOUS;

            currentNode = ClosestNodeToPoint(player.position);
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

    public void ServerRoomState()
    {
        // If the alien reaches its destination, find a new node to explore based on where the player would most likely be
        if (agent.remainingDistance <= agent.stoppingDistance)
        {

            Collider serverRoomBounds = ServerRoomStarter.instance.serverRoomBounds;
            Vector3 randomPosition = new Vector3(Random.Range(serverRoomBounds.bounds.min.x, serverRoomBounds.bounds.max.x), serverRoomBounds.bounds.center.y, Random.Range(serverRoomBounds.bounds.min.z, serverRoomBounds.bounds.max.z));

            NavMeshHit hit;
            while (!NavMesh.SamplePosition(randomPosition, out hit, 10, 1)) {
                NavMesh.SamplePosition(randomPosition, out hit, 10, 1);

                randomPosition = new Vector3(Random.Range(serverRoomBounds.bounds.min.x, serverRoomBounds.bounds.max.x), serverRoomBounds.bounds.center.y, Random.Range(serverRoomBounds.bounds.min.z, serverRoomBounds.bounds.max.z));
            }

            StartCoroutine(HandleStateTransition(1f));
            agent.SetDestination(randomPosition);
        }
    }

    public void InvokeSuspiciousEvent(Vector3 eventPosition, float audibleRange)
    {
        if (Vector3.Distance(transform.position, eventPosition) < audibleRange)
        {
            ClearStateData();

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

    /*********************************************************************************************************************/
    // Everything below are helper methods and are not meant to be public

    private void ClearStateData()
    {
        timeInState = 0;
        pointsToFollow.Clear();
        nodesToIgnore.Clear();
        agent.ResetPath();
    }

    private IEnumerator HandleStateTransition(float timeToTransition)
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(timeToTransition);
        agent.isStopped = false;
    }

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

    private void HandleAlienSuspicion()
    {
        Collider[] nearbySusNodes = Physics.OverlapSphere(transform.position, 10f, susNodeLayer);

        // TO-DO: There is a bug that allows the alien to see this through walls
        if (nearbySusNodes.Length > 0 && nearbySusNodes[0] != null)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, nearbySusNodes[0].transform.position - transform.position, out hit);
            if (hit.collider == nearbySusNodes[0])
            {
                ClearStateData();
                StartCoroutine(HandleStateTransition(1f));
                currentState = AlienState.SUSPICIOUS;

                Transform impliedObject = nearbySusNodes[0].GetComponent<SuspiciousNodeData>().impliedObject;
                if (impliedObject != null)
                {
                    currentNode = ClosestNodeToPoint(impliedObject.position);
                }
                else
                {
                    currentNode = ClosestNodeToPoint(transform.position);
                }

                List<Vector3> newPath = CalculatePaddedPathToNode(currentNode);

                foreach (Vector3 point in newPath)
                {
                    pointsToFollow.Enqueue(point);
                }

                agent.SetDestination(pointsToFollow.Dequeue());
                Destroy(nearbySusNodes[0].gameObject);
                return;
            }
        }


        // Check that the alien has direct line of sight and isn't currently chasing anyone
        if (canSeePlayer && lineOfSightDotProduct > lineOfSightThreshold && currentState != AlienState.CHASE)
        {
            ClearStateData();

            if (Vector3.Distance(transform.position, player.position) < 15f)
            {
                // If the player is too close to the player, begin chasing them.
                Debug.Log("Player was definitely seen. Alien is now chasing.");
                StartCoroutine(HandleStateTransition(1f));
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
                StartCoroutine(HandleStateTransition(1f));
                currentState = AlienState.SUSPICIOUS;
            }
        }
    }

    private void UpdatePlayerInAlienFOV()
    {
        lineOfSightDotProduct = Vector3.Dot(transform.forward.normalized, (player.transform.position - transform.position).normalized);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, playerLayer))
        {
            canSeePlayer = (hit.collider.gameObject == player.gameObject);
        }
    }

    private Node ClosestNodeToPoint(Vector3 position)
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

    private List<Vector3> CalculatePaddedPathToNode(Node currentNode)
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

    void OnTriggerEnter(Collider other)
    {
        DoorInteractable door = other.GetComponent<DoorInteractable>();

        if (door != null && !door.IsOpen)
        {
            StartCoroutine(HandleStateTransition(1f));

            door.IsOpen = !door.IsOpen;
            door.GetComponentInChildren<Animator>().SetBool("IsOpen", door.IsOpen);
        }
    }

    void OnTriggerExit(Collider other)
    {
        DoorInteractable door = other.GetComponent<DoorInteractable>();

        if (door != null && door.IsOpen)
        {
            door.IsOpen = !door.IsOpen;
            door.GetComponentInChildren<Animator>().SetBool("IsOpen", door.IsOpen);
        }
    }

    void OnDrawGizmos()
    {
        // Gizmo for visualizing the forward vector of the alien
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Ray(transform.position, transform.forward));

        // Gizmo for visualizing line of sight
        if (lineOfSightDotProduct > lineOfSightThreshold)
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


