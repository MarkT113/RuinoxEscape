using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Optional: Find existing instance in scene if one exists
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    // Create a new GameObject if no instance exists
                    GameObject singletonObject = new GameObject("GameManager");
                    _instance = singletonObject.AddComponent<GameManager>();
                    Debug.Log("GameManager instance created.");
                }
            }
            return _instance;
        }
    }

    public enum GameState {LandingMenu, Playing, Paused, GameOver, Victory}
    public GameState CurrentState {get; private set;} = GameState.LandingMenu;
    
    public const int LANDING_SCENE_INDEX = 0;
    public const int MAIN_MAP_SCENE_INDEX = 1;
    public const int MINIGAME1_SCENE_INDEX = 2;
    public const int MINIGAME2_SCENE_INDEX = 3;
    public const int MINIGAME3_SCENE_INDEX = 4;
    
    void Awake()
    {
        // --- Enforce Singleton ---
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate GameManager detected. Destroying newcomer.");
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
            Debug.Log("GameManager Awake and Persisting.");
        }
    }
    
    void Start()
    {
        Debug.Log("Initialization Scene Started. Loading Landing Page...");
        LoadScene(LANDING_SCENE_INDEX);
    }

    public void StartNewGame(int difficulty)
    {
        Debug.Log($"Starting New Game with difficulty: {difficulty}");
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        LoadScene(MAIN_MAP_SCENE_INDEX);
    }

    public void ResumeGame(int sceneIndex)
    {
        Debug.Log($"Resuming Game in scene: {sceneIndex}");
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        LoadScene(sceneIndex);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            UIManager.Instance?.ShowPauseMenu(true);
            Debug.Log("Game Paused.");
        }
    }

    public void ResumeGameplay()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            UIManager.Instance?.ShowPauseMenu(false);
            Debug.Log("Game Resumed.");
        }
    }

    public void TriggerGameOver(string reason)
    {
        if (CurrentState == GameState.Playing || CurrentState == GameState.Paused)
        {
            Debug.Log($"Game Over Triggered: {reason}");
            CurrentState = GameState.GameOver;
            Time.timeScale = 0f;
            UIManager.Instance?.ShowGameOverScreen(true);
            StartCoroutine(DelayedReturnToLanding(6f));
            DataManager.Instance?.ResetActiveData(null);
        }
    }

     public void TriggerVictory()
    {
        if (CurrentState == GameState.Playing || CurrentState == GameState.Paused)
        {
            Debug.Log("Victory Triggered!");
            CurrentState = GameState.Victory;
            Time.timeScale = 0f;
            UIManager.Instance?.ShowWinScreen(true);
             DataManager.Instance?.ResetActiveData(null);
            StartCoroutine(DelayedReturnToLanding(6f));
        }
    }
     
    public void RestartGame()
    {
         Debug.Log("Restarting Game...");
         CurrentState = GameState.Playing;
         Time.timeScale = 1f;
         DataManager.Instance?.ResetActiveData( (success) => {
            if(success) LoadScene(MAIN_MAP_SCENE_INDEX);
            else ReturnToLandingPage();
         });
    }

    public void ReturnToLandingPage()
    {
        Debug.Log("Returning to Landing Page...");
        CurrentState = GameState.LandingMenu;
        Time.timeScale = 1f; // Ensure time is running for menus
        LoadScene(LANDING_SCENE_INDEX);
    }
    
    public void LoadScene(int sceneIndex)
    {
        Debug.Log($"Loading Scene Index: {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
        if (sceneIndex == LANDING_SCENE_INDEX) {
             CurrentState = GameState.LandingMenu;
             Time.timeScale = 1f;
        } else if (CurrentState != GameState.Paused) {
             Time.timeScale = 1f;
             if (sceneIndex >= MAIN_MAP_SCENE_INDEX) {
                CurrentState = GameState.Playing;
             }
        }
    }

    private System.Collections.IEnumerator DelayedReturnToLanding(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Wait even if Time.timeScale is 0
        UIManager.Instance?.ShowGameOverScreen(false); // Hide screens before leaving
        UIManager.Instance?.ShowWinScreen(false);
        ReturnToLandingPage();
    }
}