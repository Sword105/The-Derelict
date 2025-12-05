using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : Item
{

    private void Start()
    {
        itemID = ItemID.BATTERY;
    }
    public override void Interact(PlayerInteraction player, Item activeItem)
    {
        Debug.Log("The Battery has been Picked up");

        //Gives the player the battery
        PlayerInteraction playerInteraction = player.GetComponent<PlayerInteraction>();
        playerInteraction.heldItemType = ItemID.BATTERY;

        //hides the battery from the world, idk how many problems that'll happen but fuck it
        gameObject.SetActive(false);
    }
}



//Test Enum so I can implement the batteries and they don't just leave from the inventory
public enum ItemType
{
    None,
    Battery,
    Flashlight, whatever
}

