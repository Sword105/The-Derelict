using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS SCRIPT IS LIKELY TO BE CHANGED OR MERGED WITH ANOTHER SCRIPT
// This script is made for the sole purpose of setting up a framework for the alien's state machine, which is most likely going to merge with another script.

public enum AlienState
{
    SCOUT,
    SUSPICIOUS,
    CHASE
}

public class DEBUG_AlienStateMachine : MonoBehaviour
{
    public AlienState currentState;

    // This stores the time when an alien's state begins
    // This gets set to 0 right before switching states
    [SerializeField] private float initTime;

    // Number of times the alien has consecutively changed nodes to scout after the first node has been scouted
    [SerializeField] private int consecutiveScoutCount;

    // Check if an suspicious event occured;
    [SerializeField] private bool isSuspicious;

    private void Start()
    {
        initTime = 0;
        currentState = AlienState.SCOUT;
    }

    private void Update()
    {
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
        Debug.Log("CHASING");
    }
}


