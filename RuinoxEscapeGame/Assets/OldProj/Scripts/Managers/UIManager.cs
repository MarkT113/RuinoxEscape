using UnityEngine;
using TMPro; // Required for TextMeshPro elements
using UnityEngine.UI; // Required for Button, Image etc.

// Manages UI elements like text displays, panels, buttons.
public class UIManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    private static UIManager _instance;
    public static UIManager Instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<UIManager>();
             if (_instance == null) {
                 GameObject go = new GameObject("UIManager");
                 _instance = go.AddComponent<UIManager>();
             }
            return _instance;
        }
    }

    // --- UI Element References (Assign in Inspector) ---
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI oxygenText;
    [SerializeField] private TextMeshProUGUI dashCountText; // Added for Minigame 2/3
    [SerializeField] private Button interactButton; // For main map / minigame interactions
    [SerializeField] private Button pauseButton;

    [Header("Panels (Assign Parent GameObjects)")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject settingsPanel; // Added for settings
    [SerializeField] private GameObject profilePanel;  // Added for profile/login
    [SerializeField] private GameObject loginPanel;    // Added for login page
    [SerializeField] private GameObject signupPanel;   // Added for signup page

    [Header("Dimming Panel")]
     [SerializeField] private Image dimmingPanelImage; // An Image covering the screen, used for dimming


    // --- Unity Lifecycle ---
    void Awake() {
        // --- Enforce Singleton ---
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
             return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
         Debug.Log("UIManager Awake and Persisting.");

        // --- Initial State ---
        // Ensure all panels except maybe HUD are initially hidden
        ShowPauseMenu(false);
        ShowGameOverScreen(false);
        ShowWinScreen(false);
        ShowSettingsPanel(false);
        ShowProfilePanel(false);
        ShowLoginPanel(false);
        ShowSignupPanel(false);
        SetDimmingPanelActive(false);

        // Initial state for interact button might be disabled
        SetInteractButtonVisible(false); // Or based on context
    }

    // --- Public Methods for Updating UI ---

    public void UpdateTimerText(string timeValue) {
        if (timerText != null) timerText.text = timeValue;
        else Debug.LogWarning("Timer Text not assigned in UIManager.");
    }

    public void UpdateOxygenText(string oxygenValue) {
        if (oxygenText != null) oxygenText.text = oxygenValue; // e.g., "Oxygen: 85"
         else Debug.LogWarning("Oxygen Text not assigned in UIManager.");
    }

     public void UpdateDashCount(int count) {
        if (dashCountText != null) dashCountText.text = $"Dashes: {count}";
         else Debug.LogWarning("Dash Count Text not assigned in UIManager.");
    }

    // Show/Hide Panels (can be expanded with animations later)
    public void ShowPauseMenu(bool show) {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(show);
        SetDimmingPanelActive(show); // Dim background when paused
    }

    public void ShowGameOverScreen(bool show) {
        if (gameOverPanel != null) gameOverPanel.SetActive(show);
         SetDimmingPanelActive(show); // Dim background
    }

    public void ShowWinScreen(bool show) {
        if (winPanel != null) winPanel.SetActive(show);
         SetDimmingPanelActive(show); // Dim background
    }

     public void ShowSettingsPanel(bool show) {
        if (settingsPanel != null) settingsPanel.SetActive(show);
         SetDimmingPanelActive(show); // Dim background if needed
    }

     public void ShowProfilePanel(bool show) {
        if (profilePanel != null) profilePanel.SetActive(show);
         // Decide if profile needs dimming
    }
     public void ShowLoginPanel(bool show) {
        if (loginPanel != null) loginPanel.SetActive(show);
         // Decide if login needs dimming
    }
      public void ShowSignupPanel(bool show) {
        if (signupPanel != null) signupPanel.SetActive(show);
         // Decide if signup needs dimming
    }

    // Control interact button visibility/interactivity
    public void SetInteractButtonVisible(bool visible) {
        if (interactButton != null) {
            interactButton.gameObject.SetActive(visible);
            interactButton.interactable = visible; // Make it clickable only when visible
        }
         else Debug.LogWarning("Interact Button not assigned in UIManager.");
    }

     public void SetInteractButtonEnabled(bool enabled) {
         if (interactButton != null) {
             // Keep the button GameObject active, just change interactability
             interactButton.interactable = enabled;
             // Optional: Change visual state (e.g., color tint) when disabled
             // interactButton.GetComponent<Image>().color = enabled ? Color.white : Color.gray;
         }
          else Debug.LogWarning("Interact Button not assigned in UIManager.");
     }

      // Control dimming panel
    private void SetDimmingPanelActive(bool isActive) {
        if (dimmingPanelImage != null) {
            dimmingPanelImage.gameObject.SetActive(isActive);
        } else {
            Debug.LogWarning("Dimming Panel Image not assigned in UIManager.");
        }
    }

    // Add methods here later to update profile page fields (best time, high score, leaderboard)
    // e.g., public void UpdateProfileStats(UserData data) { ... }
    // e.g., public void UpdateLeaderboard(List<LeaderboardEntry> entries) { ... }

}