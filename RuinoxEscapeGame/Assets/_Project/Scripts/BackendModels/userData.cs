using System.Collections.Generic; // Needed for List

// Represents the structure of game data stored locally or fetched from the backend.
// Needs to match the fields in the backend API response / game_data table.
[System.Serializable] // Makes it viewable in Inspector and serializable (e.g., for JSON)
public class UserData
{
    // Info from 'users' table (retrieved on login/signup)
    public int id; // User ID from database
    public string username;

    // Info from 'game_data' table
    public bool has_active_game; // Use bool in C#, convert to 0/1 for DB if needed
    public int current_scene_index = 1;
    public float player_position_x = 0f;
    public float player_position_y = 0f;
    public float current_timer = 0f;
    public int current_oxygen_level = 100;
    public List<int> minigames_status_array = new List<int> { 0, 0, 0 }; // Representing [0,0,0]
    public int dash_charges = 0;
    public int difficulty_level = 1; // 1=Easy, 2=Medium, 3=Hard
    public float? best_time = null; // Use nullable float for time (float?)
    public int high_score = 0;

    // Helper constructor for creating default data
    public UserData(int userId = 0, string name = "Guest") {
        id = userId;
        username = name;
        // Initialize with defaults
        has_active_game = false;
        current_scene_index = GameManager.MAIN_MAP_SCENE_INDEX; // Default to main map
        player_position_x = 0f; // Or determine a proper default spawn point later
        player_position_y = 0f;
        current_timer = 0f;
        current_oxygen_level = 100;
        minigames_status_array = new List<int> { 0, 0, 0 };
        dash_charges = 0;
        difficulty_level = 1; // Default Easy
        best_time = null;
        high_score = 0;
    }

     // Helper to reset only the active game state part
     public void ResetActiveState() {
        has_active_game = false;
        current_scene_index = GameManager.MAIN_MAP_SCENE_INDEX; // Default to main map
        player_position_x = 0f; // Or determine a proper default spawn point later
        player_position_y = 0f;
        current_timer = 0f;
        current_oxygen_level = 100;
        minigames_status_array = new List<int> { 0, 0, 0 };
        dash_charges = 0;
        // Does NOT reset: id, username, difficulty_level, best_time, high_score
     }
}

// --- Helper classes for JSON parsing API responses ---
// We need these because Unity's JsonUtility has limitations

// For Login/Signup response
[System.Serializable]
public class AuthResponse
{
    public int id;
    public string username;
    public string token;
    public string message;
}

// For basic success/error message response
[System.Serializable]
public class MessageResponse
{
    public string message;
    public string error; // Include error field if backend sends it
}

// For fetching the game data (needs to match backend structure closely)
// Note: We defined UserData above, which mostly matches. If the backend nests
// data differently, adjust this class or parse manually.
// For our current GET /api/gamedata, the response maps directly to UserData fields.