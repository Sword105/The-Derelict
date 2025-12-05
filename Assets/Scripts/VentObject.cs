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
    [SerializeField] float ventRadius = 1.0f; //sphere around vent, used for determining visual curve
    [SerializeField] float ventEnterSpeed;
    [SerializeField] Camera playerCamera;
    
    private Vector3 startPoint;
    private Vector3 midPoint;
    private Vector3 endPoint;
    private Vector3 VentCenterWorld => transform.TransformPoint(ventCenter);
    
    private float tCurrent = 0f;
    private float tVelocity = 0f;
    private float tStop;
    
    private bool playerEntering = false;
    private GameObject player;
    
    private Vector3 originalCamLocalPos; //player cam storage
    private Quaternion originalCamLocalRot;
    private int parabolaPhase = 0;
    
    void Update()
    {
        if (playerEntering)
        {
            SmoothDampParabola();
        }
    }
    
    
    public override void Interact(PlayerInteraction playerSource, Item activeItem)

    {
        player = playerSource.gameObject;
        
        //VISUAL SCRIPT
        
        PlayerVisual();
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
    

    private void PlayerVisual()
    {

        originalCamLocalPos = playerCamera.transform.localPosition;
        originalCamLocalRot = playerCamera.transform.localRotation;
        
        bool tooClose = Vector3.Distance(playerCamera.transform.position, VentCenterWorld) < ventRadius;

        
        if (tooClose)
        {
            RestartEnteringVent(); //skip to 2nd parabola
            parabolaPhase = 2;
            playerEntering = true;
            return;
        }
        
            //convert to clear variables for the first parabola
            startPoint = playerCamera.transform.position; 
            midPoint = VentCenterWorld;
            endPoint = GetMirrorPoint();

            tStop = FindIntersection(startPoint, midPoint, endPoint);

            tCurrent = 0f;
            tVelocity = 0f;


            parabolaPhase = 1;
            playerEntering = true;
                    
        
        
        //camera overcompensates if at a very parallel angle, at a perpendicular angle the camera swiftly reaches its destination 
        //vent opens
        //then slowly enters vent straight ahead, two different lerps
        
        //inside vent animation
    }
    

    private void SmoothDampParabola()
    {
        tCurrent = Mathf.SmoothDamp(tCurrent, tStop, ref tVelocity, 0.25f);  //smoothTime = .25

        playerCamera.transform.position = FindParabolaPoint(startPoint, midPoint, endPoint, tCurrent);

        if (Mathf.Abs(tCurrent - tStop) < 0.001f) //when t is close enough to intersection
        {
            tCurrent = tStop;

            if (parabolaPhase == 1)
            {
                RestartEnteringVent(); //then start the 2nd parabola MAYBE ADD DELAY
                parabolaPhase = 2;
            }
            else if (parabolaPhase == 2)
            {
                playerEntering = false;
                parabolaPhase = 0;
                
                playerCamera.transform.localPosition = originalCamLocalPos;
                playerCamera.transform.localRotation = originalCamLocalRot;
            }
            

            
        }
    }

    private void RestartEnteringVent()
    {
        startPoint = playerCamera.transform.position;
        midPoint = VentCenterWorld;
        endPoint = transform.position;

        tStop = 1.0f; //temp value, just ensures it doesnt bug when second parbola starts and immediately kill the 2nd run
        tCurrent = 0f;
        tVelocity = 0f;
    }
    
    private float FindIntersection(Vector3 a, Vector3 b, Vector3 c) //this is absolute shit code but its 7 am and i havent slept so i do not give a single fuck
    {
        Vector3 sphereCenter = VentCenterWorld;
        float sphereRadius = ventRadius;

        const int samples = 200;
        List<float> hits = new List<float>();

        float prev = DistanceToSpherePoint(a, sphereCenter, sphereRadius);

        for (int i = 1; i <= samples; i++)
        {
            float t = i / (float)samples;
            float d = DistanceToSpherePoint(FindParabolaPoint(a, b, c, t), sphereCenter, sphereRadius);

            if (Mathf.Sign(prev) != Mathf.Sign(d))
                hits.Add(t);

            prev = d;
        }

        // exit intersection
        if (hits.Count >= 2)
            return hits[1];

        return 1f;
    }

    private float DistanceToSpherePoint(Vector3 point, Vector3 center, float radius)
    {
        return Vector3.Distance(point, center) - radius;
    }
    private Vector3 GetMirrorPoint() //find the mirror coord
    {
        return new Vector3( 
            2 * VentCenterWorld.x - playerCamera.transform.position.x, //mirrored coord in relation to ventCenter
            transform.position.y, //same height
            transform.position.z //same depth
        ); //now, playerMirror is the parabola endpoint
    }
    
    private Vector3 FindParabolaPoint(Vector3 start, Vector3 mid, Vector3 end, float t) //CHECKMARK
            {
                float oneMinusT = 1f - t; //the idea is you have a normal line and 3 points influence how the line bends
                return oneMinusT * oneMinusT * start //calc the weight of the startpoint
                       + 2f * oneMinusT * t * mid //calc the weight of the midpoint
                       + t * t * end; //calc the weight of the endpoint
                //in the end, you get the given point on the parabola
            }
    
    private void OnDrawGizmos()
    {
        // --- Draw vent center ---
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(VentCenterWorld, 0.1f);

        // --- Draw vent radius ---
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(VentCenterWorld, ventRadius);

        Gizmos.DrawSphere(VentCenterWorld, 0.1f);
        Gizmos.DrawWireSphere(VentCenterWorld, ventRadius);

        // If game is not playing, show vent center + radius only
        if (!Application.isPlaying)
            return;

        // --- Draw current parabola ---
        Gizmos.color = Color.yellow;
        DrawParabolaGizmo(startPoint, midPoint, endPoint);

        // --- Draw intersection point ---
        Gizmos.color = Color.white;
        Vector3 intersection = FindParabolaPoint(startPoint, midPoint, endPoint, tStop);
        Gizmos.DrawSphere(intersection, 0.05f);
    }

// Utility to draw a smooth Gizmo curve
    private void DrawParabolaGizmo(Vector3 a, Vector3 b, Vector3 c)
    {
        const int segments = 50;
        Vector3 prev = a;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = FindParabolaPoint(a, b, c, t);
            Gizmos.DrawLine(prev, point);
            prev = point;
        }
    }
}
