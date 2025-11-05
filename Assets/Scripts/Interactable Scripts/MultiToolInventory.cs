using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiToolInventory : MonoBehaviour, IUsable
{
    public PlayerInteraction playerInteraction;
    public ItemID itemID;
    [SerializeField] private 



    void Update()
    {
        if (playerInteraction == null)
            return;

        if (playerInteraction.hasMultiTool)
        {
            if ((Input.GetKeyDown(KeyCode.Alpha1) && playerInteraction.hasFlashlight)){
                if (Input.GetMouseButtonDown(0))
                {
                    UseFlashlight(playerInteraction);
                }
            }
            


        }


    }
    public void AddItemToMultiTool(ItemID newItem)
    {
        playerInteraction.inventory.Add(newItem);
        Debug.Log("Added " + newItem + " to multitool inventory");
    }

    private void Use(PlayerInteraction player)
    {
        switch(itemID)
        {
            case ItemID.None: 
                break;

            case ItemID.Flashlight:
                UseFlashlight(player);
                break;
        }
    }

    public void UseFlashlight(PlayerInteraction player)
    {
        Debug.Log("Hey whats up im the FLASHLIGHT being used lmao");
    }

}
