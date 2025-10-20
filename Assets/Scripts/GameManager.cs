using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public double timeElapsed;
    [Header("DEBUG")]
    public bool instantStart = false;
    public bool autoSpawnAlien = false;

    void Update()
    {
        timeElapsed += Time.deltaTime;
        Debug.Log(timeElapsed);
    }

    void Start()
    {
        if (instantStart) { StartGame(); }
        if (autoSpawnAlien) { SpawnAlien(); }
    }

    void StartGame() { Debug.Log("Starting game..."); }
    void SpawnAlien() { Debug.Log("Spawning alien...");  }
    void WinGame() { Debug.Log("You won!");  }
}
