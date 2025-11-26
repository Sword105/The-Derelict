using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepContoller : MonoBehaviour
{
    // Start is called before the first frame update

     // Audio variables
    public AudioSource audioSource; // Audio source component to play sounds
    public AudioClip[] footstepClips; // Array of footstep sounds
    public float stepInterval = 0.5f; // Time between each footstep sound

    // Movement variables
    private float stepTimer = 0f; // Timer to track time between steps
    private Vector3 lastPosition; // To track the last position of the player
    void Start()
    {
        if (audioSource == null) // If no audio source assigned, try to get one from the GameObject
        {
            audioSource = GetComponent<AudioSource>();
        }

        lastPosition = transform.position; // Initialize last position
    
    }

    // Update is called once per frame
    void Update()
    {
        // How far the player has moved this frame
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        // Consider the player moving if they moved more than a tiny amount
        bool isMoving = distanceMoved > 0.001f;

        if (isMoving)
        {
            stepTimer += Time.deltaTime; // if the player is moving, increment the timer

            if (stepTimer >= stepInterval) // if the timer exceeds the step interval, play a footstep sound
            {
                PlayRandomFootstep();
                stepTimer = 0f; // reset timer after playing sound
            }
        }
        else
        {
            // Reset timer if not moving
            stepTimer = 0f;
        }

        lastPosition = transform.position; // Update last position
        
        

    }

    void PlayRandomFootstep()
    {

        Debug.Log("Footstep played!"); // console check
        
        if (footstepClips == null || footstepClips.Length == 0) return;
        if (audioSource == null) return;

        int index = Random.Range(0, footstepClips.Length);
        audioSource.pitch = Random.Range(0.95f, 1.05f); // Slight random pitch
        audioSource.PlayOneShot(footstepClips[index]);
    }
    

}
