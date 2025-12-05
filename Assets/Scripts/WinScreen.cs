using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{

    [SerializeField] private Image winScreen;
    [SerializeField] private GameObject FadeToBlack; //reference to fade to black script

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("win triggered");
            TriggerWinscreen();
        }
    }

    private void TriggerWinscreen()
    {
        //trigger fade to black script
        FadeToBlack.GetComponent<FadeToBlack>().StartFade(winScreen, 2f);
        Debug.Log("win fade started");
        //image fades in 
    }
    
}
