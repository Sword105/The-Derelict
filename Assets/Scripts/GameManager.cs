using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject alienPrefab;
    public Transform alienSpawnLocation;

    [Header("DEBUG")]
    public double timeElapsed;
    public bool instantStart = false;
    public bool autoSpawnAlien = false;
    public GameManager instance;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (instantStart) { StartGame(); }
        if (autoSpawnAlien) { SpawnAlien(); }
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
    }

    void StartGame() 
    { 
        Debug.Log("Starting game..."); 
    }

    void SpawnAlien()
    {
        Instantiate(alienPrefab, alienSpawnLocation);
    }

    void WinGame() 
    {
        Debug.Log("You won!");  
    }
}
