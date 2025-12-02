using UnityEngine;

/*
 * ItemSO
 * ------
 * ScriptableObject representing an item definition.
 * Holds metadata (name, description, icon, prefab, buy/sell prices, and stat effects).
 */
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public enum StatType
    {
        None,   // Special marker: this item does not change stats
        gold    // Example stat: Pesetas
        // Add more stats here in the future
    }

    [Header("Basic Info")]
    public string itemName;                   // Name of the item
    public Sprite icon;                       // Icon for UI
    [TextArea] public string itemDescription; // Description text
    public GameObject prefab3D;               // Associated 3D prefab in world

    [Header("Stat Effect")]
    public StatType statToChange;             // Which stat this item affects
    public int amountToChangeStat;            // How much to change the stat

    [Header("Shop Settings")]
    public int buyPrice;                      // Cost to buy in Pesetas
    public int sellPrice;                     // Value when selling in Pesetas

    // Display name for stat
    public string GetStatDisplayName()
    {
        switch (statToChange)
        {
            case StatType.gold: return "Pesetas";
            case StatType.None: return "None";
            default: return statToChange.ToString();
        }
    }

    // Current stat value
    public int GetCurrentStatValue()
    {
        if (statToChange == StatType.None) return 0;
        return StatManager.Instance != null ? StatManager.Instance.GetCurrentValue(statToChange) : 0;
    }

    // New stat value after applying item
    public int GetNewStatValue()
    {
        if (statToChange == StatType.None) return 0;
        return GetCurrentStatValue() + amountToChangeStat;
    }

    // Use the item (only if statToChange is not None)
    public void UseItem()
    {
        Debug.Log("[ItemSO] Using " + itemName);

        if (statToChange == StatType.None)
        {
            Debug.Log("[ItemSO] " + itemName + " does not affect stats.");
            return;
        }

        if (StatManager.Instance != null)
        {
            StatManager.Instance.TryUseItem(this);
        }
        else
        {
            Debug.LogWarning("[ItemSO] No StatManager found for " + statToChange);
        }
    }
}
