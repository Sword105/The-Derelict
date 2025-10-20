using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{

    // Movement variables
    public float moveSpeed;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Rigidbody rb;

    // Ground check variables
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public float groundDrag;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {

        // handles horizontal and vertical input
        MyInput();

        SpeedControl();


        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, (float)(playerHeight * 0.5 + 0.2f), whatIsGround);// declare what grounded is by raycasting downwards from player position
        
        if (grounded) // check if the player is grounded
        {
            Debug.Log("Grounded Sucessfully");
            
            if (horizontalInput == 0 && verticalInput == 0) // now we check if the user let go of the movement keys then we apply drag to prevent sliding
                rb.drag = groundDrag * 10;
            else
                rb.drag = 0; // airborne movement
        }

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    
    // movement methods

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // input from the user
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    public void MovePlayer()
    {
        // calculate movement direction
        Vector3 MoveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(MoveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed; // max velocity would be
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); // applying limited velocity to player speed
        }
    }
}
