using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory UI")]
    public GameObject InventoryMenu;
    public ItemSlot[] itemSlot;

    [Header("Item Data")]
    public ItemSO[] itemSOs;

    private bool menuActivated;

    private void OnEnable()
    {
        StartCoroutine(RefreshSlotsNextFrame());
    }

    private IEnumerator RefreshSlotsNextFrame()
    {
        yield return null; // Wait one frame so UI is fully active

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

        Time.timeScale = menuActivated ? 0f : 1f;
    }

    public void UseItem(string itemName)
    {
        foreach (var item in itemSOs)
        {
            if (item.itemName == itemName)
            {
                item.UseItem();
                return;
            }
        }
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        foreach (var slot in itemSlot)
        {
            if ((!slot.isFull && slot.itemName == itemName) || slot.quantity == 0)
            {
                int leftOverItems = slot.AddItem(itemName, quantity, itemSprite, itemDescription);

                if (leftOverItems > 0)
                    return AddItem(itemName, leftOverItems, itemSprite, itemDescription);

                return 0;
            }
        }

        return quantity;
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
