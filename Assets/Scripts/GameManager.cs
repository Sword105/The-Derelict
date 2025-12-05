using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public FadeToBlack fadeToBlackScript;

    [Header("Button Variables")]

    public Button mainMenuButton;

    
    void Start()
    {

        fadeToBlackScript = FindObjectOfType<FadeToBlack>();// get reference to fade to black script

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

    public void goMainMenu()
    {
        //TODO: fix this 
        Debug.Log("Main Menu button pressed.");
        
        Time.timeScale = 1f; // ensure time scale is normal

        SceneManager.LoadScene("StartMenu");

        Debug.Log("Loading StartMenu scene...");
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
        
        StartCoroutine(PauseGameAfterDelay());
        fadeToBlackScript.FadeOut(); // start the fade


        // the fade is still going but we need to pause the game once completed
        // thats why we have the public PausedGameAfter()

        

    }

    public void PauseGameAfter()
    {
        StartCoroutine(PauseGameAfterDelay());
    }

    private IEnumerator PauseGameAfterDelay()
    {

       float delay = fadeToBlackScript.fadeDuration;

        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 0f;
        Debug.Log("Game Paused!");
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
