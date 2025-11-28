using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MultiToolInventory : MonoBehaviour 
{
    public static Action<bool> OnBiotrackerUse;
    public PlayerInteraction playerInteraction;
    public ItemID itemID;
    [SerializeField] private Boolean isItemActive = false;
    public ItemID activeItem;

    //Tool Configuration
    [SerializeField] private Light flashlightSource;
    [SerializeField] private GameObject biotrackerVisuals;
    [SerializeField] private AudioSource biotrackerAudio;


    void Start()
    {
        playerInteraction = FindObjectOfType<PlayerInteraction>();

        if (playerInteraction == null)
            Debug.LogError("PlayerInteraction not found in scene!");

        //Ensures all tools start off 
        turnOffAllTools();
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

            //Flashlight Select
            if ((Input.GetKeyDown(KeyCode.Alpha1) && playerInteraction.hasFlashlight))
            {
                //Turn off the active tool immediately if switching
                if (activeItem != ItemID.Flashlight && flashlightSource != null)
                {
                    //Turns off other tools when switching
                    turnOffAllTools();
                    activeItem = ItemID.Flashlight;
                    Debug.Log("1 is pressed and active item is: " + activeItem);
                }
            }

            //Biotracker Select
            else if ((Input.GetKeyDown(KeyCode.Alpha2) && playerInteraction.hasBiotracker))
            {
                if (activeItem != ItemID.BioTracker)
                {
                    //Turns off other tools when switching
                    turnOffAllTools();
                    activeItem = ItemID.BioTracker;
                    Debug.Log("2 is pressed and active item is: " + activeItem);
                }
            }

            //Tazer Select
            else if ((Input.GetKeyDown(KeyCode.Alpha3) && playerInteraction.hasTazer))
            {
                //Turns off other tools when switching
                turnOffAllTools();
                activeItem = ItemID.Tazer;
                Debug.Log("3 is pressed and active item is: " + activeItem);
            }
        }
    }

    public void AddItemToMultiTool(ItemID newItem) 
    {
        playerInteraction.inventory.Add(newItem);
        Debug.Log("Added " + newItem + " to multitool inventory");
    }

    //This makes all tools turned off when switching
    private void turnOffAllTools()
    {
        if (flashlightSource != null)
        {
            flashlightSource.enabled = false;
        }

        if (biotrackerVisuals != null)
        {
            biotrackerVisuals.SetActive(false);
        }

        if (biotrackerAudio != null)
        {
            biotrackerAudio.Stop();
        }

    }

    private void Use(ItemID currentActiveItem)
    {
        switch (currentActiveItem)
        {
            case ItemID.None:
                Debug.Log("you cant use nothing mate");
                break;

            case ItemID.Flashlight:
                UseFlashlight(); 
                break;

            case ItemID.Tazer:
                UseTazer(); 
                break;

            case ItemID.BioTracker:
                UseBiotracker(); 
                break;

            case ItemID.BATTERY:
                UseBattery(playerInteraction);
                break;
        }
    }

    private void UseFlashlight() 
    {
        //Toggle the state
        isItemActive = !isItemActive;

        //Controls the light
        if (flashlightSource != null)
        {
            flashlightSource.enabled = isItemActive;
        }
        else
        {
            Debug.LogError("CRITICAL ERROR: Flashlight Source is not connected in the Inspector.");
        }

        Debug.Log("Flashlight active state set to: " + isItemActive);
    }

    private void UseTazer() 
    {
        isItemActive = !isItemActive;
        Debug.Log("Tazer active state set to: " + isItemActive);
    }


    //UNFINISHED: Biotracker cannot display alien on minimap yet
    private void UseBiotracker() 
    {
        isItemActive = !isItemActive;

        //Play biotracker audio when activated
        if (isItemActive)
        {
            if(biotrackerAudio != null)
            {
                biotrackerAudio.Play();
            }
            else
            {
                Debug.LogError("Audio not detected.");
            }
            //Checks if alien can hear the noise
            OnBiotrackerUse?.Invoke(true);
        }
        else
        {
            if(biotrackerAudio != null)
            {
                biotrackerAudio.Stop();
            }
            OnBiotrackerUse?.Invoke(false);
        }
        Debug.Log("Biotracker active state set to: " + isItemActive);
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