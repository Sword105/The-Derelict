using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Interactable
{
    //The Variables to the Generator/Batteries check System
    public int requiredbatteries = 3; //Total batteries needed, can change anytime
    public float chargeTime = 5f; //How long the player must hold to fully charge
    public GameObject[] linkedObjects; // Objects to unlock once powered

    private int insertedBatteries = 0;
    private bool isChrging = false;
    private bool isUnlocked = false;
    private float currentCharge = 0f;

    public override void Interact(GameObject player, Item activeItem)
    {
        // Prevent further interaction once powered
        if (isUnlocked) return; 

        //1. Inserting the Battery
        //if (activeItem is Battery && insertedBatteries < requiredbatteries)
        {
            insertedBatteries++;
            Debug.Log("Battery Inserted:" + insertedBatteries + "/" + requiredbatteries);

            //Remove The Battery from the Player's Inventory

            // The Battery is fully Inserted
            if (insertedBatteries == requiredbatteries)
            {
                Debug.Log("All batteries inserted! Hold E to charge the generator.");
            }
            return;
        }

        // 2. All Batteries in, The batteries are starting to charge
        if(insertedBatteries == requiredbatteries && !isUnlocked)
        {
            player.GetComponent<MonoBehaviour>().StartCoroutine(ChargeGenerator(player));
            return;
        }

        // 3. Not Enough Batteries
        Debug.Log("MORE BATTERIES!!");
    }

    private IEnumerator ChargeGenerator(GameObject player)
    {
        if (isChrging) yield break;

        isChrging = true;
        currentCharge = 0f;

        Debug.Log("Charging generator... Hold E to continue"); 

        while (currentCharge < chargeTime)
        {
            //Stops if the Player lets go of the button
            if (!Input.GetKey(KeyCode.E))
            {
                Debug.Log("Charging Interrupted.");
                isChrging = false;
                yield break;
            }

            currentCharge += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Generator Fully Charged");
        UnlockedLinkedObjects();
        isChrging = false;
    }

    private void UnlockedLinkedObjects()
    {
        isUnlocked = true;

        foreach (GameObject obj in linkedObjects)
        {
           // obj.setActive(true);
        }

        Debug.Log("Generator has unlocked linked systems!");
    }
}
