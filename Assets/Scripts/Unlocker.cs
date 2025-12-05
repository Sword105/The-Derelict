using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlocker : Interactable
{
    public DoorInteractable linkedDoor;

    public override void Interact(PlayerInteraction player)
    {
        linkedDoor.poweredOn = true;
    }
}
