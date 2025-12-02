using UnityEngine;
using System.Collections.Generic;

/*
 * SellPedestal
 * ------------
 * This script is attached to a pedestal (or trigger zone) that allows the player to sell items.
 * It does not display any 3D object. When the player enters the trigger, it opens a popup
 * where they can choose to sell items from their inventory.
 */
public class SellPedestal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Only react to the player
        if (!other.CompareTag("Player")) return;

        // Find the first item in the inventory (basic version)
        ItemSlot[] slots = InventoryManager.Instance.ItemSlots;
        ItemSlot firstFilledSlot = null;

        foreach (var slot in slots)
        {
            if (slot != null && slot.quantity > 0)
            {
                firstFilledSlot = slot;
                break; // For now, just pick the first item found
            }
        }

        // If no items are found, exit
        if (firstFilledSlot == null)
        {
            Debug.Log("No items in inventory to sell.");
            return;
        }

        // Lookup the ItemSO by name
        ItemSO itemToSell = InventoryManager.Instance.GetItemSO(firstFilledSlot.itemName);

        if (itemToSell == null)
        {
            Debug.LogWarning("Could not find ItemSO for " + firstFilledSlot.itemName);
            return;
        }

        // Show popup in Spanish for the player
        if (OptionPopupManager.Instance != null)
        {
            OptionPopupManager.Instance.ShowPopup(
                "¿Quieres vender " + itemToSell.itemName + " por " + itemToSell.sellPrice + " Pesetas?",
                new Dictionary<string, System.Action> {
                    { "Sí", () => {
                        // Remove one item from inventory
                        InventoryManager.Instance.RemoveItem(itemToSell.itemName, 1);

                        // Add gold equal to the item's sell price
                        StatManager.Instance.ChangeStat(ItemSO.StatType.gold, itemToSell.sellPrice);

                        Debug.Log("Player sold " + itemToSell.itemName + " for " + itemToSell.sellPrice + " Pesetas.");
                    }},
                    { "No", () => {
                        // Do nothing except log
                        Debug.Log("Sale cancelled.");
                    }}
                }
            );
        }
    }
}