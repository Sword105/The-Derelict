using UnityEngine;
using UnityEngine.AI;


public class DoorInteractable : Interactable
{
    public GameObject doorObject;
    public NavMeshObstacle obstacle;

    public bool IsOpen = false;
    public bool poweredOn = true;

    private void Start()
    {
        if (!poweredOn)
            obstacle.gameObject.SetActive(true);
        else
            obstacle.gameObject.SetActive(false);
    }

    public override void Interact(PlayerInteraction player, Item activeItem)
    {
        Debug.Log(player.name + " is interacting with door " + gameObject.name);

        if (poweredOn)
        {
            IsOpen = !IsOpen;
            obstacle.gameObject.SetActive(false);
            GetComponentInChildren<Animator>().SetBool("IsOpen", IsOpen);
            Debug.Log("The door is powered off and cannot be opened.");
            return;
        }

        // if (!isClosed)
        // {
        //     Debug.Log("The door opens.");
        //     isClosed = false;
        //     doorObject.GetComponentInChildren<Animator>().Play("door_2_open", 0, 0f);
        // }
        // else
        // {
        //     Debug.Log("The door closes.");
        //     isClosed = true;
        //     doorObject.GetComponentInChildren<Animator>().Play("door_2_close", 0, 0f);
        // }   

    }

}
