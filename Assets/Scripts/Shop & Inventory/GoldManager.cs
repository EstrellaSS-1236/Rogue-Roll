using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; } // Singleton

    [SerializeField] private TextMeshProUGUI goldText; // UI text showing gold
    [SerializeField] private int startingGold = 0;     // Starting gold
    [SerializeField] private int maxGold = 1000;       // Maximum gold cap

    private int currentGold;           // Current gold amount
    private ItemSO pendingItem;        // Item waiting for confirmation

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentGold = startingGold;

        if (goldText == null)
            goldText = GetComponent<TextMeshProUGUI>();

        UpdateGoldUI();
    }

    // Called by ItemSO.UseItem()
    public void TryUseGoldItem(ItemSO item)
    {
        pendingItem = item;

        Debug.Log("[GoldManager] Trying to use " + item.itemName +
                  " | currentGold=" + currentGold +
                  " | amount=" + item.amountToChangeStat +
                  " | max=" + maxGold);

        // If adding would exceed max, show popup first
        if (currentGold + item.amountToChangeStat > maxGold)
        {
            Debug.Log("[GoldManager] Overflow detected, showing popup");

            if (ConfirmationPopupManager.Instance != null)
            {
                // Text shown to player must be in Spanish
                ConfirmationPopupManager.Instance.ShowPopup(
                    "Usar este objeto superara el maximo de pesetas (" + maxGold + "). Quieres usarlo?",
                    () =>
                    {
                        ApplyGold(item.amountToChangeStat);
                        ConsumePendingItem();
                    },
                    () =>
                    {
                        Debug.Log("[GoldManager] Player cancelled using the gold item.");
                        pendingItem = null;
                    }
                );
            }
            else
            {
                Debug.LogWarning("[GoldManager] No ConfirmationPopupManager found, applying directly.");
                ApplyGold(item.amountToChangeStat);
                ConsumePendingItem();
            }
        }
        else
        {
            // Safe add without popup
            ApplyGold(item.amountToChangeStat);
            ConsumePendingItem();
        }
    }

    // Change gold directly (shops, rewards, etc.)
    public void ChangeGold(int amount)
    {
        ApplyGold(amount);
    }

    // Apply gold change
    private void ApplyGold(int amount)
    {
        currentGold += amount;
        if (currentGold > maxGold)
            currentGold = maxGold;

        Debug.Log("[GoldManager] Gold updated: " + currentGold + "/" + maxGold);
        UpdateGoldUI();
    }

    // Consume the pending item after confirmation or safe add
    private void ConsumePendingItem()
    {
        if (pendingItem != null)
        {
            Debug.Log("[GoldManager] Consumed item: " + pendingItem.itemName);

            // Quantity is managed by InventoryManager, not the ScriptableObject
            InventoryManager.Instance.RemoveItem(pendingItem.itemName, 1);

            pendingItem = null;
        }
    }

    // Update the UI
    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            // Text shown to player must be in Spanish
            goldText.text = "Pesetas: " + currentGold + "/" + maxGold;
        }
    }

    // Getters
    public int GetCurrentGold() { return currentGold; }
    public int GetMaxGold() { return maxGold; }
}
