using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StaticItemList
{
    Generator,
    Door,
    Lever,
    Vent,
}
public class StaticInteractable : Interactable
{
    public StaticItemList staticItems;

    public override void Interact(PlayerInteraction player)
    {
        switch (staticItems)
        {
            case StaticItemList.Generator:
                InteractGenerator(player); break;

        }
    }
    private void InteractGenerator(PlayerInteraction player)
    {
        Debug.Log("ITEM GENERATOR HAS BEEN INTERACTED WITH");
    }
}

