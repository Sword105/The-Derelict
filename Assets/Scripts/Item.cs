using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactable
{
    // This has not been finished yet
    // This is only here so I can test the PlayerInteraction script
    public override void Interact(GameObject player, Item activeItem)
    {
        Debug.Log("Item detected!");
    }
}
