using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    public InventorySlot[] slots;
    private int selectedSlotIndex = 0;

    void Start()
    {
        SelectSlot(0);
    }

    void Update()
    {
       //Debug.Log("Script is running!");
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
    }

    void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;

        Debug.Log("Manager is trying to select slot: " + index);

        slots[selectedSlotIndex].SetSelected(false);
        selectedSlotIndex = index;
        slots[selectedSlotIndex].SetSelected(true);
        Debug.Log("Selected Slot: " + (index + 1));
    }
}