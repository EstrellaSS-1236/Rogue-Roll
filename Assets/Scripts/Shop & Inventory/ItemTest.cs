using UnityEngine;
public class ItemTest : MonoBehaviour
{
    [SerializeField] private ItemSO itemData;       // Assign your D6 or other ItemSO in Inspector
    [SerializeField] private int quantity = 1;      // How many of this item to give
    [SerializeField] private InventoryManager inventoryManager;

    private GameObject spawnedModel;

    private void Start()
    {
        // Spawn the 3D model defined in the ItemSO
        if (itemData != null && itemData.prefab3D != null)
        {
            spawnedModel = Instantiate(
                itemData.prefab3D,
                transform.position,
                transform.rotation,
                transform // parent to this empty slot
            );
        }

        // If not assigned in Inspector, try to find InventoryManager
        if (inventoryManager == null)
        {
            inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && itemData != null)
        {
            // Add to inventory
            int leftOverItems = inventoryManager.AddItem(
                itemData.itemName,
                quantity,
                itemData.icon,
                itemData.itemDescription
            );

            if (leftOverItems <= 0)
            {
                Destroy(gameObject); // fully picked up
            }
            else
            {
                quantity = leftOverItems; // still some left
            }
        }
    }
}
