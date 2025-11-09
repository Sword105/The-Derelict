using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// necessary to use events
using UnityEngine.Events;

// necessary to use TextMeshPro object
using TMPro;


public class ObjectNameDisplay : MonoBehaviour
{
    // declarations i can't remember what these do or how to use them
    
    [SerializeField]
    private PlayerInteraction Player;
    
    // assigns object's name i think
    [SerializeField]
    // MAKE SURE this IT'S NOT THE UGUI VERSION the text is a 3d object so it needs to be TextMeshPro
    private TextMeshPro ObjectIdentityText;

    private Interactable ObjectToInteract;
        

    /* I CHANGED THE PlayerInteraction SCRIPT ANYWAY so yeah
     
    public class HoveredEvent : UnityEvent<Interactable>
    {
        
    }

    public HoveredEvent ObjectHovered;
    */
    
    // to place at end of PlayerInteraction script
    // ObjectHovered.Invoke(objectToInteract);

    
    // Start is called before the first frame update
    void Start()
    {
        if (Player == null)
        {
            Debug.LogError("Player is NULL on " + gameObject.name);
            return;
        }

        if (ObjectIdentityText == null)
        {
            Debug.LogError("ObjectIdentityText is NULL on " + gameObject.name);
            return;
        }

        Debug.Log("ObjectNameDisplay.Start subscribing to Player.ObjectHovered");
        Player.ObjectHovered.AddListener(GetObjectToInteract);
    }

    private void GetObjectToInteract(Interactable interactable)
    {
        Debug.Log("called GetObjectToInteract with: " + (interactable!=null ? interactable.name : "NULL"));
        ObjectToInteract = interactable;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (ObjectToInteract == null)
        {
            // Debug.Log("Update(): ObjectToInteract is NULL");
            // return;
            ObjectIdentityText.text = "";
            return;
        }

        if (ObjectIdentityText != null)
        {
            ObjectIdentityText.text = ObjectToInteract.name;
        }
    }
}
