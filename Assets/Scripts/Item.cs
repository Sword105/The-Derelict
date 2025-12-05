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
    PowerDrill

}

public class Item : Interactable

{

    public PlayerInteraction playerInteraction;
    // public MultiToolInventory inventory;



    public ItemID itemID = ItemID.None;
    //public ItemType itemType = Item.None;
    public AudioClip testSound;

    public override void Interact(PlayerInteraction player, Item activeItem)
    {
        HotbarManager hud = FindObjectOfType<HotbarManager>();

        //My ItemType Shit
        Debug.Log("$Picked up an item of type: {itemType }");
        PlayerInteraction playerInteraction = player.GetComponent<PlayerInteraction>();
        playerInteraction.heldItemType = itemID;

        /* if (testSound != null)
        {
            AudioManager audioManager = AudioManager.instance;
            audioManager.PlaySoundFX(testSound, transform.position, 1.0f);
        } */

        Debug.Log("Item detected!");

        //Hides from Game Scene
        gameObject.SetActive(false);
     
    


        if (hud != null)
        {
            hud.AddItemToHud(itemID);
        }

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
            case ItemID.BATTERY:
                InteractBattery(player);
                break;
            case ItemID.PowerDrill:
                InteractPowerDrill(player);
                break;

        }


    }

    private void InteractMultiTool(PlayerInteraction player)
    {
        Debug.Log("ITEM multitool HAS BEEN INTERACTED WITH");
        player.hasMultiTool = true;
        Debug.Log("HasMMT set to true");
        Destroy(gameObject);

    }

    private void InteractFlashlight(PlayerInteraction player) // Will set the hasFlashlight bool to true
    {
        player.hasFlashlight = true;
        Destroy(gameObject);

    }

    private void InteractBioTracker(PlayerInteraction player)
    {

        player.hasBiotracker = true;
        Destroy(gameObject);

    }

    private void InteractTazer(PlayerInteraction player)
    {
        player.hasTazer = true;
        Destroy(gameObject);
    }


    private void InteractBattery(PlayerInteraction player)
    {
        if (playerInteraction.batteryCount >= 6)
        {
            Debug.Log("Player has maximum of batteries in inventory");
            return;
        }

        //player.inventory.Add(ItemID.BATTERY);
        playerInteraction.batteryCount++;
        Debug.Log("Battery collected. Total batteries: " + playerInteraction.batteryCount);
        Destroy(gameObject);
    }

    private void InteractPowerDrill(PlayerInteraction player){
        player.hasPowerDrill = true;
        Debug.Log("Power drill collected");
        Destroy(gameObject);

    }

    //:3


}




