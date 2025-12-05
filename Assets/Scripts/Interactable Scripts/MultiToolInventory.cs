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
    private bool isItemActive = false;
    public ItemID activeItem;
    public SpriteRenderer enemyIconRenderer;

    //Tool Configuration
    [SerializeField] private Light flashlightSource;
    [SerializeField] private AudioSource biotrackerAudio;

    //Power Drill timers
    [SerializeField] private float powerDrillHeldTime = 10.0f;
    [SerializeField] private float timeHeld = 0.0f;
    private bool hasPowerDrillBeenFired = false;

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
        if (playerInteraction == null){
            return;
        }

        //  Power Drill SELECT
        if ((Input.GetKeyDown(KeyCode.Alpha4) && playerInteraction.hasPowerDrill))
        {
            turnOffAllTools();
            activeItem = ItemID.PowerDrill;
            Debug.Log("4 is pressed and active item is: " + activeItem);
        }

        // ONLY call Use() for CLICK-based tools ========================================
        if (playerInteraction.hasMultiTool)
        {
            // CLICK-based tools (Flashlight, Tazer, Biotracker)
            if (Input.GetMouseButtonDown(0) && activeItem != ItemID.PowerDrill)
            {
                Use(activeItem);
            }

            //Flashlight Select -------------------
            if ((Input.GetKeyDown(KeyCode.Alpha1) && playerInteraction.hasFlashlight))
            {
                if (activeItem != ItemID.Flashlight && flashlightSource != null)
                {
                    turnOffAllTools();
                    activeItem = ItemID.Flashlight;
                    Debug.Log("1 is pressed and active item is: " + activeItem);
                }
            }

            //Biotracker Select -------------------
            else if ((Input.GetKeyDown(KeyCode.Alpha2) && playerInteraction.hasBiotracker))
            {
                if (activeItem != ItemID.BioTracker)
                {
                    turnOffAllTools();
                    activeItem = ItemID.BioTracker;
                    Debug.Log("2 is pressed and active item is: " + activeItem);
                }
            }

            //Tazer Select -------------------
            else if ((Input.GetKeyDown(KeyCode.Alpha3) && playerInteraction.hasTazer))
            {
                turnOffAllTools();
                activeItem = ItemID.Tazer;
                Debug.Log("3 is pressed and active item is: " + activeItem);
            }
        }


        if (activeItem == ItemID.PowerDrill)
        {
            // If button is held
            if (Input.GetMouseButton(0))
            {
                timeHeld += Time.deltaTime;
                Debug.Log("time held: " + timeHeld);

                if (!hasPowerDrillBeenFired && timeHeld >= powerDrillHeldTime)
                {
                    hasPowerDrillBeenFired = true;
                    FirePowerDrill();
                }
            }

            // If button is released
            if (Input.GetMouseButtonUp(0))
            {
                timeHeld = 0f;
                hasPowerDrillBeenFired = false;
                Debug.Log("The key LMB was released.");
            }
        }

        else if (playerInteraction.batteryCount > 0 && Input.GetKeyDown(KeyCode.Alpha5))
        {
            Use(ItemID.BATTERY);
            Debug.Log("5 is pressed and active item is: " + ItemID.BATTERY);
        }
    }

    //This makes all tools turned off when switching
    private void turnOffAllTools()
    {
        //Turns off flashlight components
        if (flashlightSource != null)
        {
            flashlightSource.enabled = false;
        }

        //Turns off biotracker components
        if (biotrackerAudio != null)
        {
            biotrackerAudio.Stop();
        }
        if (enemyIconRenderer != null)
        {
            enemyIconRenderer.enabled = false;
        }

        isItemActive = false;

        // Reset drill when switching tools
        timeHeld = 0f;
        hasPowerDrillBeenFired = false;
    }

    //Logic for using each item
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

    //Logic for using flashlight
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

    private void UseTazer() //NEEDS TO BE IMPLEMENTED
    {
        isItemActive = !isItemActive;
        Debug.Log("Tazer active state set to: " + isItemActive);
    }

    //Logic for using biotracker
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
        }

        if (enemyIconRenderer != null)
        {
            enemyIconRenderer.enabled = isItemActive;
        }
        else
        {
            Debug.LogError("Enemy Icon Renderer is not linked in the Inspector!");
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

    private void FirePowerDrill()
    {
        Debug.Log("DRILL FIRED!!!");
    }







}

