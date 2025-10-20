using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryInteractable : Interactable


{

    public float chargePercent = 0f;
    public GameObject batteryObject;
    public LayerMask interactableLayer;

    // Start is called before the first frame update
    void Start()
    {
        batteryObject = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
