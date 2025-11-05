using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public enum ItemID
{
    None,
    MultiTool,
    Flashlight,
    BioTracker,
    Tazer,

}

public class Item : Interactable

{

    public PlayerInteraction playerInteraction;
    public MultiToolInventory inventory;


  
    public ItemID itemID;
    

    public override void Interact(PlayerInteraction player)
    {
        switch (itemID)
        {
            case ItemID.MultiTool:
                InteractMultiTool(player); 
                break;

            case ItemID.Flashlight:
                InteractFlashlight(player); 
                break;

            case ItemID.BioTracker:
                InteractBioTracker(player);
                break;
            case ItemID.Tazer:
                InteractTazer(player);
                break;

        }

    }

    private void InteractMultiTool(PlayerInteraction player)
    {
        Debug.Log("ITEM multitool HAS BEEN INTERACTED WITH");
        player.hasMultiTool = true;
        Debug.Log("HasMMT set to true");
        
    }

    private void InteractFlashlight(PlayerInteraction player) // Will set the hasFlashlight bool to true
    {

        player.hasFlashlight = true;

    }

    private void InteractBioTracker(PlayerInteraction player)
    {
        
        player.hasBiotracker = true;
        

    }

    private void InteractTazer(PlayerInteraction player)
    {

        player.hasTazer = true;

    }


    //:3


}




