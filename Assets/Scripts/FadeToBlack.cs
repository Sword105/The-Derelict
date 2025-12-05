using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{

    [SerializeField] private CanvasGroup canvasGroup; // the deathScreen canvas
    [SerializeField] public float fadeDuration = 5.0f; // time duration in inspedtor
    [SerializeField] Canvas hudCanvas; // reference to HUD canvas

    public void FadeIn() // starts balck to clear
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration));
    }

    public void FadeOut() // fades to black
    {
        Debug.Log("Fading to black...");

        // since the HUD canvas still shows over the death screen, we need to disable it
        hudCanvas.enabled = false;
        
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
        
        // game is still running during and after the fade out
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    private void Start() // called once 
    {
        //FadeOut();
    }

    private void Update() // called every frame
    {
        
    }



}
