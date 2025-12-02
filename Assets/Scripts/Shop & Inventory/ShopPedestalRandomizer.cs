using UnityEngine;
using System.Collections.Generic;

/*
 * ShopPedestalRandomizer
 * ----------------------
 * Responsibilities:
 *  - Randomly selects an ItemSO from a list of possible items
 *  - Spawns the item's 3D model as a child of displayPoint
 *  - Positions it at the center of the pedestal's top face + vertical offset
 *  - Strips physics components so the display model is static
 *  - Allows scale and vertical offset adjustment via Inspector
 *  - Handles purchase logic
 */
public class ShopPedestalRandomizer : MonoBehaviour
{
    [Header("Possible items for this pedestal")]
    [SerializeField] private ItemSO[] possibleItems;

    [Header("Visuals")]
    [SerializeField] private Transform displayPoint; // parent for spawned item
    [SerializeField] private float displayScale = 0.5f;
    [SerializeField] private float displayYOffset = 0.1f;

    private ItemSO chosenItem;
    private GameObject spawnedModel;

    private void Start()
    {
        RefreshItem();
    }

    private void RefreshItem()
    {
        if (possibleItems == null || possibleItems.Length == 0)
        {
            Debug.LogWarning("No possible items assigned to ShopPedestalRandomizer!");
            return;
        }

        if (spawnedModel != null) Destroy(spawnedModel);

        int index = Random.Range(0, possibleItems.Length);
        chosenItem = possibleItems[index];

        if (chosenItem.prefab3D != null && displayPoint != null)
        {
            // Calculate pedestal top center in world space
            Collider pedestalCollider = GetComponent<Collider>();
            Vector3 worldSpawnPos;

            if (pedestalCollider != null)
            {
                Bounds bounds = pedestalCollider.bounds;
                worldSpawnPos = new Vector3(bounds.center.x, bounds.max.y + displayYOffset, bounds.center.z);
            }
            else
            {
                worldSpawnPos = transform.position + Vector3.up * (1f + displayYOffset);
            }

            // Convert world position to local relative to displayPoint
            Vector3 localSpawnPos = displayPoint.InverseTransformPoint(worldSpawnPos);

            // Instantiate as child of displayPoint
            spawnedModel = Instantiate(chosenItem.prefab3D, displayPoint);
            spawnedModel.transform.localPosition = localSpawnPos;
            spawnedModel.transform.localRotation = Quaternion.identity;
            spawnedModel.transform.localScale = Vector3.one * displayScale;

            // Strip physics
            Rigidbody rb = spawnedModel.GetComponent<Rigidbody>();
            if (rb != null) Destroy(rb);

            Collider col = spawnedModel.GetComponent<Collider>();
            if (col != null) Destroy(col);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (OptionPopupManager.Instance != null && chosenItem != null)
        {
            OptionPopupManager.Instance.ShowPopup(
                "¿Quieres comprar " + chosenItem.itemName + " por " + chosenItem.buyPrice + " Pesetas?",
                new Dictionary<string, System.Action> {
                    { "Sí", () => {
                        int currentGold = StatManager.Instance.GetCurrentValue(ItemSO.StatType.gold);

                        if (currentGold >= chosenItem.buyPrice)
                        {
                            StatManager.Instance.ChangeStat(ItemSO.StatType.gold, -chosenItem.buyPrice);
                            InventoryManager.Instance.AddItem(chosenItem, 1);

                            Debug.Log("Player bought " + chosenItem.itemName + " for " + chosenItem.buyPrice + " Pesetas.");

                            if (spawnedModel != null) Destroy(spawnedModel);
                        }
                        else
                        {
                            Debug.Log("Not enough gold to buy this item!");
                        }
                    }},
                    { "No", () => {
                        Debug.Log("Purchase cancelled.");
                    }}
                }
            );
        }
    }
}
