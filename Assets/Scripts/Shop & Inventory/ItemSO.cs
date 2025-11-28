using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    // Enum defined inside ItemSO
    public enum StatType
    {
        gold       // Gold / Pesetas
    }

    public string itemName;              // Name of the item
    public Sprite icon;                  // Icon for UI
    [TextArea] public string itemDescription; // Description text
    public GameObject prefab3D;          // Associated 3D prefab
    public StatType statToChange;        // Which stat this item affects
    public int amountToChangeStat;       // How much to change the stat

    // Called when the player uses the item from inventory
    public void UseItem()
    {
        Debug.Log("[ItemSO] Using " + itemName + " | statToChange=" + statToChange);

        if (statToChange == StatType.gold && GoldManager.Instance != null)
        {
            GoldManager.Instance.TryUseGoldItem(this);
        }
        else
        {
            Debug.LogWarning("[ItemSO] statToChange not handled: " + statToChange);
        }
    }
}
