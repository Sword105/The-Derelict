using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{
    public Image deathScreenImage;
    public GameObject deathscreenUI;
    public float fadeDuration = 2f;

    private void Start()
    {
        Color c = deathScreenImage.color;
        c.a = 0f;
        deathScreenImage.color = c;

        // Hide the death UI unntil fade completes
        deathscreenUI.SetActive(false);
    }

    public void StartFade()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color c = deathScreenImage.color;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            c.a = Mathf.Lerp(0f, 1f, t);
            deathScreenImage.color = c;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the image is fully opaque
        c.a = 1f;
        deathScreenImage.color = c;

        // Show the death UI after fade completes
        deathscreenUI.SetActive(true);
    }

}
