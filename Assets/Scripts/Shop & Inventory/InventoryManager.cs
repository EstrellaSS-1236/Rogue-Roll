using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject InventoryMenu;          // Reference to the inventory menu UI
    public ItemSlot[] itemSlot;               // Array of item slots in the UI

    [Header("Item Data")]
    public ItemSO[] itemSOs;                  // All available item definitions

    private bool menuActivated;               // Tracks whether inventory menu is open
    private Dictionary<string, ItemSO> itemLookup; // Faster lookup for items by name

    private void Awake()
    {
        // Build dictionary for quick item lookups
        itemLookup = new Dictionary<string, ItemSO>();
        foreach (var item in itemSOs)
        {
            if (!itemLookup.ContainsKey(item.itemName))
                itemLookup[item.itemName] = item;
        }
    }

    private void OnEnable()
    {
        // Refresh slots after UI is fully active
        StartCoroutine(RefreshSlotsNextFrame());
    }

    private IEnumerator RefreshSlotsNextFrame()
    {
        yield return null; // Wait one frame so UI is ready

        if (InventoryMenu != null && InventoryMenu.activeSelf)
        {
            foreach (var slot in itemSlot)
            {
                if (slot != null)
                    slot.ForceUpdateUI();
            }
        }
    }

    public void ToggleInventory()
    {
        menuActivated = !menuActivated;
        Debug.Log("ToggleInventory called, menuActivated = " + menuActivated);

        if (InventoryMenu != null)
        {
            InventoryMenu.SetActive(menuActivated);

            if (menuActivated)
                StartCoroutine(RefreshSlotsNextFrame());
        }

        // Pause game when inventory is open
        Time.timeScale = menuActivated ? 0f : 1f;
    }

    public void UseItem(string itemName)
    {
        // Use dictionary for faster lookup
        if (itemLookup.TryGetValue(itemName, out var item))
        {
            item.UseItem();
        }
        else
        {
            Debug.LogWarning($"Item {itemName} not found in inventory.");
        }
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        foreach (var slot in itemSlot)
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

    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlot)
        {
            if (slot.selectedShader != null) slot.selectedShader.SetActive(false);
            slot.thisItemSelected = false;
        }
    }
}
