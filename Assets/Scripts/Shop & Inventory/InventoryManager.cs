using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private ItemSlot[] itemSlots;

    [Header("Item Data")]
    [SerializeField] private ItemSO[] itemSOs;

    private bool menuActivated;
    private Dictionary<string, ItemSO> itemLookup;

    /* Pending replacement state */
    private bool waitingForReplace;
    private string pendingItemName;
    private int pendingQuantity;
    private Sprite pendingSprite;
    private string pendingDescription;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        itemLookup = new Dictionary<string, ItemSO>();
        foreach (var item in itemSOs)
        {
            if (!itemLookup.ContainsKey(item.itemName))
                itemLookup[item.itemName] = item;
        }

        if (inventoryMenu != null)
            inventoryMenu.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(RefreshSlotsNextFrame());
    }

    private IEnumerator RefreshSlotsNextFrame()
    {
        yield return null;
        if (inventoryMenu != null && inventoryMenu.activeSelf)
        {
            foreach (var slot in itemSlots)
                slot?.RefreshUI();
        }
    }

    public ItemSlot[] ItemSlots => itemSlots;

    public void ToggleInventory()
    {
        menuActivated = !menuActivated;
        inventoryMenu?.SetActive(menuActivated);

        if (menuActivated)
            StartCoroutine(RefreshSlotsNextFrame());
        else
            DeselectAllSlots();

        Time.timeScale = menuActivated ? 0f : 1f;
    }

    // Explicit open (always opens, doesn’t toggle)
    public void OpenInventory()
    {
        menuActivated = true;
        inventoryMenu?.SetActive(true);
        StartCoroutine(RefreshSlotsNextFrame());
        Time.timeScale = 0f;
    }

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

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        foreach (var slot in itemSlots)
        {
            if ((!slot.isFull && slot.itemName == itemName) || slot.quantity == 0)
            {
                int leftOverItems = slot.AddItem(itemName, quantity, itemSprite, itemDescription);
                if (leftOverItems > 0)
                    return AddItem(itemName, leftOverItems, itemSprite, itemDescription);
                return 0;
            }
        }

        OptionPopupManager.Instance.ShowInventoryFullPopup(itemName, quantity, itemSprite, itemDescription);
        return quantity;
    }

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

    public void PrepareReplace(string itemName, int quantity, Sprite sprite, string description)
    {
        waitingForReplace = true;
        pendingItemName = itemName;
        pendingQuantity = quantity;
        pendingSprite = sprite;
        pendingDescription = description;
    }

    public bool IsWaitingForReplace() => waitingForReplace;

    public void ReplaceInSlot(ItemSlot slot)
    {
        // Show confirmation popup before replacing
        OptionPopupManager.Instance.ShowConfirmReplacePopup(slot, () =>
        {
            slot.ClearSlot();
            slot.AddItem(pendingItemName, pendingQuantity, pendingSprite, pendingDescription);
            waitingForReplace = false;
        });
    }

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

                if (quantity <= 0) return;
            }
        }

        if (quantity > 0)
            Debug.LogWarning("RemoveItem: Could not remove enough of " + itemName);
    }

    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            slot.selectedShader?.SetActive(false);
            slot.thisItemSelected = false;
        }
    }
}
