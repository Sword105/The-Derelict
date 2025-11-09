using UnityEngine;


public class DoorInteractable : Interactable

{
    public GameObject doorObject;
    public bool IsOpen = false;
    public bool poweredOn = true;
    public override void Interact(PlayerInteraction player)
    {
        Debug.Log(player.name + " is interacting with door " + gameObject.name);

        if (poweredOn)
        {
            IsOpen = !IsOpen;
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
