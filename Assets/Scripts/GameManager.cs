using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Death varibles")]
    public GameObject fadeToBlackScript;

    
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

    public void HandlePlayerDeath(GameObject player)
    {
        Debug.Log("Player has died."); // logs death to console
        Cursor.lockState = CursorLockMode.Confined; // locks cursor
       
        fadeToBlackScript.GetComponent<FadeToBlack>().StartFade();
        
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
