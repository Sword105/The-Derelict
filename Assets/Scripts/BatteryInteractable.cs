using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryInteractable : Item


{

    public float chargePercent = 0f;
    
    public GameObject batteryObject;
    

    // Start is called before the first frame update
    void Start()
    {
        batteryObject = gameObject;
    }

    public virtual void Interact(GameObject player, Item activeItem)
    {
        Debug.Log(player.name + " is interacting with object " + item.name);
    }

}
