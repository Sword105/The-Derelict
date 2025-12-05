using UnityEngine;
using TMPro;

public class HotbarManager : MonoBehaviour
{
    public InventorySlot[] slots;
    public TextMeshProUGUI itemInHandText;

    private int selectedSlotIndex = -1;

    void Start()
    {
        if (itemInHandText != null) itemInHandText.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
    }

    void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        if (selectedSlotIndex >= 0) slots[selectedSlotIndex].SetSelected(false);
        selectedSlotIndex = index;
        slots[selectedSlotIndex].SetSelected(true);

        UpdateTopText();
    }

    void UpdateTopText()
    {
        if (itemInHandText == null) return;

        InventorySlot currentSlot = slots[selectedSlotIndex];

        if (currentSlot.HasItem())
        {
            itemInHandText.text = currentSlot.displayName;
        }
        else
        {
            itemInHandText.text = "EMPTY";
        }
    }

    public bool AddItemToHud(ItemID newItemID)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.assignedItemType == newItemID)
            {
                slot.UnlockItem();

                if (slots[selectedSlotIndex] == slot) UpdateTopText();

                return true;
            }
        }

        Debug.LogWarning("No slot found assigned to: " + newItemID);
        return false;
    }
}
