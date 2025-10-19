using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentObject : Interactable
{
    [SerializeField] private GameObject[] connectedVents; //array in case we want multiple connections somehow
    [SerializeField] private Vector3 ventCenter;
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public override void Interact(GameObject player, Item activeItem)
    {
        //VISUAL SCRIPT
        //camera lerps to the front of the vent
        playerVisual(player);
        //camera rotates, shimmeys through vent
        //black screen? only see the glow of the multitool?

        //CODE SCRIPT
        //set player position to right in front

        //player is immortal while venting
    }

    private void playerVisual(GameObject player)
    {
        Camera PlayerCamera = player.GetComponent<Camera>(); //get players camera
        
        
        //camera overcompensates if at a very parallel angle, at a perpendicular angle the camera swiftly reaches its destination 
        //vent opens
        //then slowly enters vent straight ahead, two different lerps
    }
}
