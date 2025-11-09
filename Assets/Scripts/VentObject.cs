using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;


public class VentObject : Interactable

{
    // [SerializeField] private GameObject[] connectedVents; //array in case we want multiple connections somehow
    [SerializeField] private GameObject ventEntrace;
    [SerializeField] private GameObject ventExit;
    [SerializeField] private GameObject ventSpawn; //spawn point in front of exit vent
    [SerializeField] private Vector3 ventCenter; //vent center, put down in inspector for ease of testing 
    // this is camera movement for entering vent
    [SerializeField] private float ventRadius; //circle around vent, used for determining curve
    private Vector3 playerDestination;
    
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public override void Interact(PlayerInteraction player)
    {
        //VISUAL SCRIPT

        //camera lerps to the front of the vent
        playerVisual(player);
        //black screen? only see the glow of the multitool?

        //CODE SCRIPT

        if (ventEntrace != null || ventExit != null)
        {
            Debug.Log(player.name + " is interacting with vent " + gameObject.name);

            //determine which vent we're at, send to the other one
            if (gameObject == ventEntrace)
            {
                Teleport(player, ventEntrace, ventSpawn);
            }
            else if (gameObject == ventExit)
            {
                Teleport(player, ventExit, ventSpawn);
            }
        }
        else
        {
            Debug.Log("Cannot interact with vent");
        }

        //set player position to right in front

        //player is immortal while venting -- player health script not done yet
    }

    private void Teleport(GameObject player, GameObject target, GameObject destination)
    {
        player.SetActive(false);
        player.transform.position = destination.transform.position; // + new Vector3(0,0,1) to do: add offset so player doesn't spawn inside vent object
        player.SetActive(true);
        playerVisual(player);
    }

    private Vector3 findPlayerVisualDestination(Camera playerCamera) //purely for visuals
    {

        Vector3 playerMirror = new Vector3( //FIRST: find the mirror coord
            2 * ventCenter.x - playerCamera.transform.position.x, //mirrored coord in relation to ventCenter
            transform.position.y, //same height
            transform.position.z //same depth
        );

        return playerDestination;
    }

    private void playerVisual(PlayerInteraction player)
    {
        Camera playerCamera = player.GetComponentInChildren<Camera>(); //get player's camera 
        
        playerDestination = findPlayerVisualDestination(playerCamera);
        
        // playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, ventCenter, 0.5f);
        
        //camera overcompensates if at a very parallel angle, at a perpendicular angle the camera swiftly reaches its destination 
        //vent opens
        //then slowly enters vent straight ahead, two different lerps
        
        //inside vent animation
    }
}

