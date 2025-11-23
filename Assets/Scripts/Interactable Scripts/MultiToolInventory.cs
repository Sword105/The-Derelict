using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiToolInventory : MonoBehaviour, IUsable
{
    public PlayerInteraction playerInteraction;
    public ItemID itemID;
    [SerializeField] private Boolean isItemActive = false;
    public ItemID activeItem;

    void Start()
    {
        // Automatically find the PlayerInteraction script in the scene
        playerInteraction = FindObjectOfType<PlayerInteraction>();

        if (playerInteraction == null)
            Debug.LogError("PlayerInteraction not found in scene!");
    }


    void Update()
    {
        if (playerInteraction == null)
            return;

        // Logic needed to use batteries on other items, right now just click 4 and it uses it
        if (playerInteraction.batteryCount > 0 && Input.GetKeyDown(KeyCode.Alpha4))
        {
            Use(ItemID.BATTERY);
            Debug.Log("4 is pressed and active item is: " + ItemID.BATTERY);
        }

        if (playerInteraction.hasMultiTool)
        {
            if (Input.GetMouseButtonDown(0))
            {

                Use(activeItem);

            }

            if ((Input.GetKeyDown(KeyCode.Alpha1) && playerInteraction.hasFlashlight))
            {
                isItemActive = false;
                activeItem = ItemID.Flashlight;
                Debug.Log("1 is pressed and active item is: " + activeItem);

            }

            else if ((Input.GetKeyDown(KeyCode.Alpha2) && playerInteraction.hasBiotracker))
            {
                isItemActive = false;
                activeItem = ItemID.BioTracker;
                Debug.Log("2 is pressed and active item is: " + activeItem);

            }

            else if ((Input.GetKeyDown(KeyCode.Alpha3) && playerInteraction.hasTazer))
            {
                isItemActive = false;
                activeItem = ItemID.Tazer;
                Debug.Log("3 is pressed and active item is: " + activeItem);

            }






        }


    }
    public void AddItemToMultiTool(ItemID newItem) //kind of redundant 
    {
        playerInteraction.inventory.Add(newItem);
        Debug.Log("Added " + newItem + " to multitool inventory");
    }

    private void Use(ItemID activeItem)
    {
        switch (activeItem)
        {
            case ItemID.None:
                Debug.Log("you cant use nothing mate");
                break;

            case ItemID.Flashlight:
                UseFlashlight(activeItem);
                break;

            case ItemID.Tazer:
                UseTazer(activeItem);
                break;

            case ItemID.BioTracker:
                UseBiotracker(activeItem);
                break;
            case ItemID.BATTERY:
                UseBattery(playerInteraction);
                break;


        }
    }

    private void UseFlashlight(ItemID itemID)
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isItemActive)
            {
                isItemActive = false;
            }
            else
            {
                isItemActive = true;
            }

            Debug.Log("isActive set to" + isItemActive + "for FLASHLIGHT");
        }

    }

    private void UseTazer(ItemID itemID)
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isItemActive)
            {
                isItemActive = false;
            }
            else
            {
                isItemActive = true;
            }

            Debug.Log("isActive set to" + isItemActive + "for TAZER");
        }

    }

    private void UseBiotracker(ItemID itemID)
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isItemActive)
            {
                isItemActive = false;
            }
            else
            {
                isItemActive = true;
            }

            Debug.Log("isActive set to" + isItemActive + "for BIOTRACKER");
        }

    }

    //More logic for using batteries here
    public void UseBattery(PlayerInteraction player)
    {
        if (player.inventory.Contains(ItemID.BATTERY))
        {
            if (playerInteraction.batteryCount >= 1)
            {
                player.inventory.Remove(ItemID.BATTERY);
                playerInteraction.batteryCount--;
                Debug.Log("Battery used. Remaining batteries: " + playerInteraction.batteryCount);
            }


        }
        else
        {
            Debug.Log("No batteries in inventory to use.");
        }
    }

}