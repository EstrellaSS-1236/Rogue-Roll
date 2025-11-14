public class Consumable : Item
{
    public Consumable(string name, int price)
        : base(name, price, ItemType.Consumable) { }

    public override void Use()
    {
        UnityEngine.Debug.Log("Consumed " + itemName);
        // Apply consumable effect
        // After use, remove from inventory
    }
}
