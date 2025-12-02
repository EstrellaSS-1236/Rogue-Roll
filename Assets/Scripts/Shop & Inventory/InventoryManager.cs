using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * InventoryManager
 * ----------------
 * Central system for managing the player's inventory.
 * Handles adding/removing items, opening/closing the inventory UI,
 * slot replacement when full, and item usage.
 */
public class InventoryManager : MonoBehaviour
{
    // Singleton instance so other scripts can access InventoryManager easily
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryMenu; // The UI panel for the inventory
    [SerializeField] private ItemSlot[] itemSlots;     // Array of slots shown in the inventory UI

    [Header("Item Data")]
    [SerializeField] private ItemSO[] itemSOs;         // All possible item definitions (ScriptableObjects)

    // Tracks whether the inventory menu is currently open
    private bool menuActivated;

    // Lookup dictionary to quickly find ItemSO by name
    private Dictionary<string, ItemSO> itemLookup;

    // Replacement state when inventory is full
    private bool waitingForReplace;    // True if waiting for the player to replace an item
    private ItemSO pendingItem;        // Item pending replacement
    private int pendingQuantity;       // Quantity of the pending item

    private void Awake()
    {
        // Ensure only one InventoryManager exists (singleton pattern)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Build lookup dictionary from provided ItemSO array
        itemLookup = new Dictionary<string, ItemSO>();
        foreach (var item in itemSOs)
        {
            if (!itemLookup.ContainsKey(item.itemName))
                itemLookup[item.itemName] = item;
        }

        // Hide inventory menu at start
        if (inventoryMenu != null)
            inventoryMenu.SetActive(false);
    }

    private void OnEnable()
    {
        // Refresh slots next frame when enabled
        StartCoroutine(RefreshSlotsNextFrame());
    }

    private IEnumerator RefreshSlotsNextFrame()
    {
        // Wait one frame to ensure UI is ready
        yield return null;

        // If inventory menu is open, refresh all slot UIs
        if (inventoryMenu != null && inventoryMenu.activeSelf)
        {
            foreach (var slot in itemSlots)
                slot?.RefreshUI();
        }
    }

    // Public getter for slots
    public ItemSlot[] ItemSlots => itemSlots;

    // Toggle inventory open/close
    public void ToggleInventory()
    {
        if (menuActivated)
        {
            CloseInventory();
        }
        else
        {
            menuActivated = true;
            inventoryMenu?.SetActive(true);
            StartCoroutine(RefreshSlotsNextFrame());
            Time.timeScale = 0f; // Pause game when inventory is open
        }
    }

    // Explicit open (always opens, does not toggle)
    public void OpenInventory()
    {
        menuActivated = true;
        inventoryMenu?.SetActive(true);
        StartCoroutine(RefreshSlotsNextFrame());
        Time.timeScale = 0f;
    }

    // Explicit close (handles cancel fallback)
    public void CloseInventory()
    {
        menuActivated = false;
        inventoryMenu?.SetActive(false);
        Time.timeScale = 1f; // Resume game when inventory closes

        // If we were waiting for replacement, cancel it
        if (waitingForReplace)
        {
            Debug.Log("Replacement cancelled. Pending item " + pendingItem?.itemName + " discarded.");
            waitingForReplace = false;
            pendingItem = null;
        }
    }

    // Use an item by name (calls ItemSO.UseItem)
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

    // Overload: Add item directly from ItemSO
    public int AddItem(ItemSO item, int quantity)
    {
        return AddItem(item.itemName, quantity, item.icon, item.itemDescription);
    }

    // Add item by name + sprite + description
    // Returns leftover quantity if inventory is full
    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // Try to place item in existing slot or empty slot
        foreach (var slot in itemSlots)
        {
            if ((!slot.isFull && slot.itemName == itemName) || slot.quantity == 0)
            {
                int leftOverItems = slot.AddItem(itemName, quantity, itemSprite, itemDescription);

                // If leftover items remain, try to add them recursively
                if (leftOverItems > 0)
                    return AddItem(itemName, leftOverItems, itemSprite, itemDescription);

                return 0; // All items added successfully
            }
        }

        // Inventory is full
        if (OptionPopupManager.Instance != null)
        {
            // Show popup asking player to replace an item
            OptionPopupManager.Instance.ShowInventoryFullPopup(itemName, quantity, itemSprite, itemDescription);
        }
        else
        {
            // Fallback: open inventory for manual replacement
            Debug.Log("Inventory full. Opening inventory for manual substitution.");
            PrepareReplace(itemName, quantity, itemSprite, itemDescription);
            OpenInventory();
        }

        return quantity; // Return leftover quantity
    }

    // Called when a slot is clicked in the UI
    public void OnSlotClicked(ItemSlot slot)
    {
        if (waitingForReplace)
        {
            ReplaceInSlot(slot);
        }
        else
        {
            slot.SelectSlot();
        }
    }

    // Prepare replacement state when inventory is full
    public void PrepareReplace(string itemName, int quantity, Sprite sprite, string description)
    {
        waitingForReplace = true;
        // Create a temporary ItemSO-like object to hold data
        pendingItem = new ItemSO { itemName = itemName, icon = sprite, itemDescription = description };
        pendingQuantity = quantity;
    }

    public bool IsWaitingForReplace() => waitingForReplace;

    // Replace item in a slot with pending item
    public void ReplaceInSlot(ItemSlot slot)
    {
        if (pendingItem == null) return;

        if (OptionPopupManager.Instance != null)
        {
            // Show confirmation popup before replacing
            OptionPopupManager.Instance.ShowConfirmReplacePopup(slot, () =>
            {
                slot.ClearSlot();
                slot.AddItem(pendingItem.itemName, pendingQuantity, pendingItem.icon, pendingItem.itemDescription);
                waitingForReplace = false;
                pendingItem = null;
                CloseInventory(); // Auto-close after replacement
            });
        }
        else
        {
            // Fallback: replace directly without confirmation
            Debug.Log("Replacing item in slot without popup confirmation.");
            slot.ClearSlot();
            slot.AddItem(pendingItem.itemName, pendingQuantity, pendingItem.icon, pendingItem.itemDescription);
            waitingForReplace = false;
            pendingItem = null;
            CloseInventory();
        }
    }

    // Remove items from inventory
    public void RemoveItem(string itemName, int quantity)
    {
        foreach (var slot in itemSlots)
        {
            if (slot.itemName == itemName && slot.quantity > 0)
            {
                int removeAmount = Mathf.Min(quantity, slot.quantity);
                slot.quantity -= removeAmount;
                quantity -= removeAmount;

                if (slot.quantity <= 0)
                    slot.ClearSlot();
                else
                    slot.RefreshUI();

                if (quantity <= 0) return; // Done removing
            }
        }

        if (quantity > 0)
            Debug.LogWarning("RemoveItem: Could not remove enough of " + itemName);
    }

    // Deselect all slots (clear selection highlight)
    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            slot.selectedShader?.SetActive(false);
            slot.thisItemSelected = false;
        }
    }

    //Get ItemSO by name
    public ItemSO GetItemSO(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return null;
        if (itemLookup == null) return null;

        ItemSO result;
        return itemLookup.TryGetValue(itemName, out result) ? result : null;
    }
}
