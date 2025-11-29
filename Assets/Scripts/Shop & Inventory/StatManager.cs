using UnityEngine;
using TMPro;
using System.Collections.Generic;

/* 
 * Serializable binding so you can connect each stat type
 * to a TextMeshProUGUI element in the Inspector.
 */
[System.Serializable]
public class StatUIBinding
{
    public ItemSO.StatType statType;       // Which stat this binding represents
    public TextMeshProUGUI statText;       // UI text element to update
}

public class StatManager : MonoBehaviour
{
    public static StatManager Instance { get; private set; }

    [Header("UI Bindings")]
    [SerializeField] private List<StatUIBinding> statBindings; // List of stat-to-UI mappings

    [Header("Starting Values")]
    [SerializeField] private int startingGold = 0;   // Initial gold value
    [SerializeField] private int maxGold = 1000;     // Maximum gold allowed

    // Dictionaries to track current and max values for each stat
    private Dictionary<ItemSO.StatType, int> currentValues = new Dictionary<ItemSO.StatType, int>();
    private Dictionary<ItemSO.StatType, int> maxValues = new Dictionary<ItemSO.StatType, int>();

    // Lookup for UI text elements
    private Dictionary<ItemSO.StatType, TextMeshProUGUI> statTextLookup = new Dictionary<ItemSO.StatType, TextMeshProUGUI>();

    // Item currently being processed (used for confirmation popups)
    private ItemSO pendingItem;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Build lookup for UI texts
        foreach (var binding in statBindings)
        {
            if (!statTextLookup.ContainsKey(binding.statType))
                statTextLookup[binding.statType] = binding.statText;
        }

        // Initialize gold stat
        currentValues[ItemSO.StatType.gold] = startingGold;
        maxValues[ItemSO.StatType.gold] = maxGold;

        // Update UI at start
        UpdateUI(ItemSO.StatType.gold);
    }

    /* Called by ItemSO.UseItem() when player tries to use an item */
    public void TryUseItem(ItemSO item)
    {
        pendingItem = item;
        var stat = item.statToChange;

        int current = GetCurrentValue(stat);
        int max = GetMaxValue(stat);

        Debug.Log($"[StatManager] Trying to use {item.itemName} | stat={stat} | current={current} | amount={item.amountToChangeStat} | max={max}");

        // If using the item would exceed the max, show a confirmation popup
        if (current + item.amountToChangeStat > max)
        {
            Debug.Log("[StatManager] Overflow detected, showing popup");

            if (OptionPopupManager.Instance != null)
            {
                OptionPopupManager.Instance.ShowPopup(
                    $"Usar este objeto superará el máximo de {GetDisplayName(stat)} ({max}). ¿Quieres usarlo?",
                    new Dictionary<string, System.Action> {
                        { "Sí", () => {
                            ApplyChange(stat, item.amountToChangeStat);
                            ConsumePendingItem();
                        }},
                        { "No", () => {
                            Debug.Log("[StatManager] Player cancelled using the item.");
                            pendingItem = null;
                        }}
                    }
                );
            }
            else
            {
                ApplyChange(stat, item.amountToChangeStat);
                ConsumePendingItem();
            }
        }
        else
        {
            // Safe to apply directly
            ApplyChange(stat, item.amountToChangeStat);
            ConsumePendingItem();
        }
    }

    /* Direct stat change without item */
    public void ChangeStat(ItemSO.StatType stat, int amount)
    {
        ApplyChange(stat, amount);
    }

    /* Apply change and clamp between 0 and max */
    private void ApplyChange(ItemSO.StatType stat, int amount)
    {
        if (!currentValues.ContainsKey(stat)) currentValues[stat] = 0;
        if (!maxValues.ContainsKey(stat)) maxValues[stat] = int.MaxValue;

        currentValues[stat] += amount;
        if (currentValues[stat] > maxValues[stat]) currentValues[stat] = maxValues[stat];
        if (currentValues[stat] < 0) currentValues[stat] = 0;

        Debug.Log($"[StatManager] {stat} updated: {currentValues[stat]}/{maxValues[stat]}");
        UpdateUI(stat);
    }

    /* Remove one unit of the pending item from inventory */
    private void ConsumePendingItem()
    {
        if (pendingItem != null)
        {
            Debug.Log("[StatManager] Consumed item: " + pendingItem.itemName);
            InventoryManager.Instance.RemoveItem(pendingItem.itemName, 1);
            pendingItem = null;
        }
    }

    /* Update UI text for a given stat */
    private void UpdateUI(ItemSO.StatType stat)
    {
        if (statTextLookup.TryGetValue(stat, out var text))
        {
            int current = GetCurrentValue(stat);
            int max = GetMaxValue(stat);
            string displayName = GetDisplayName(stat);

            text.text = $"{displayName}: {current}/{max}";
        }
    }

    /* Cleaner approach: static display names */
    private string GetDisplayName(ItemSO.StatType stat)
    {
        switch (stat)
        {
            case ItemSO.StatType.gold: return "Pesetas";
            default: return stat.ToString();
        }
    }

    public int GetCurrentValue(ItemSO.StatType stat) =>
        currentValues.ContainsKey(stat) ? currentValues[stat] : 0;

    public int GetMaxValue(ItemSO.StatType stat) =>
        maxValues.ContainsKey(stat) ? maxValues[stat] : int.MaxValue;
}
