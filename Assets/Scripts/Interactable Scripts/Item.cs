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
    BATTERY,

}

public class Item : Interactable

{

    public PlayerInteraction playerInteraction;


    public AudioClip testSound;
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
                
        }



    }

    private void InteractMultiTool(PlayerInteraction player)
    {
        Debug.Log("ITEM multitool HAS BEEN INTERACTED WITH");
        playerInteraction.hasMultiTool = true;
        Debug.Log("HasMMT set to true");
        
    }

    private void InteractFlashlight(PlayerInteraction player) // Will set the hasFlashlight bool to true and add to MTT
    {
        Debug.Log("ITEM flashlight HAS BEEN INTERACTED WITH");
        
    }

    private void InteractBioTracker(PlayerInteraction player)
    {
        Debug.Log("ITEM BioTracker HAS BEEN INTERACTED WITH");
        
    }

    //:3
    
    
}




