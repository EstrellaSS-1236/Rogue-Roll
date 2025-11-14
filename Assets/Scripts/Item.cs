using UnityEngine;
public enum ItemType
{
    Permanent,
    Consumable,
    Dice
}
public abstract class Item
{
    public string itemName;
    public int price;
    public ItemType itemType;
    public Item(string name, int price, ItemType type)
    {
        this.itemName = name;
        this.price = price;
        this.itemType = type;
    }
    public abstract void Use();
}

