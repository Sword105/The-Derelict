using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : Interactable
{
    public DoorInteractable linkedDoor;
    public bool hasBattery;

    private void Start()
    {
        hasBattery = false;
    }

    public override void Interact(PlayerInteraction player, Item activeItem)
    {
        if (player.batteryCount > 0 && !hasBattery)
        {
            hasBattery = true;
            player.batteryCount--;

            linkedDoor.poweredOn = true;
        }
    }
}
