using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //======ITEM DATA======//
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    [SerializeField] private int maxNumberOfItems;

    //======ITEM SLOT======//
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;

    //======ITEM DESCRIPTION SLOT======//
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    private void Awake()
    {
        inventoryManager = GameObject.Find("InventoryCanvas")?.GetComponent<InventoryManager>();

        if (quantityText != null)
        {
            quantityText.text = string.Empty;
            quantityText.enabled = false;
        }

        if (selectedShader != null)
            selectedShader.SetActive(false);
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // Check to see if the slot is already full
        if (isFull)
        {
            Debug.Log($"Slot already full, returning {quantity} items");
            return quantity;
        }

        this.itemName = itemName;
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;
        this.itemDescription = itemDescription;

        this.quantity += quantity;
        Debug.Log($"Slot {gameObject.name} now has quantity = {this.quantity}");

        if (this.quantity >= maxNumberOfItems)
        {
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            isFull = true;

            quantityText.text = this.quantity.ToString();
            quantityText.enabled = true;

            Debug.Log($"Slot reached max. Text set to {quantityText.text}, leftover = {extraItems}");
            return extraItems;
        }

        quantityText.text = this.quantity.ToString();
        quantityText.enabled = true;

        Debug.Log($"Text updated to {quantityText.text}, enabled = {quantityText.enabled}");
        return 0;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                OnLeftClick();
                break;
            case PointerEventData.InputButton.Right:
                OnRightClick();
                break;
        }
    }

    private void OnLeftClick()
    {
        inventoryManager?.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        itemDescriptionNameText.text = itemName;
        itemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
        if (itemDescriptionImage.sprite == null)
        {
            itemDescriptionImage.sprite = emptySprite;
        }
    }

    private void OnRightClick()
    {
    }
}
