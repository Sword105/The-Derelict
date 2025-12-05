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
    
    public void StartFade(float fadeDuration) //StartFade given a duration is simply a black screen
    {
        StartCoroutine(FadeIn(fadeDuration));
    }

    public void StartFade(Image givenImage, float fadeDuration) //StartFade given image and duration is custom
    {          
        StartCoroutine(FadeIn(givenImage, fadeDuration));
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
    
    private IEnumerator FadeIn(float fadeDuration)
    {
        float elapsedTime = 0f;
        Color currentColor = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            float currentTime = elapsedTime / fadeDuration;
            currentColor.a = Mathf.Lerp(0f, 1f, currentTime);
            fadeImage.color = currentColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the image is fully opaque
        currentColor.a = 1f;
        fadeImage.color = currentColor;
        
    }

    private IEnumerator FadeIn(Image givenImage, float fadeDuration)
    {
        float elapsedTime = 0f;
        Color currentColor = givenImage.color;

        while (elapsedTime < fadeDuration)
        {
            float currentTime = elapsedTime / fadeDuration;
            currentColor.a = Mathf.Lerp(0f, 1f, currentTime);
            givenImage.color = currentColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the image is fully opaque
        currentColor.a = 1f;
        deathScreenImage.color = currentColor;
    }

    private void Start() // called once 
    {
        //FadeOut();
    }

    private void Update() // called every frame
    {
        
    }



}
