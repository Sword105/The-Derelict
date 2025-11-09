using System;
using UnityEngine;

// event integration
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour
{
    public class HoveredEvent : UnityEvent<Interactable>
    {
        
    }

    public HoveredEvent ObjectHovered = new HoveredEvent();
    
    //FYI, "NonSerialized" just hides the variable from the inspector

    [NonSerialized] public GameObject player;
    public LayerMask interactableLayer;
    public Item activeItem;
    private Interactable currentHovered;

    //DEPRECATED
    /*
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isInteracting = true;
        }
        //Debug.Log(isInteracting);
    }
    */

    private void Start()
    {
        player = gameObject;
    }

    void Update()
    {
        // Uses a raycast to check if the player is looking at an Interactable object
        // All Interactable objects MUST be on the Interactable layer to work
        RaycastHit hit;
        Interactable objectToInteract = null;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 50.0f, interactableLayer))
        {
            objectToInteract = hit.collider.GetComponent<Interactable>();
            ObjectHovered.Invoke(objectToInteract);
        }
        if (objectToInteract != currentHovered)
        {
            // looks like the raycast isn't hitting anything!!
            {
                currentHovered = objectToInteract;
                ObjectHovered.Invoke(currentHovered);
            }
        }

        // If an Interactable object was found, interact with it when you press the Interact key
        if (Input.GetKeyDown(KeyCode.E) && objectToInteract != null)
        {
            Debug.Log("Interactable detected as " + objectToInteract.name + ", trying interaction");
            objectToInteract.Interact(player, activeItem);
        }
    }
}

