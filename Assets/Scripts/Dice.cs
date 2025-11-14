using UnityEngine;

public abstract class Dice : Item
{
    public int sides;

    public Dice(string name, int price, int sides)
        : base(name, price, ItemType.Dice)
    {
        this.sides = sides;
    }

    public virtual int Roll()
    {
        return UnityEngine.Random.Range(1, sides + 1);
    }
}

