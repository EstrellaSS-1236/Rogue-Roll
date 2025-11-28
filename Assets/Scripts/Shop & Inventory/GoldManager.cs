using UnityEngine;
using TMPro; // Needed for TextMeshProUGUI

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; } // Singleton instance

    [SerializeField] private TextMeshProUGUI goldText; // Reference to the UI text
    [SerializeField] private int startingGold = 0;     // Starting gold value

    private int currentGold; // Tracks current gold amount

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize gold
        currentGold = startingGold;

        // Auto-grab TMP component if not assigned
        if (goldText == null)
        {
            goldText = GetComponent<TextMeshProUGUI>();
        }

        UpdateGoldUI();
    }

    public void ChangeGold(int amount)
    {
        // Add or subtract gold
        currentGold += amount;

        // Prevent negative gold
        if (currentGold < 0)
        {
            currentGold = 0;
        }

        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        // Update UI text safely
        if (goldText != null)
        {
            goldText.text = "Gold: " + currentGold;
        }
    }

    public int GetCurrentGold()
    {
        // Allow other scripts to read current gold
        return currentGold;
    }
}
