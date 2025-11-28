using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Loot/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;               // Name of the item
    public Sprite icon;                   // Icon for inventory or UI
    public int quantity = 1;              // Default quantity when picked up
    public int price = 0;                 // Price if sold in a shop
    public int soldBy = 0;                // Vendor ID or reference
    [TextArea] public string itemDescription; // Description shown in UI

    [Header("3D Prefab")]
    public GameObject prefab3D;           // 3D model to spawn in the world

    [Header("Item Effect")]
    public StatToChange statToChange = StatToChange.none; // Which stat this item affects
    public int amountToChangeStat;        // How much to change the stat by

    // Enum for different stats the item can affect
    public enum StatToChange
    {
        none,                             // No effect
        gold                              // Affects gold amount
        // Future: health, mana, stamina, etc.
    }

    // Safely apply the item effect
    public void UseItem()
    {
        switch (statToChange)
        {
            case StatToChange.gold:
                // Use singleton access instead of GameObject.Find
                if (GoldManager.Instance != null)
                {
                    GoldManager.Instance.ChangeGold(amountToChangeStat);
                }
                else
                {
                    Debug.LogWarning($"GoldManager not found when using {itemName}");
                }
                break;

            case StatToChange.none:
            default:
                Debug.Log($"{itemName} has no effect.");
                break;
        }
    }
}
