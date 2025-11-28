using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //====== ITEM DATA ======//
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;

    [SerializeField] private int maxNumberOfItems = 10;

    //====== UI REFERENCES ======//
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Image itemImage;

    //====== ITEM DESCRIPTION UI ======//
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    //====== SELECTION ======//
    public GameObject selectedShader;
    public bool thisItemSelected;

    private void Awake()
    {
        // Initialize quantity text
        if (quantityText != null)
        {
            quantityText.text = string.Empty;
            quantityText.enabled = false;
        }

        // Ensure selection shader is hidden initially
        if (selectedShader != null) selectedShader.SetActive(false);
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        if (isFull) return quantity;

        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        itemImage.sprite = itemSprite;

        this.quantity += quantity;

        if (this.quantity >= maxNumberOfItems)
        {
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            isFull = true;

            UpdateUI();
            return extraItems;
        }

        UpdateUI();
        return 0;
    }

    private void UpdateUI()
    {
        if (quantityText != null)
        {
            if (quantity > 0)
            {
                quantityText.text = quantity.ToString();
                quantityText.enabled = true;
            }
            else
            {
                quantityText.text = string.Empty;
                quantityText.enabled = false;
            }
        }
    }

    public void ForceUpdateUI()
    {
        if (quantityText != null)
        {
            quantityText.ForceMeshUpdate();
            UpdateUI();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick();
    }

    private void OnLeftClick()
    {
        if (thisItemSelected)
        {
            if (!string.IsNullOrEmpty(itemName))
            {
                // Call UseItem, but don't decrement here
                InventoryManager.Instance?.UseItem(itemName);

                // UI will update after GoldManager consumes the item
                UpdateUI();

                if (quantity <= 0)
                    ClearSlot();
            }
        }
        else
        {
            InventoryManager.Instance?.DeselectAllSlots();
            selectedShader?.SetActive(true);
            thisItemSelected = true;

            if (itemDescriptionNameText != null)
                itemDescriptionNameText.text = itemName;

            if (itemDescriptionText != null)
                itemDescriptionText.text = itemDescription;

            if (itemDescriptionImage != null)
                itemDescriptionImage.sprite = itemSprite ?? emptySprite;
        }
    }

    public void ClearSlot()
    {
        quantity = 0;
        isFull = false;
        thisItemSelected = false;

        itemName = string.Empty;
        itemSprite = null;
        itemDescription = string.Empty;

        if (quantityText != null) quantityText.enabled = false;
        if (itemImage != null) itemImage.sprite = emptySprite;

        if (itemDescriptionNameText != null) itemDescriptionNameText.text = "";
        if (itemDescriptionText != null) itemDescriptionText.text = "";
        if (itemDescriptionImage != null) itemDescriptionImage.sprite = emptySprite;

        if (selectedShader != null) selectedShader.SetActive(false);
    }

    private void OnRightClick()
    {
        Debug.Log($"Right-clicked on {itemName}. Future: drop/split functionality.");
    }
}
