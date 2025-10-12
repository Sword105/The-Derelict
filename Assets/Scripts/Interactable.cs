using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(OutlineThing))]
public abstract class Interactable : MonoBehaviour 
{
    void Awake()
    {
        //DEPRECATED - Will delete depending on whether we need this later on or not
        /*
        GetComponent<OutlineThing>().OutlineWidth = 5;
        GetComponent<OutlineThing>().enabled = false;
        GetComponent<OutlineThing>().OutlineMode = OutlineThing.Mode.OutlineVisible;
        */
    }

    //DEPRECATED - Same reason as above
    /*
    public bool UpdateOutline()
    {
        bool isANearestInteractable = false;
        foreach (PlayerInteraction player in PlayerInfo.listOfPlayers())
        {
            if (player.nearestInteractable != null && ReferenceEquals(player.nearestInteractable.gameObject, gameObject))
            {
                isANearestInteractable = true;
                break;
            }
        }

        if (isANearestInteractable)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    

    private void Update()
    {
        UpdateOutline();
    }
    */

    //Placeholder method
    //This is meant to be overridden by subclasses of Interactable
    public virtual void Interact(GameObject player, Transform heldItem)
    {
        //Debug.Log(player.name + " is interacting with object " + item.name);
    }
    
    //This creates a blue box above any interactable object (debug purposes)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position + new Vector3(0, 1, 0), new Vector3(0.2f, 0.2f, 0.2f));
    }
}
