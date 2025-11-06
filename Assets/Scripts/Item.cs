using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    // This has not been finished yet
    // This is only here so I can test the PlayerInteraction script
    // Added more stuff for the ItemType Script

    
    public ItemType itemType = ItemType.None;
    public AudioClip testSound;


    public override void Interact(GameObject player, Item activeItem)
    {
        //My ItemType Shit
        Debug.Log("$Picked up an item of type: {itemType }");
        PlayerInteraction playerInteraction = player.GetComponent<PlayerInteraction>();
        playerInteraction.heldItemType = itemType; 

        if (testSound != null)
        {
            AudioManager audioManager = AudioManager.instance;
            audioManager.PlaySoundFX(testSound, transform.position, 1.0f);
        }

        Debug.Log("Item detected!");

        //Hides from Game Scene
        gameObject.SetActive(false);
    }
}
