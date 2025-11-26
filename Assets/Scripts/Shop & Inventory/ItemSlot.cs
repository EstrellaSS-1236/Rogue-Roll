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

    private InventoryManager inventoryManager;

    private void Awake()
    {
        inventoryManager = GameObject.Find("InventoryCanvas")?.GetComponent<InventoryManager>();

        if (quantityText != null)
        {
            quantityText.text = string.Empty;
            quantityText.enabled = false;
        }

        if (selectedShader != null) selectedShader.SetActive(false);
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        if (isFull) return quantity;

        this.itemName = itemName;
        this.itemSprite = itemSprite;
        itemImage.sprite = itemSprite;
        this.itemDescription = itemDescription;

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
            quantityText.text = this.quantity.ToString();
            quantityText.enabled = this.quantity > 0;
        }
    }

    // Force TMP to refresh when menu is enabled
    public void ForceUpdateUI()
    {
        if (quantityText != null)
        {
            quantityText.ForceMeshUpdate();
            quantityText.text = this.quantity.ToString();
            quantityText.enabled = this.quantity > 0;
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
            inventoryManager?.UseItem(itemName);

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

    private void OnRightClick()
    {
        Debug.Log($"Right-clicked on {itemName}. Future: drop/split functionality.");
    }
}
