using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Loot/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;       // Name of the item
    public Sprite icon;           // Icon for inventory or UI
    public int quantity = 1;      // Default quantity
    public int price = 0;         // Price for the shop
    public int soldBy = 0;
    [TextArea] public string itemDescription;

    [Header("3D Prefab")]
    public GameObject prefab3D;   // The 3D model to spawn in the world

    public StatToChange statToChange = new StatToChange();
    public int amountToChangeStat;
    public void UseItem()
    {
        if(statToChange == StatToChange.gold){
            GameObject.Find("GoldManager").GetComponent<GoldManager>().ChangeGold(amountToChangeStat);
        }
    }

    public enum StatToChange
    {
        none,
        gold
    }
}