using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryInteractable : Interactable

{
    
    
    [Header("Battery Settings")]
    public ItemID itemID = ItemID.BATTERY;

    public PlayerInteraction playerInteraction;

    private void Start()
    {
        if (playerInteraction == null)
            playerInteraction = FindObjectOfType<PlayerInteraction>();
    }

    // Player picks up the battery
    public override void Interact(PlayerInteraction player)
    {
        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();

        // Prevent overflow
        if (pi.batteryCount >= 6)
        {
            Debug.Log("Player has the maximum number of batteries.");
            return;
        }

        // Add battery to player inventory
        pi.inventory.Add(ItemID.BATTERY);
        pi.batteryCount++;
        pi.heldItemType = ItemID.BATTERY;

        Debug.Log($"Battery picked up. Total: {pi.batteryCount}");

        // Hide or destroy object in scene
        gameObject.SetActive(false);
    }

    // Use 1 battery
    public void UseBattery(PlayerInteraction player)
    {
        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();

        if (pi.batteryCount > 0)
        {
            pi.inventory.Remove(ItemID.BATTERY);
            pi.batteryCount--;
            Debug.Log("Battery used. Remaining: " + pi.batteryCount);
        }
        else
        {
            Debug.Log("No batteries to use.");
        }
    }

    // Use 6 batteries for a generator (example)
    public bool ConsumeForGenerator(PlayerInteraction player)
    {
        PlayerInteraction pi = player.GetComponent<PlayerInteraction>();

        if (pi.batteryCount >= 6)
        {
            for (int i = 0; i < 6; i++)
            {
                pi.inventory.Remove(ItemID.BATTERY);
            }

            pi.batteryCount -= 6;
            Debug.Log("Used 6 batteries for generator.");
            return true;
        }

        Debug.Log("Not enough batteries for generator.");
        return false;
    }
    
}
