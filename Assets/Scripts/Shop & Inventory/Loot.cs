using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSO itemSO;  // Reference to the item data

    private void Start()
    {
        // Instantiate the 3D model from the ItemSO
        if (itemSO != null && itemSO.prefab3D != null)
        {
            GameObject model = Instantiate(itemSO.prefab3D, transform);
            model.transform.localPosition = Vector3.zero;    // Center the model
            model.transform.localRotation = Quaternion.identity;
        }

        // Optionally rename the object to match the item
        if (itemSO != null)
            this.name = itemSO.itemName;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player touches the loot
        if (other.CompareTag("Player"))
        {
            // TODO: Add the item to inventory here
            // Inventory.Add(itemSO);

            // Destroy the loot object immediately
            Destroy(gameObject);
        }
    }
}
