using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConfirmationPopupManager : MonoBehaviour
{
    public static ConfirmationPopupManager Instance { get; private set; } // Singleton instance

    [Header("Popup UI")]
    [SerializeField] private GameObject popupPanel;   // The panel GameObject
    [SerializeField] private TextMeshProUGUI popupText; // The text message
    [SerializeField] private Button yesButton;        // Yes button
    [SerializeField] private Button noButton;         // No button

    private System.Action onYesAction; // Action to run if player clicks Yes
    private System.Action onNoAction;  // Action to run if player clicks No

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Hide popup at start
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    // Show popup with custom message and callbacks
    public void ShowPopup(string message, System.Action onYes, System.Action onNo)
    {
        if (popupPanel == null || popupText == null || yesButton == null || noButton == null)
        {
            Debug.LogError("Popup references not set in Inspector!");
            return;
        }

        popupPanel.SetActive(true); // show panel
        popupText.text = message;   // set message

        onYesAction = onYes;
        onNoAction = onNo;

        // Clear old listeners to avoid stacking
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        // Add new listeners
        yesButton.onClick.AddListener(() =>
        {
            popupPanel.SetActive(false);
            onYesAction?.Invoke();
        });

        noButton.onClick.AddListener(() =>
        {
            popupPanel.SetActive(false);
            onNoAction?.Invoke();
        });
    }
}
