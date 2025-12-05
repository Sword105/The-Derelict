using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Interactable
{
    //The Variables to the Generator/Batteries check System
    public int requiredBatteries = 3; //Total batteries needed, can change anytime
    public float chargeTime = 5f; //How long the player must hold to fully charge
    public GameObject[] linkedObjects; // Objects to unlock once powered

    private int insertedBatteries = 0;
    private bool isCharging = false;
    private bool isUnlocked = false;
    private float currentCharge = 0f;

    public override void Interact(PlayerInteraction player, Item activeItem)
    {
        // Stop if already powered
        if (isUnlocked)
        {
            Debug.Log("Generator already powered!");
            return;
        }

        PlayerInteraction playerInteraction = player.GetComponent<PlayerInteraction>();
        // 1. Insert a battery if player has one
        if (playerInteraction.heldItemType == ItemID.BATTERY && insertedBatteries < requiredBatteries)
        {
            insertedBatteries++;
            Debug.Log($"Battery inserted: {insertedBatteries}/{requiredBatteries}");

            // Remove battery from player inventory (placeholder)
            playerInteraction.activeItem = null;
            //Not enough Batteries
            if (insertedBatteries < requiredBatteries)
            {
                Debug.Log($"You still need {requiredBatteries - insertedBatteries} more batteries");
                return;
            }

            // All Batteries are in
            if (insertedBatteries == requiredBatteries)
            {
                Debug.Log("All batteries inserted! Hold E to charge the generator.");
                return;
            }

        }

        // 2. Start charging if all batteries are inserted
        if (insertedBatteries == requiredBatteries && !isUnlocked)
        {
            if (!isCharging)
            {
                player.GetComponent<MonoBehaviour>().StartCoroutine(ChargeGenerator());
            }
            return;
        }

        // 3. Otherwise, player still needs more batteries

        //Debug.Log("You need more batteries to power this generator!");
    }

    private IEnumerator ChargeGenerator()
    {
        if (isCharging) yield break;

        isCharging = true;
        currentCharge = 0f;
        Debug.Log("Charging generator... Hold E to continue.");

        while (currentCharge < chargeTime)
        {
            if (!Input.GetKey(KeyCode.E))
            {
                Debug.Log("Charging interrupted.");
                isCharging = false;
                yield break;
            }

            currentCharge += Time.deltaTime;
            Debug.Log($"Chargin {currentCharge:F2}/{chargeTime}");
            yield return null;
        }

        Debug.Log("Generator fully charged!");
        UnlockLinkedObjects();
        isCharging = false;
    }

    private void UnlockLinkedObjects()
    {
        isUnlocked = true;

        foreach (GameObject obj in linkedObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        Debug.Log("Generator powered! Linked systems activated.");
    }
}
