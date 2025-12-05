using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("Settings")]
    public ItemID assignedItemType;
    public string displayName;

    [Header("References")]
    public Image slotBackground;
    public TextMeshProUGUI labelText;

    [Header("Colors")]
    public Color selectedBgColor = new Color(1f, 1f, 1f, 0.2f);
    public Color normalBgColor = new Color(0f, 0f, 0f, 0.5f);

    public Color ownedTextColor = Color.white;
    public Color unownedTextColor = Color.gray;
    public float unownedAlpha = 0.3f;

    private bool isOwned = false;

    void Start()
    {
        if (labelText != null)
        {
            labelText.text = displayName;
            labelText.color = unownedTextColor;
            labelText.alpha = unownedAlpha;
        }

        if (slotBackground != null) slotBackground.color = normalBgColor;
    }

    public void UnlockItem()
    {
        isOwned = true;
        if (labelText != null)
        {
            labelText.color = ownedTextColor;
            labelText.alpha = 1f;
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (slotBackground != null)
        {
            slotBackground.color = isSelected ? selectedBgColor : normalBgColor;
        }
    }

    public bool HasItem()
    {
        return isOwned;
    }
}
