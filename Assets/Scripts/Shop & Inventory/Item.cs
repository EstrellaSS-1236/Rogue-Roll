using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Properties")]
    [SerializeField] private string itemName;
    [SerializeField] private int quantity = 1;
    [SerializeField] private Sprite sprite;
    [TextArea][SerializeField] private string itemDescription;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas")?.GetComponent<InventoryManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && inventoryManager != null)
        {
            int leftOverItems = inventoryManager.AddItem(itemName, quantity, sprite, itemDescription);

            if (leftOverItems <= 0)
                Destroy(gameObject);
            else
                quantity = leftOverItems;
        }
    }
}
