using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public ItemSlot[] itemSlot;

    private bool menuActivated;

    private void Start()
    {
        InventoryMenu.SetActive(false);
    }
    public void ToggleInventory()
    {
        menuActivated = !menuActivated;

        if (InventoryMenu != null)
            InventoryMenu.SetActive(menuActivated);

        Time.timeScale = menuActivated ? 0 : 1;
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false && itemSlot[i].itemName == itemName || itemSlot[i].quantity == 0)
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                if (leftOverItems > 0)
                    leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription);

                return leftOverItems;
                  
            }
        }
        return quantity;
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].selectedShader != null)
                itemSlot[i].selectedShader.SetActive(false);

            itemSlot[i].thisItemSelected = false;
        }
    }
}
