using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentObjectduh : Interactable
{
    [SerializeField] private GameObject[] connectedVents; //array in case we want multiple connections somehow
    [SerializeField] private Vector3 ventCenter; //vent center, put down in inspector for ease of testing
    [SerializeField] GameObject ventRadius; //sphere around vent, used for determining visual curve
    [SerializeField] float ventEnterSpeed;
    private Vector3 playerVisualDestination;
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
    
    private Vector3 findPlayerVisualDestination(Camera playerCamera)
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
        
        playerVisualDestination = Vector3.zero;
        

        Vector3 sphereCenter = ventRadius.transform.position;
        float sphereRadius = ventRadius.transform.localScale.x / 2f;

        List<float> intersectionTs = new List<float>();
        const int samples = 1000;

        float prevDist = Vector3.Distance(findParabolaPoint(startPoint, midPoint, endPoint, 0f), sphereCenter) - sphereRadius;

        for (int i = 1; i <= samples; i++)
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
            playerVisualDestination = findParabolaPoint(startPoint, midPoint, endPoint, intersectionTs[1]);
        }
        
        //plug in start mid and end
        //findParabolaPoint(playerCamera.transform.position, playerMirror, playerMirror, );
        
        
        
        
        return playerVisualDestination;
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
        Camera playerCamera = player.GetComponent<Camera>(); //get players camera //EDIT: do this with serialize field instead?
        
        playerVisualDestination = findPlayerVisualDestination(playerCamera);
        
        //NOT BELOW; instead, bezier curve 
       // playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, playerVisualDestination, ventEnterSpeed);
        
        //camera overcompensates if at a very parallel angle, at a perpendicular angle the camera swiftly reaches its destination 
        //vent opens
        //then slowly enters vent straight ahead, two different lerps
        
        //inside vent animation
    }
}
