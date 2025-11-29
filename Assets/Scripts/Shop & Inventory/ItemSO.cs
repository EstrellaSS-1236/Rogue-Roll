using UnityEngine;

/* 
 * ScriptableObject representing an item definition.
 * Holds metadata (name, icon, description, prefab) and stat effects.
 */
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    // Enum of all possible stats this item can affect
    public enum StatType
    {
        gold // Oro / Pesetas
        // Add more stats here in the future
    }

    public string itemName;                  // Name of the item
    public Sprite icon;                      // Icon for UI
    [TextArea] public string itemDescription; // Description text
    public GameObject prefab3D;              // Associated 3D prefab in world

    public StatType statToChange;            // Which stat this item affects
    public int amountToChangeStat;           // How much to change the stat

    /* Display name for the stat (Spanish labels) */
    public string GetStatDisplayName()
    {
        switch (statToChange)
        {
            case StatType.gold: return "Pesetas";
            default: return statToChange.ToString();
        }
    }

    /* Current value of the stat this item affects */
    public int GetCurrentStatValue()
    {
        return StatManager.Instance != null ? StatManager.Instance.GetCurrentValue(statToChange) : 0;
    }

    /* New value after applying this item */
    public int GetNewStatValue()
    {
        return GetCurrentStatValue() + amountToChangeStat;
    }

    /* Called when the player uses the item from inventory */
    public void UseItem()
    {
        Debug.Log("[ItemSO] Usando " + itemName + " | statToChange=" + statToChange);
        if (StatManager.Instance != null)
        {
            StatManager.Instance.TryUseItem(this);
        }
        else
        {
            Debug.LogWarning("[ItemSO] No StatManager encontrado para " + statToChange);
        }
    }
}
