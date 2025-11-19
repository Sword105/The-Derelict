using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{

    

    public float bobSpeed = 6f; // How fast the bob oscillates
    public float bobAmount = 0.05f; // How far the camera moves up/down
    public float smooth = 8f; // How smoothly it returns to idle

    private float timer = 0f;
    private Vector3 startPos;

    

    void Start()
    {
        // svae the starting position of the camera
        startPos = transform.localPosition;
    }


    // Update is called once per frame
    void Update()
    {
        // Check if player is pressing any movement keys (WASD)
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (isMoving)
        {
            // Animate bobbing using a sine wave
            timer += Time.deltaTime * bobSpeed;
            float newY = startPos.y + Mathf.Sin(timer) * bobAmount;
            transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
        }
        else
        {
            // Smoothly return camera to its resting position
            timer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * smooth);
        }
       
    }    
}
