using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; } // Singleton instance

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryMenu;   // Reference to the inventory menu panel (drag from Canvas)
    [SerializeField] private ItemSlot[] itemSlots;       // Array of item slots in the UI (drag all slots here)

    [Header("Item Data")]
    [SerializeField] private ItemSO[] itemSOs;           // All available item definitions (ScriptableObjects)

    private bool menuActivated;                          // Tracks whether inventory menu is open
    private Dictionary<string, ItemSO> itemLookup;       // Faster lookup for items by name

    private void Awake()
    {
        // Ensure only one instance exists (singleton pattern)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Build dictionary for quick item lookups
        itemLookup = new Dictionary<string, ItemSO>();
        foreach (var item in itemSOs)
        {
            if (!itemLookup.ContainsKey(item.itemName))
                itemLookup[item.itemName] = item;
        }

        // Make sure inventory starts hidden
        if (inventoryMenu != null)
            inventoryMenu.SetActive(false);
    }

    private void OnEnable()
    {
        // Refresh slots after UI is fully active
        StartCoroutine(RefreshSlotsNextFrame());
    }

    private IEnumerator RefreshSlotsNextFrame()
    {
        yield return null; // Wait one frame so UI is ready

        if (inventoryMenu != null && inventoryMenu.activeSelf)
        {
            foreach (var slot in itemSlots)
            {
                if (slot != null)
                    slot.ForceUpdateUI(); // Force slot to redraw its contents
            }
        }
    }

    // Toggles the inventory menu on/off.
    // Also pauses the game when inventory is open.
    public void ToggleInventory()
    {
        menuActivated = !menuActivated;
        Debug.Log("ToggleInventory called, menuActivated = " + menuActivated);

        if (inventoryMenu != null)
        {
            inventoryMenu.SetActive(menuActivated);

            if (menuActivated)
            {
                // Refresh slots when opening
                StartCoroutine(RefreshSlotsNextFrame());
            }
            else
            {
                // Deselect all slots when closing
                DeselectAllSlots();
            }
        }

        // Pause game when inventory is open
        Time.timeScale = menuActivated ? 0f : 1f;
    }


    // Uses an item by name, calling its ScriptableObject logic.
    public void UseItem(string itemName)
    {
        if (itemLookup.TryGetValue(itemName, out var item))
        {
            item.UseItem();
        }
        else
        {
            Debug.LogWarning("Item " + itemName + " not found in inventory.");
        }
    }

    // Adds an item to the inventory slots.
    // Returns leftover quantity if slots are full.
    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        foreach (var slot in itemSlots)
        {
            // Add to slot if empty or matches same item
            if ((!slot.isFull && slot.itemName == itemName) || slot.quantity == 0)
            {
                int leftOverItems = slot.AddItem(itemName, quantity, itemSprite, itemDescription);

                if (leftOverItems > 0)
                    return AddItem(itemName, leftOverItems, itemSprite, itemDescription);

                return 0; // All items added successfully
            }
        }

        return quantity; // Return leftover if no slots available
    }

    // Removes a quantity of an item from inventory slots.
    public void RemoveItem(string itemName, int quantity)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemName == itemName && slot.quantity > 0)
            {
                slot.quantity -= quantity;
                if (slot.quantity <= 0)
                {
                    slot.ClearSlot(); // Custom method in ItemSlot to reset UI and data
                }
                else
                {
                    slot.ForceUpdateUI(); // Refresh UI with new quantity
                }
                return; // Stop after removing from one slot
            }
        }

        Debug.LogWarning("RemoveItem: Item " + itemName + " not found or quantity already 0.");
    }

    // Deselects all slots (removes highlight/selection).
    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            if (slot.selectedShader != null)
                slot.selectedShader.SetActive(false);

            slot.thisItemSelected = false;
        }
    }
}
