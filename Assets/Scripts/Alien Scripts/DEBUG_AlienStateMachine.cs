using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// THIS SCRIPT IS LIKELY TO BE CHANGED OR MERGED WITH ANOTHER SCRIPT
// This script is made for the sole purpose of setting up a framework for the alien's state machine, which is most likely going to merge with another script.

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
    public float lineOfSightAngle;
    //public float chaseRange = 10f;
    //public float stopDistance = 2f;
    public LayerMask viewLayers;

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

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        initTime = 0;
        currentState = AlienState.SCOUT;
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
        initTime += Time.deltaTime;
        if (initTime >= 10)
        {
            // If more than one node after the initial node has been scouted (in other words, after you scout your 3rd node)
            if (consecutiveScoutCount > 1) 
            {
                Debug.Log("Go to any random node on the map");
                consecutiveScoutCount = 0;
            }
            else
            {
                Debug.Log("Go to a different, nearby node");
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
            agent.ResetPath();
            initTime = 0;
            currentState = AlienState.SCOUT;
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        /*
        // Gizmo for visualizing max distance for the alien to chase the player
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        */
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
    }
}


