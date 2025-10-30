using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
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
public class DEBUG_AlienStateMachine : MonoBehaviour
{
    [Header("Setup")]
    public Transform player;
    public NodeManager nodeManager;
    public float lineOfSightAngle;
    public float minimumThreshold;
    //public float chaseRange = 10f;
    //public float stopDistance = 2f;
    public LayerMask viewLayers;

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

    // NOTE: lineOfSightAngle is a range that goes from -1 for completely behind the alien, 0 for perpendicular to the alien, 1 for perfectly in front of the alien, and everything in between
    [Header("DEBUG")]
    public AlienState currentState;

    // This stores the time when an alien's state begins
    // This gets set to 0 right before switching states
    [SerializeField] private float initTime;

    // Number of times the alien has consecutively changed nodes to scout after the first node has been scouted
    [SerializeField] private int consecutiveScoutCount;

    // Check if an suspicious event occured;
    [SerializeField] private bool isSuspicious;

    // Check to see if the alien can see the player
    [SerializeField] private bool canSeePlayer;

    // Dot product to see if the player is in line of sight
    [SerializeField] private float lineOfSight;

    // Dot product to see if the player is in line of sight
    [SerializeField] private Node currentNode;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        initTime = 0;
        currentState = AlienState.SCOUT;
        currentNode = MostLikelyNode();
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogError("Alien is missing a reference to the player. Please attach the player object in the inspector or else it would not work.");
            return;
        }

        // Calculating this outside of the state machine because this is important everywhere.
        lineOfSight = Vector3.Dot(transform.forward.normalized, (player.transform.position - transform.position).normalized);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, viewLayers))
        {
            canSeePlayer = (hit.collider.gameObject == player.gameObject);
        }

        // Each method should have a condition that allows the alien to switch states

        // NOTE TO SELF: this sucks actually, ill replace it with a coroutine later (and a lock to stop it from constantly starting a new coroutine)
        // NOTE TO NOTE TO SELF: fuck you for even suggesting that
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
    }

    public void scoutState()
    {
        if (agent.remainingDistance < 1)
            initTime += Time.deltaTime;

        if (initTime >= 5)
        {
            // If more than one node after the initial node has been scouted (in other words, after you scout your 3rd node)
            if (consecutiveScoutCount > 1) 
            {
                Debug.Log("Go to any random node on the map");

                currentNode = MostLikelyNode();
                agent.SetDestination(currentNode.transform.position);
                consecutiveScoutCount = 0;
            }
            else
            {
                Debug.Log("Go to a different point within the node's radius");

                Vector3 randomPoint = currentNode.transform.position + (UnityEngine.Random.insideUnitSphere * currentNode.range);
                NavMeshHit hit;
                while (!NavMesh.SamplePosition(randomPoint, out hit, currentNode.range, 1) 
                    || Vector3.Distance(agent.transform.position, hit.position) < currentNode.range)
                {
                    randomPoint = currentNode.transform.position + (UnityEngine.Random.insideUnitSphere * currentNode.range);
                }

                agent.SetDestination(hit.position);

                consecutiveScoutCount++;
            }

            initTime = 0;
        }

        if (isSuspicious) // If the alien finds something suspicious (maybe an event) then it will start doing suspicion logic
        {
            Debug.Log("Suspicious activity detected, investigating");
            initTime = 0;
            currentState = AlienState.SUSPICIOUS;
        }

        if (canSeePlayer && lineOfSight > lineOfSightAngle)
        {
            Debug.Log("Player within line of sight, now chasing");
            agent.ResetPath();
            initTime = 0;
            currentState = AlienState.CHASE;
        }
    }

    public void susState()
    {
        // Get a queue of suspicious activity (this ensures that the alien STAYS in the suspicious state when a different event happens while it is already investigating something
        // If the queue empties, go back to scouting
        Debug.Log("Observing suspicious activity");

        // This is a placeholder for now
        initTime += Time.deltaTime;
        if (initTime >= 20)
        {
            Debug.Log("No more suspicious activity, scouting once again");
            isSuspicious = false;
            initTime = 0;
            currentState = AlienState.SCOUT;
        }
    }

    public void chaseState()
    {
        // Pathfind to the player
        // If your line of sight breaks for more than 5 seconds, go to the last known position of the player and enter the suspicious state

        if (canSeePlayer)
        {
            initTime = 0;
        }
        else
        {
            initTime += Time.deltaTime;
        }

        if (initTime > 5)
        {
            Debug.Log("Lost line of sight for more than 5 seconds, returning to scouting");
            initTime = 0;
            currentState = AlienState.SCOUT;

            currentNode = MostLikelyNode();
            agent.SetDestination(currentNode.transform.position);
            consecutiveScoutCount = 0;
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    // Uses the "Roulette Wheel Selection" algorithm to calculate a node to go to
    // Don't ask me how it works unless you want gaps in your teeth
    private Node MostLikelyNode()
    {
        List<Node> allNodes = nodeManager.nodeList;

        // Make a new array to set cumulative probability values
        double[] cumulativeProbabilities = new double[allNodes.Count];
        cumulativeProbabilities[0] = Math.Pow(allNodes[0].nodeProbability, 1 / temperature);
        for (int i = 1; i < cumulativeProbabilities.Length; i++)
        {
            cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + allNodes[i].nodeProbability;
        }

        float valueToFind = UnityEngine.Random.Range(0f, 1f);

        for (int i = 0; i < cumulativeProbabilities.Length; i++)
        {
            if (valueToFind <= cumulativeProbabilities[i])
                return nodeManager.nodeList[i];
        }

        return nodeManager.nodeList[0];
    }

    void OnDrawGizmos()
    {
        // Gizmo for visualizing the forward vector of the alien
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Ray(transform.position, transform.forward));

        // Gizmo for visualizing line of sight
        if (lineOfSight > lineOfSightAngle)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawLine(transform.position, player.transform.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(currentNode.transform.position, Vector3.one * 2);

        /*
        Gizmos.color = Color.blue;
        NavMeshHit hit;
        NavMesh.FindClosestEdge(transform.position, out hit, 1);
        Gizmos.DrawRay(new Ray(hit.position, hit.normal));
        */
    }
}


