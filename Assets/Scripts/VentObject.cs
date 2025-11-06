using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class VentObject : Interactable

{
    [SerializeField] private GameObject[] connectedVents; //array in case we want multiple connections somehow
    [SerializeField] private Vector3 ventCenter; //vent center, put down in inspector for ease of testing
    [SerializeField] private float ventRadius; //circle around vent, used for determining curve
    private Vector3 playerDestination;
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
    
    private Vector3 findPlayerDestination(Camera playerCamera)
    {

        Vector3 playerMirror = new Vector3( //FIRST: find the mirror coord
            2 * ventCenter.x - playerCamera.transform.position.x, //mirrored coord in relation to ventCenter
            transform.position.y, //same height
            transform.position.z //same depth
        );


        return playerDestination;
    }

    private void playerVisual(GameObject player)
    {
        Camera playerCamera = player.GetComponent<Camera>(); //get players camera //EDIT: do this with serialize field instead?
        
        playerDestination = findPlayerDestination(playerCamera);
        
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, ventCenter, 0.5f);
        
        //camera overcompensates if at a very parallel angle, at a perpendicular angle the camera swiftly reaches its destination 
        //vent opens
        //then slowly enters vent straight ahead, two different lerps
        
        //inside vent animation
    }
}
*/
