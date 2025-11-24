using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject alienPrefab;
    public GameObject suspiciousNodePrefab;
    public Transform alienSpawnLocation;

    [Header("DEBUG")]
    public double timeElapsed;
    public bool instantStart = false;
    public bool autoSpawnAlien = false;
    public static GameManager instance;

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
        Instantiate(alienPrefab, alienSpawnLocation.position, Quaternion.identity);
    }

    void WinGame() 
    {
        Debug.Log("You won!");  
    }

    public void SpawnSuspiciousNode(Vector3 position)
    {
        Instantiate(suspiciousNodePrefab, position, Quaternion.identity);
    }

    public void SpawnSuspiciousNode(Vector3 position, Transform impliedObject)
    {
        SuspiciousNodeData newSusNode = Instantiate(suspiciousNodePrefab, position, Quaternion.identity).GetComponent<SuspiciousNodeData>();
        newSusNode.impliedObject = impliedObject;
    }
}
