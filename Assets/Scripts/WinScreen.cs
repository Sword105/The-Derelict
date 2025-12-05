using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WinScreen : MonoBehaviour
{

    [SerializeField] private Image winScreen;
    [SerializeField] private GameObject Screen;
    [SerializeField] private GameObject FadeToBlack;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerWinscreen();
        }
    }

    private void TriggerWinscreen()
    {
        //trigger fade to black script
        FadeToBlack.GetComponent<FadeToBlack>().StartFade();
        //image fades in 
    }
    
}
