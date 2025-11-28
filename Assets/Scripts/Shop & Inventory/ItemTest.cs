using UnityEngine;

public class ItemTest : MonoBehaviour
{
    [SerializeField] private ItemSO itemData;              // Reference to the ScriptableObject that defines this item
    [SerializeField] private int quantity = 1;             // How many of this item to give
    [SerializeField] private InventoryManager inventoryManager; // Reference to inventory manager

    private GameObject spawnedModel; // Holds the spawned 3D model

    private void Start()
    {
        // Spawn the 3D model defined in the ItemSO
        if (itemData != null && itemData.prefab3D != null)
        {
            spawnedModel = Instantiate(
                itemData.prefab3D,
                transform.position,
                transform.rotation,
                transform // parent to this object
            );
        }

        // If not assigned in Inspector, try to find InventoryManager safely
        if (inventoryManager == null)
        {
            inventoryManager = Object.FindFirstObjectByType<InventoryManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only allow pickup when colliding with the Player
        if (other.CompareTag("Player") && itemData != null && inventoryManager != null)
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
