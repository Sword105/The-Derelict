using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    //FYI, "NonSerialized" just hides the variable from the inspector

    [NonSerialized] public GameObject player;
    public LayerMask interactableLayer;
    

    public List<ItemID> inventory = new List<ItemID>();
    [SerializeField] public Boolean hasMultiTool = false;


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
            
        }

        // If an Interactable object was found, interact with it when you press the Interact key
        if (Input.GetKeyDown(KeyCode.E) && objectToInteract != null)
        {
            Debug.Log("Interactable detected as " + objectToInteract.name + ", trying interaction");
            objectToInteract.Interact(player.GetComponent<PlayerInteraction>());

            if(objectToInteract.TryGetComponent<Item>(out Item item))
            {
                Debug.Log("TRYING TO DESTROY ITEM");
                Destroy(objectToInteract.gameObject);
                Debug.Log("Current ItemID added to inv: " + inventory[(inventory.Count - 1)]);
            }
            
        }
    }
}

