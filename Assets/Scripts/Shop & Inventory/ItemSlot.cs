using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //====== ITEM DATA ======//
    public string itemName;                  // Name of the item stored in this slot
    public int quantity;                     // Current quantity of the item
    public Sprite itemSprite;                // Sprite representing the item
    public bool isFull;                      // Flag to check if slot reached max capacity
    public string itemDescription;           // Description of the item
    public Sprite emptySprite;               // Sprite shown when slot is empty

    [SerializeField] private int maxNumberOfItems = 10; // Maximum items per slot

    //====== UI REFERENCES ======//
    [SerializeField] private TMP_Text quantityText;     // Text showing item quantity
    [SerializeField] private Image itemImage;           // Image showing item sprite

    //====== ITEM DESCRIPTION UI ======//
    public Image itemDescriptionImage;                  // Image in description panel
    public TMP_Text itemDescriptionNameText;            // Name text in description panel
    public TMP_Text itemDescriptionText;                // Description text in description panel

    //====== SELECTION ======//
    public GameObject selectedShader;                   // Highlight shader when selected
    public bool thisItemSelected;                       // Flag if this slot is selected

    private InventoryManager inventoryManager;          // Reference to inventory manager

    private void Awake()
    {
        // Cache InventoryManager safely
        inventoryManager = Object.FindFirstObjectByType<InventoryManager>();

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
        if (isFull) return quantity; // If slot is already full, return all items

        // Assign item data
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        itemImage.sprite = itemSprite;

        // Add quantity
        this.quantity += quantity;

        // Handle overflow if quantity exceeds max
        if (this.quantity >= maxNumberOfItems)
        {
            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
            isFull = true;

            UpdateUI();
            return extraItems; // Return leftover items
        }

        UpdateUI();
        return 0; // No leftover items
    }

    private void UpdateUI()
    {
        // Update quantity text visibility and value
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
        // Force TMP refresh to fix UI glitches when reopening inventory
        if (quantityText != null)
        {
            quantityText.ForceMeshUpdate();
            UpdateUI();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Handle left and right clicks
        if (eventData.button == PointerEventData.InputButton.Left)
            OnLeftClick();
        else if (eventData.button == PointerEventData.InputButton.Right)
            OnRightClick();
    }

    private void OnLeftClick()
    {
        if (thisItemSelected)
        {
            // Use item if already selected
            if (!string.IsNullOrEmpty(itemName))
            {
                inventoryManager?.UseItem(itemName);
                quantity--;

                UpdateUI();

                if (quantity <= 0)
                    EmptySlot();
            }
        }
        else
        {
            // Select this slot and update description UI
            inventoryManager?.DeselectAllSlots();
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

    private void EmptySlot()
    {
        quantity = 0;
        isFull = false;
        thisItemSelected = false;

        // Clear item data
        itemName = string.Empty;
        itemSprite = null;
        itemDescription = string.Empty;

        // Reset UI
        if (quantityText != null) quantityText.enabled = false;
        if (itemImage != null) itemImage.sprite = emptySprite;

        if (itemDescriptionNameText != null) itemDescriptionNameText.text = "";
        if (itemDescriptionText != null) itemDescriptionText.text = "";
        if (itemDescriptionImage != null) itemDescriptionImage.sprite = emptySprite;

        // Hide selection shader
        if (selectedShader != null) selectedShader.SetActive(false);
    }

    private void OnRightClick()
    {
        // Placeholder for future drop/split functionality
        Debug.Log($"Right-clicked on {itemName}. Future: drop/split functionality.");
    }
}
