using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : Item
{

    private void Start()
    {
        itemType = ItemType.Battery;
    }
    public override void Interact(GameObject player, Item activeItem)
    {
        Debug.Log("The Battery has been Picked up");

        //Gives the player the battery
        PlayerInteraction playerInteraction = player.GetComponent<PlayerInteraction>();
        playerInteraction.heldItemType = ItemType.Battery;

        //hides the battery from the world, idk how many problems that'll happen but fuck it
        gameObject.SetActive(false);
    }
}

