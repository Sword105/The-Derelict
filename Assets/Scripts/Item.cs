using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ItemID
{
    Flashlight,
    BioTracker,
    Tazer,
    BATTERY,

}

public class Item : Interactable
{
    

    public AudioClip testSound;
    public ItemID itemID;

    public override void Interact(PlayerInteraction player)
    {
        switch (itemID)
        {
            case ItemID.Flashlight:
                InteractFlashlight(player); 
                break;

            case ItemID.BioTracker:
                
        }



    }

    private void InteractFlashlight(PlayerInteraction player)
    {
        Debug.Log("ITEM flashlight HAS BEEN INTERACTED WITH");
        AudioManager audioManager = AudioManager.instance;
        audioManager.PlaySoundFX(testSound, transform.position, 1.0f);   
    }

    private void InteractBattery(PlayerInteraction player)
    {
        Debug.Log("ITEM Battery HAS BEEN INTERACTED WITH");
        AudioManager audioManager = AudioManager.instance;
        audioManager.PlaySoundFX(testSound, transform.position, 1.0f);
    }

    //:3
    
    
}




