using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Loot/ItemSO")]
public class ItemSO : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;       // Name of the item
    public Sprite icon;           // Icon for inventory or UI
    public int quantity = 1;      // Default quantity
    public int price = 0;         // Price for the shop

    [Header("3D Prefab")]
    public GameObject prefab3D;   // The 3D model to spawn in the world
}