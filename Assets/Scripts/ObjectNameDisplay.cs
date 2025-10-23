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
    private TextMeshProUGUI ObjectIdentityText;

    private Interactable ObjectToInteract;
        

    public class HoveredEvent : UnityEvent<Interactable>
    {
        
    }

    public HoveredEvent ObjectHovered;
    
    
    // to place at end of PlayerInteraction script
    // ObjectHovered.Invoke(objectToInteract);

    
    // Start is called before the first frame update
    void Start()
    {
        Player.ObjectHovered.AddListener(GetObjectToInteract);
    }

    private void GetObjectToInteract(Interactable interactable)
    {
        Debug.Log("called GetObjectToInteract");
        ObjectToInteract = interactable;
    }
    
    // Update is called once per frame
    void Update()
    {
        ObjectIdentityText.text = ObjectToInteract.name;
    }
}
