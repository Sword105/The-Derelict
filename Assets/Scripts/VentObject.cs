using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;


public class VentObject : Interactable

{
    
    // [SerializeField] private GameObject[] connectedVents; //array in case we want multiple connections somehow
    [SerializeField] private GameObject ventEntrance;
    [SerializeField] private GameObject ventExit;
    [SerializeField] private GameObject ventSpawn; //spawn point in front of exit vent
    [SerializeField] private Vector3 ventCenter; //vent center, put down in inspector for ease of testing 
    [SerializeField] GameObject ventRadius; //sphere around vent, used for determining visual curve
    [SerializeField] float ventEnterSpeed;
    [SerializeField] Camera playerCamera;
    private Vector3 startPoint;
    private Vector3 midPoint;
    private Vector3 endPoint;
    private Vector3 parabolaStart;
    private Vector3 parabolaControl;
    private Vector3 parabolaEnd;
    private float tCurrent = 0f;
    private bool playerEntering = false;
    private bool isMovingAlongParabola = true;
    private float playerVisualDestination = 1f;
    private GameObject player;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (playerEntering)
        {
            //lerp towards parabola
            tCurrent += Time.deltaTime * ventEnterSpeed; //point based off time
            float tSmooth = 1f - Mathf.Pow(1f - tCurrent, 2f); //smooths it out 

            // clamp to stop exactly at intersection
            if (tSmooth >= playerVisualDestination)
            {
                tSmooth = playerVisualDestination;
                isMovingAlongParabola = false;
            }

            playerCamera.transform.position = findParabolaPoint(parabolaStart, parabolaControl, parabolaEnd, tSmooth);
            //if (playerHasEntered //WHEN IT REACHES THE MIDPOINT, LERP INTO VENT)
            //{
            // playerEntering = false;
        }
    }
    
    
    public override void Interact(PlayerInteraction playerSource)

    {
        player = playerSource.gameObject;
        
        //VISUAL SCRIPT

        //camera lerps to the front of the vent
        playerVisual(player);
        //black screen? only see the glow of the multitool?

        //CODE SCRIPT

        if (ventEntrance != null || ventExit != null)
        {
            Debug.Log(player.name + " is interacting with vent " + gameObject.name);

            //determine which vent we're at, send to the other one
            if (gameObject == ventEntrance)
            {
                Teleport(player, ventEntrance, ventSpawn);
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

        //player is immortal while venting -- player health script not done yet //DOESNT MATTER, PLAYERS OVER THERE 
    }

    private void Teleport(GameObject player, GameObject target, GameObject destination)
    {
        player.SetActive(false);
        player.transform.position = destination.transform.position; // + new Vector3(0,0,1) to do: add offset so player doesn't spawn inside vent object
        player.SetActive(true);
    }

    private void findPlayerVisualDestination()
    {
        
        Vector3 sphereCenter = ventRadius.transform.position;
        float sphereRadius = ventRadius.transform.localScale.x / 2f;

        List<float> intersectionTs = new List<float>();
        const int samples = 1000; //create a list of ts to test out

        float prevDist = Vector3.Distance(findParabolaPoint(startPoint, midPoint, endPoint, 0f), sphereCenter) - sphereRadius;

        for (int i = 1; i <= samples; i++) //then loop through all samples, finding which t is the closest to intersection
        {
            float t = i / (float)samples;
            float dist = Vector3.Distance(findParabolaPoint(startPoint, midPoint, endPoint, t), sphereCenter) - sphereRadius;

            if (Mathf.Sign(prevDist) != Mathf.Sign(dist))
            {
                float interp = dist / (dist - prevDist);
                float intersectionT = t - interp * (1f / samples);
                intersectionTs.Add(intersectionT);
            }

            prevDist = dist;
        }

        intersectionTs.Sort();

        if (intersectionTs.Count > 1)
        {
            playerVisualDestination = intersectionTs[1];
        }
        else
        {
            playerVisualDestination = 1f;
        }
    }
    
    private Vector3 findParabolaPoint(Vector3 start, Vector3 mid, Vector3 end, float t)
    {
        float oneMinusT = 1f - t; //the idea is you have a normal line and 3 points influence how the line bends
        return oneMinusT * oneMinusT * start //calc the weight of the startpoint
               + 2f * oneMinusT * t * mid //calc the weight of the midpoint
               + t * t * end; //calc the weight of the endpoint
        //in the end, you get the given point on the parabola
    }

    private void playerVisual(GameObject player)
    {
        
        
        //FIRST: Find the endpoint
        Vector3 playerMirror = new Vector3( //find the mirror coord
            2 * ventCenter.x - playerCamera.transform.position.x, //mirrored coord in relation to ventCenter
            transform.position.y, //same height
            transform.position.z //same depth
            ); //now, playerMirror is the parabola endpoint
        
        Vector3 startPoint = playerCamera.transform.position;
        Vector3 midPoint = ventCenter;
        Vector3 endPoint = playerMirror;
        
        parabolaStart = startPoint;
        parabolaControl = midPoint;
        parabolaEnd = endPoint;

        
        findPlayerVisualDestination();
        
        
        //NOW: bezier curve 
        // playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, ventCenter, 0.5f);
        
        //camera overcompensates if at a very parallel angle, at a perpendicular angle the camera swiftly reaches its destination 
        //vent opens
        //then slowly enters vent straight ahead, two different lerps
        
        //inside vent animation
    }
    
    
    
    
}
