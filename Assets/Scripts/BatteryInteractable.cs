using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryInteractable : Interactable


{

    public float chargePercent = 0f;

    public int batteryCount;
    
    public GameObject battery;
    

    // Start is called before the first frame update
    void Start()
    {
        battery = gameObject;
    }

    public override void Interact(PlayerInteraction player)
    {
        //Debug.Log(player.name + " is interacting with object " + batteryObject.name);
    }

    public void CollectBattery(PlayerInteraction player)
    {
        //Max battery count can be changed, I just put 5 for testing purposes
        if (batteryCount >= 5)
        {
            Debug.Log("Player has maximum of batteries in inventory");
            return;
        }

        player.inventory.Add(ItemID.BATTERY);
        batteryCount++;
        Destroy(battery);
    }

    //Would need to be a used method in order to use the battery on items that require it
    public void UseBattery(PlayerInteraction player)
    {
        if (player.inventory.Contains(ItemID.BATTERY))
        {
            if(batteryCount == 1)
            {
            player.inventory.Remove(ItemID.BATTERY);
            batteryCount--;
            Debug.Log("Battery used. Remaining batteries: " + batteryCount);
            } else if(batteryCount == 5)
            {
                for (int i = 0; i < 6; i++)
                {
                    player.inventory.Remove(ItemID.BATTERY);
                    batteryCount--;

                }
                Debug.Log("Batteries used. Remaining batteries: " + batteryCount);
            }
            
        }
        else
        {
            Debug.Log("No batteries in inventory to use.");
        }
    }

}
