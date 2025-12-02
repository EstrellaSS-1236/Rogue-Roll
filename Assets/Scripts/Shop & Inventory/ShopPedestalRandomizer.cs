using UnityEngine;
using System.Collections.Generic;

/*
 * ShopPedestalRandomizer
 * ----------------------
 * This script is attached to each shop pedestal prefab.
 * Responsibilities:
 *  - Randomly selects an ItemSO from a list of possible items
 *  - Spawns the item's 3D model at the DisplayPoint
 *  - Shows a popup when the player enters the pedestal trigger
 *  - Handles purchase logic: deduct gold, add item to inventory, remove model
 */
public class ShopPedestalRandomizer : MonoBehaviour
{
    [Header("Possible items for this pedestal")]
    [SerializeField] private ItemSO[] possibleItems; // List of items that can appear on this pedestal

    [Header("Visuals")]
    [SerializeField] private Transform displayPoint; // Empty child transform where the item model will spawn

    private ItemSO chosenItem;       // The item currently displayed on this pedestal
    private GameObject spawnedModel; // Reference to the spawned 3D model

    private void Start()
    {
        // Spawn an item immediately when the scene loads
        RefreshItem();
    }

    /*
     * RefreshItem
     * -----------
     * Picks a random item from possibleItems and spawns its 3D model at the displayPoint.
     * Called at Start() and can be called again if you want to re-roll items later.
     */
    private void RefreshItem()
    {
        if (possibleItems == null || possibleItems.Length == 0)
        {
            Debug.LogWarning("No possible items assigned to ShopPedestalRandomizer!");
            return;
        }

        // Destroy old model if one exists
        if (spawnedModel != null) Destroy(spawnedModel);

        // Pick a random item from the array
        int index = Random.Range(0, possibleItems.Length);
        chosenItem = possibleItems[index];

        // Spawn its 3D model at the DisplayPoint
        if (chosenItem.prefab3D != null && displayPoint != null)
        {
            spawnedModel = Instantiate(chosenItem.prefab3D, displayPoint);
            spawnedModel.transform.localPosition = Vector3.zero;
            spawnedModel.transform.localRotation = Quaternion.identity;
            spawnedModel.transform.localScale = Vector3.one * 1.5f; // Scale up slightly for visibility
        }
    }

    /*
     * OnTriggerEnter
     * ---------------
     * Called when the player enters the pedestal's trigger collider.
     * Shows a popup asking if they want to buy the item.
     */
    private void OnTriggerEnter(Collider other)
    {
        // Only react to the player
        if (!other.CompareTag("Player")) return;

        // Show popup if OptionPopupManager exists and we have a chosen item
        if (OptionPopupManager.Instance != null && chosenItem != null)
        {
            OptionPopupManager.Instance.ShowPopup(
                "¿Quieres comprar " + chosenItem.itemName + " por " + chosenItem.buyPrice + " Pesetas?",
                new Dictionary<string, System.Action> {
                    { "Sí", () => {
                        int currentGold = StatManager.Instance.GetCurrentValue(ItemSO.StatType.gold);

                        // Check if player has enough gold
                        if (currentGold >= chosenItem.buyPrice)
                        {
                            // Deduct gold
                            StatManager.Instance.ChangeStat(ItemSO.StatType.gold, -chosenItem.buyPrice);

                            // Add item to inventory using the overload that accepts ItemSO
                            InventoryManager.Instance.AddItem(chosenItem, 1);

                            Debug.Log("Player bought " + chosenItem.itemName + " for " + chosenItem.buyPrice + " Pesetas.");

                            // Remove the model from the pedestal
                            if (spawnedModel != null) Destroy(spawnedModel);
                        }
                        else
                        {
                            Debug.Log("Not enough gold to buy this item!");
                        }
                    }},
                    { "No", () => {
                        // Do nothing except log
                        Debug.Log("Purchase cancelled.");
                    }}
                }
            );
        }
    }
}
