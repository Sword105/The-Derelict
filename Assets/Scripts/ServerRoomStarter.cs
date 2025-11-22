using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.AI;

public class ServerRoomStarter : Interactable
{
    public DoorInteractable doorToLock;
    public float timeUntilCompletion;
    public bool eventStarted;
    public bool eventCompleted;
    public Transform locationToTeleportAlien;

    public static ServerRoomStarter instance;
    public Collider serverRoomBounds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        eventCompleted = false;
        eventStarted = false;
    }

    public override void Interact(PlayerInteraction player)
    {
        if (!eventCompleted && !eventStarted)
        {
            eventStarted = true;
            StartCoroutine(ServerRoomEvent());
        }
    }

    private IEnumerator ServerRoomEvent()
    {
        doorToLock.poweredOn = false;

        if (doorToLock.IsOpen)
        {
            doorToLock.IsOpen = false;
            doorToLock.GetComponentInChildren<Animator>().SetBool("IsOpen", doorToLock.IsOpen);
        }

        NavMeshAgent alienAgent = AlienStateMachine.instance.GetComponent<NavMeshAgent>();
        AlienStateMachine.instance.inServerRoom = true;

        alienAgent.enabled = false;
        alienAgent.transform.position = locationToTeleportAlien.position;
        alienAgent.enabled = true;
        // We also need to tell the alien to enter a particular state in this room specifically

        for (int seconds = 0; seconds < timeUntilCompletion; seconds++)
        {
            Debug.Log("Time Elapsed in Server Room: " + seconds);
            yield return new WaitForSeconds(1f);
        }

        AlienStateMachine.instance.inServerRoom = false;
        doorToLock.IsOpen = true;
        doorToLock.poweredOn = true;
        doorToLock.GetComponentInChildren<Animator>().SetBool("IsOpen", true);
    }
}
