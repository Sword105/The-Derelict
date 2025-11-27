using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image slotBackground;
    public Image iconDisplay;
    public Color selectedColor = Color.white;
    public Color normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    void Start()
    {
        if (iconDisplay != null)
        {
            var tempColor = iconDisplay.color;
            tempColor.a = 0f;
            iconDisplay.color = tempColor;
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (slotBackground != null)
        {
            Debug.Log(gameObject.name + " changing color to " + (isSelected ? "SELECTED" : "NORMAL"));
            slotBackground.color = isSelected ? selectedColor : normalColor;
        }
        else
        {
            Debug.LogError("Slot Background is MISSING on " + gameObject.name);
        }
    }
}