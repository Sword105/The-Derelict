using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField] public GameObject doorObject;
    [SerializeField] public bool doorOpen = false;
    public override void Interact(GameObject player, Item activeItem)
    {
        Debug.Log(player.name + " is interacting with door " + gameObject.name);

        if (doorOpen)
        {
            Debug.Log("The door opens.");
            // doorOpen = true;
            doorObject.GetComponentInChildren<Animator>().Play("door_2_open", 0, 0f);
        }
    
    }

}