using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryInteractable : Interactable


{

    public float chargePercent = 0f;

    public int batteryCount;
    
    public GameObject batteryObject;
    

    // Start is called before the first frame update
    void Start()
    {
        batteryObject = gameObject;
    }

    public override void Interact(PlayerInteraction player)
    {
        //Debug.Log(player.name + " is interacting with object " + batteryObject.name);
    }

}
