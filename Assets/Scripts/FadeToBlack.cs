using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public Image deathScreenImage; //default image FOR DEATH
    public GameObject deathscreenUI;
    public Image fadeImage; //default image fade to black
    public float fadeDuration = 2f;
    
    private void Start()
    {
        deathscreenUI.SetActive(false);
    }

    public void StartFade() //StartFade by default is for the deathscreen
    {
        StartCoroutine(FadeIn());
    }
    
    public void StartFade(float fadeDuration) //StartFade given a duration is simply a black screen
    {
        StartCoroutine(FadeIn(fadeDuration));
    }

    public void StartFade(Image givenImage, float fadeDuration) //StartFade given image and duration is custom
    {          
        StartCoroutine(FadeIn(givenImage, fadeDuration));
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color currentColor = deathScreenImage.color;

        while (elapsedTime < fadeDuration)
        {
            float currentTime = elapsedTime / fadeDuration;
            currentColor.a = Mathf.Lerp(0f, 1f, currentTime);
            deathScreenImage.color = currentColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the image is fully opaque
        currentColor.a = 1f;
        deathScreenImage.color = currentColor;

        // Show the death UI after fade completes
        deathscreenUI.SetActive(true);
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
            deathScreenImage.color = currentColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the image is fully opaque
        currentColor.a = 1f;
        deathScreenImage.color = currentColor;

        // Show the death UI after fade completes
        deathscreenUI.SetActive(true);
    }

}
