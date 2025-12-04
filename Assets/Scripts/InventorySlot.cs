using UnityEngine;
using UnityEngine.UI;
using System.Collections; 
public class InventorySlot : MonoBehaviour
{
    public Image slotBackground;
    public Image iconDisplay;
    public Color selectedColor = Color.white;
    public Color normalColor = new Color(0.3f, 0.3f, 0.3f, 1f); 
    public float waitBeforeFading = 2.0f; 
    public float fadeDuration = 1.0f;   

    private Coroutine activeFadeRoutine;

    void Start()
    {
        if (slotBackground != null)
        {
            slotBackground.color = normalColor;
        }
        
        if (iconDisplay != null)
        {
            var tempColor = iconDisplay.color;
            tempColor.a = 0f;
            iconDisplay.color = tempColor;
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (slotBackground == null) return;

        
        if (activeFadeRoutine != null) StopCoroutine(activeFadeRoutine);

        if (isSelected)
        {
             slotBackground.color = selectedColor;
            
            
            activeFadeRoutine = StartCoroutine(FadeOutRoutine());
        }
        else
        {
            slotBackground.color = normalColor;
        }
    }

    IEnumerator FadeOutRoutine()
    {
        yield return new WaitForSeconds(waitBeforeFading);

        float elapsedTime = 0f;
        Color startColor = slotBackground.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float percentComplete = elapsedTime / fadeDuration;

            slotBackground.color = Color.Lerp(startColor, normalColor, percentComplete);

            yield return null;
        }
        
        slotBackground.color = normalColor;
    }

    public void AddItem(Sprite newItem)
    {
        iconDisplay.sprite = newItem;
        iconDisplay.enabled = true;
        var tempColor = iconDisplay.color;
        tempColor.a = 1f;
        iconDisplay.color = tempColor;
    }
}