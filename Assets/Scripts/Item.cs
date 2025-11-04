using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    // This has not been finished yet
    // This is only here so I can test the PlayerInteraction script

    public AudioClip testSound;

    public override void Interact(GameObject player, Item activeItem)
    {
        if (testSound != null)
        {
            AudioManager audioManager = AudioManager.instance;
            audioManager.PlaySoundFX(testSound, transform.position, 1.0f, false);
        }

        Debug.Log("Item detected!");
    }
}
