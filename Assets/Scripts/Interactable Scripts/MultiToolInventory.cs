using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MultiToolInventory : MonoBehaviour
{
    public PlayerInteraction playerInteraction;


    void Start()
    {
        // Automatically find the PlayerInteraction script in the scene
        playerInteraction = FindObjectOfType<PlayerInteraction>();

        if (playerInteraction == null)
            Debug.LogError("PlayerInteraction not found in scene!");
    }

    void Update()
    {
        if (playerInteraction == null)
            return;

        if (playerInteraction.hasMultiTool)
        {
            
        }


    }

    }
