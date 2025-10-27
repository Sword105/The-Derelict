using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField] public GameObject doorObject;
    [SerializeField] public bool IsOpen = false; 
    public override void Interact(GameObject player, Item activeItem)
    {
        Debug.Log(player.name + " is interacting with door " + gameObject.name);

        IsOpen = !IsOpen;
        GetComponentInChildren<Animator>().SetBool("IsOpen", IsOpen);

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