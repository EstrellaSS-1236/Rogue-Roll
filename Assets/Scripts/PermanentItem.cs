public class PermanentItem : Item
{
    public int quantity;

    public PermanentItem(string name, int price, int quantity = 1)
        : base(name, price, ItemType.Permanent)
    {
        this.quantity = quantity;
    }

    public override void Use()
    {
        UnityEngine.Debug.Log("Using " + itemName + " x" + quantity);
        // Apply permanent effect (could be passive)
    }

    public void AddOne()
    {
        quantity++;
    }

    public void RemoveOne()
    {
        if (quantity > 0) quantity--;
    }
}
