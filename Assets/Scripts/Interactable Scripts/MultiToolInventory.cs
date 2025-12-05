using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiToolInventory : MonoBehaviour 
{
    public PlayerInteraction playerInteraction;
    public ItemID itemID;
    [SerializeField] private Boolean isItemActive = false;
    public ItemID activeItem;
    [SerializeField] private Light flashlightSource;

    void Start()
    {
        playerInteraction = FindObjectOfType<PlayerInteraction>();

        Debug.Log("here we go" + playerInteraction.hasMultiTool);
        playerInteraction.hasMultiTool = true; //TESTING TALE OUJT
        Debug.Log("here we go" + playerInteraction.hasMultiTool);

        if (playerInteraction == null)
            Debug.LogError("PlayerInteraction not found in scene!");

        // Ensure the light starts off 
        if (flashlightSource != null)
        {
            flashlightSource.enabled = false;
        }
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

            // Flashlight Select
            if ((Input.GetKeyDown(KeyCode.Alpha1) && playerInteraction.hasFlashlight))
            {
                // Turn off the active tool immediately if switching
                if (activeItem != ItemID.Flashlight && flashlightSource != null)
                {
                    flashlightSource.enabled = false;
                }
                isItemActive = false;
                activeItem = ItemID.Flashlight;
                Debug.Log("1 is pressed and active item is: " + activeItem);
            }

            // Biotracker Select
            else if ((Input.GetKeyDown(KeyCode.Alpha2) && playerInteraction.hasBiotracker))
            {
                if (flashlightSource != null) { flashlightSource.enabled = false; }
                isItemActive = false;
                activeItem = ItemID.BioTracker;
                Debug.Log("2 is pressed and active item is: " + activeItem);
            }

            // Tazer Select
            else if ((Input.GetKeyDown(KeyCode.Alpha3) && playerInteraction.hasTazer))
            {
                if (flashlightSource != null) { flashlightSource.enabled = false; }
                isItemActive = false;
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
        float nextTazerTime = 0f;
        float tazerCooldown = 5f;

        if (Time.time < nextTazerTime)
        {
            Debug.Log("Tazer is recharging, please wait.");
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.collider.CompareTag("Alien"))
            {
                Debug.Log("Tazer hit the alien!");
                nextTazerTime = Time.time + tazerCooldown;
                AlienStateMachine.instance.Stun(1.5f);
                // StartCoroutine(AlienStateMachine.instance.Stun(1.5f));
            }
            else
            {
                Debug.Log("Tazer missed.");
                return;
            }
        }
        else
        {
            Debug.Log("Tazer missed.");
            return;
        }

        isItemActive = !isItemActive;
        Debug.Log("Tazer active state set to: " + isItemActive);

        // AlienStateMachine.instance.Stun(1.5f);
        
    }

    private void UseBiotracker() 
    {
        isItemActive = !isItemActive;
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