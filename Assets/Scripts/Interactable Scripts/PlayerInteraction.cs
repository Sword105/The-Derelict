using System;
using System.Collections.Generic;
using UnityEngine;
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
    public float interactionRange = 5f;
    public Item activeItem;
    public ItemID heldItemType = ItemID.None;
    private Interactable currentHovered;

    public List<ItemID> inventory = new List<ItemID>();
    public int batteryCount;
    [SerializeField] public Boolean hasMultiTool = false;
    [SerializeField] public Boolean hasFlashlight = false;
    [SerializeField] public Boolean hasTazer = false;
    [SerializeField] public Boolean hasBiotracker = false;
    [SerializeField] public Boolean hasPowerDrill = false;
    
   


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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionRange, interactableLayer))
        {
            objectToInteract = hit.collider.GetComponent<Interactable>();
            
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
            objectToInteract.Interact(player.GetComponent<PlayerInteraction>());

        }
    }
}

