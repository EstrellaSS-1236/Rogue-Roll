using UnityEngine;
using TMPro; // Needed for TextMeshProUGUI

public class GoldManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText; // Reference to the UI text
    private int currentGold;

    private void Awake()
    {
        // If the script is attached directly to the TMP text object,
        // auto-grab the component so you don’t need to drag it in manually.
        if (goldText == null)
        {
            goldText = GetComponent<TextMeshProUGUI>();
        }

        UpdateGoldUI();
    }

    public void ChangeGold(int amount)
    {
        currentGold += amount;
        if (currentGold < 0)
        {
            currentGold = 0; // Prevent negative gold
        }
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + currentGold;
        }
    }
}
