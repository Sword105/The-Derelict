using UnityEngine;
using TMPro;
using System.Collections.Generic; 

public class HotbarManager : MonoBehaviour
{
    public InventorySlot[] slots;
    public TextMeshProUGUI itemInHandText;

    [System.Serializable]
    public struct ItemDefinition
    {
        public ItemID id;        
        public string displayName; 
        public Sprite icon;      
    }

    public List<ItemDefinition> allItemDefinitions; 

    private ItemID[] inventoryContent;
    private int selectedSlotIndex = -1;

    void Start()
    {
        inventoryContent = new ItemID[slots.Length];
        if(itemInHandText != null) itemInHandText.text = "";
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

        UpdateItemInHandText();
    }

    void UpdateItemInHandText()
    {
        if (itemInHandText == null) return;

        ItemID currentID = inventoryContent[selectedSlotIndex];
        ItemDefinition def = GetDefinition(currentID);
        
        if (def.id != ItemID.None)
        {
            itemInHandText.text = def.displayName;
        }
        else
        {
            itemInHandText.text = "";
        }
    }

    ItemDefinition GetDefinition(ItemID id)
    {
        foreach (var item in allItemDefinitions)
        {
            if (item.id == id) return item;
        }
        return new ItemDefinition(); 
    }

    public bool AddItemToHud(ItemID newItemID)
    {
        ItemDefinition def = GetDefinition(newItemID);
        if (def.id == ItemID.None) 
        {
            Debug.LogError("HotbarManager: No definition found for " + newItemID);
            return false;
        }

        for (int i = 0; i < inventoryContent.Length; i++)
        {
            if (inventoryContent[i] == ItemID.None) 
            {
                inventoryContent[i] = newItemID;
                slots[i].AddItem(def.icon); 

                if (i == selectedSlotIndex) UpdateItemInHandText();
                return true;
            }
        }
        return false;
    }
}