using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Properties")]
    [SerializeField] private ItemSO itemData;       // Reference to the ScriptableObject that defines this item
    [SerializeField] private int quantity = 1;      // How many of this item to give
    [SerializeField] private InventoryManager inventoryManager; // Reference to inventory manager

    private void Start()
    {
        // If not assigned in Inspector, try to find InventoryManager safely
        if (inventoryManager == null)
        {
            inventoryManager = Object.FindFirstObjectByType<InventoryManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only allow pickup when colliding with the Player
        if (other.CompareTag("Player") && inventoryManager != null && itemData != null)
        {
            // Add item to inventory
            int leftOverItems = inventoryManager.AddItem(
                itemData.itemName,
                quantity,
                itemData.icon,
                itemData.itemDescription
            );

            // If all items were picked up, destroy this object
            if (leftOverItems <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                // Update quantity if some items couldn’t fit
                quantity = leftOverItems;
            }
        }
    }
}
