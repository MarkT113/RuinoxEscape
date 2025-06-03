using UnityEngine;
using UnityEngine.Networking; // Required for UnityWebRequest
using System; // Required for Action callbacks
using System.Collections; // Required for Coroutines (IEnumerator)
using System.Text; // Required for Encoding POST data
using System.Collections.Generic; // Required for List

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance {
        get {
            if (_instance == null) _instance = FindObjectOfType<DataManager>();
            if (_instance == null) {
                 GameObject go = new GameObject("DataManager");
                 _instance = go.AddComponent<DataManager>();
            }
            return _instance;
        }
    }

    // --- Backend API ---
    // IMPORTANT: Replace localhost with your computer's LOCAL IP ADDRESS
    // if testing on a mobile device connected to the SAME WiFi network.
    // Find local IP: Windows (ipconfig in cmd), Mac (ifconfig in terminal or Network Preferences)
    private string backendUrl = "http://localhost:3000"; // CHANGE FOR MOBILE TESTING
    private string apiToken = null; // Stores the JWT token after login

    // --- User State ---
    public bool IsLoggedIn { get; private set; } = false;
    public UserData CurrentUserData { get; private set; } = null; // Holds loaded data (local or remote)

    // --- PlayerPrefs Keys (Constants for safety) ---
    private const string PREFS_PREFIX = "AstronautGame_";
    private const string PREFS_IS_LOGGED_IN = PREFS_PREFIX + "IsLoggedIn";
    private const string PREFS_AUTH_TOKEN = PREFS_PREFIX + "AuthToken";
    private const string PREFS_USER_DATA_GUEST = PREFS_PREFIX + "UserDataGuest"; // Store guest data as JSON string

    // --- Unity Lifecycle ---
    void Awake() {
        // --- Enforce Singleton ---
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("DataManager Awake and Persisting.");

        // --- Load Initial State ---
        LoadLoginState(); // Check if we were previously logged in
    }

    // --- Login / Logout ---

    private void LoadLoginState() {
        IsLoggedIn = PlayerPrefs.GetInt(PREFS_IS_LOGGED_IN, 0) == 1;
        if (IsLoggedIn) {
            apiToken = PlayerPrefs.GetString(PREFS_AUTH_TOKEN, null);
            if (string.IsNullOrEmpty(apiToken)) {
                // If token is missing somehow, force logout
                IsLoggedIn = false;
                PlayerPrefs.SetInt(PREFS_IS_LOGGED_IN, 0);
                PlayerPrefs.DeleteKey(PREFS_AUTH_TOKEN);
                 Debug.LogWarning("Was logged in but token missing. Forced logout.");
            } else {
                Debug.Log("Previously logged in. Token loaded.");
                // Optionally: Could try to fetch user data immediately here
                // LoadData(null); // Be careful about timing if called in Awake
            }
        } else {
             Debug.Log("Not logged in (Guest Mode).");
             LoadLocalGuestData(); // Load guest data if not logged in
        }
    }

    public void SetLoggedIn(AuthResponse authData) {
        IsLoggedIn = true;
        apiToken = authData.token;
        CurrentUserData = new UserData(authData.id, authData.username); // Create basic user data shell

        PlayerPrefs.SetInt(PREFS_IS_LOGGED_IN, 1);
        PlayerPrefs.SetString(PREFS_AUTH_TOKEN, apiToken);
        PlayerPrefs.Save(); // Ensure prefs are saved immediately

        Debug.Log($"SetLoggedIn: User '{authData.username}' (ID: {authData.id}). Token saved.");

        // After logging in, immediately try to load their data from the server
        LoadData( (success, loadedData) => {
             if(success) Debug.Log("User data loaded successfully after login.");
             else Debug.LogError("Failed to load user data after login!");
        });
    }

    public void SetLoggedOut() {
        IsLoggedIn = false;
        apiToken = null;
        CurrentUserData = null; // Clear user data

        PlayerPrefs.SetInt(PREFS_IS_LOGGED_IN, 0);
        PlayerPrefs.DeleteKey(PREFS_AUTH_TOKEN);
        // Keep guest data: PlayerPrefs.DeleteKey(PREFS_USER_DATA_GUEST);
        PlayerPrefs.Save();
        Debug.Log("SetLoggedOut: Session cleared.");
        LoadLocalGuestData(); // Load guest data after logging out
    }

    // --- Public Data Access Methods ---

    // Load data - Decides based on login state
    public void LoadData(Action<bool, UserData> callback) {
        if (IsLoggedIn) {
            StartCoroutine(GetRequest("/api/gamedata", apiToken, (success, responseJson) => {
                if (success) {
                    try {
                        // Directly parse the response into UserData
                        // Note: Backend sends full game_data structure, which matches UserData fields
                        UserData loadedData = JsonUtility.FromJson<UserData>(responseJson);

                        // Need to handle the minigames_status JSON string parsing manually
                        // JsonUtility doesn't handle nested arrays/lists well directly from root object
                         if (loadedData != null) {
                            // Attempt to parse the string field if backend didn't send _array version
                             JsonHelper.ParseMinigamesStatus(responseJson, loadedData); // Use helper
                             CurrentUserData = loadedData; // Store the fully loaded data
                             CurrentUserData.id = loadedData.id; // Map user_id if backend sends it this way
                             Debug.Log($"Remote data loaded. HasActiveGame: {CurrentUserData.has_active_game}");
                             callback?.Invoke(true, CurrentUserData);
                         } else {
                             Debug.LogError("Failed to parse game data JSON.");
                            callback?.Invoke(false, null);
                         }

                    } catch (Exception e) {
                        Debug.LogError($"Error parsing game data JSON: {e.Message}\nJSON: {responseJson}");
                        callback?.Invoke(false, null);
                    }
                } else {
                    Debug.LogError($"Failed to load remote data: {responseJson}"); // responseJson holds error here
                    // Handle specific errors? e.g., 404 might mean create default data?
                    callback?.Invoke(false, null);
                }
            }));
        } else {
            LoadLocalGuestData(); // Load from PlayerPrefs
            callback?.Invoke(true, CurrentUserData); // Loading local is considered synchronous success
        }
    }

     // Save data - Decides based on login state
    public void SaveData(Action<bool> callback) {
         if (CurrentUserData == null) {
             Debug.LogWarning("SaveData called but CurrentUserData is null.");
             callback?.Invoke(false);
             return;
         }

        if (IsLoggedIn) {
            // Prepare data payload (convert UserData object to JSON string)
            // Need a custom approach if JsonUtility doesn't handle nullable best_time well
             string jsonData = JsonHelper.ToJsonString(CurrentUserData); // Use helper
             Debug.Log($"Saving remote data: {jsonData}");

            StartCoroutine(PutRequest("/api/gamedata", jsonData, apiToken, (success, responseJson) => {
                if(success) {
                    Debug.Log("Remote data saved successfully.");
                    callback?.Invoke(true);
                } else {
                     Debug.LogError($"Failed to save remote data: {responseJson}");
                     callback?.Invoke(false);
                }
            }));
        } else {
            SaveLocalGuestData(); // Save to PlayerPrefs
            callback?.Invoke(true); // Saving local is considered synchronous success
        }
    }

    // Reset ACTIVE data - Decides based on login state
    public void ResetActiveData(Action<bool> callback) {
        if (CurrentUserData != null) {
            CurrentUserData.ResetActiveState(); // Reset the C# object state first
        } else {
             Debug.LogWarning("ResetActiveData called but CurrentUserData is null. Creating default guest.");
             CurrentUserData = new UserData(); // Create default guest data
        }


        if (IsLoggedIn) {
             StartCoroutine(PutRequest("/api/gamedata/reset", null, apiToken, (success, responseJson) => {
                 if (success) {
                     Debug.Log("Remote active data reset successfully.");
                     callback?.Invoke(true);
                 } else {
                     Debug.LogError($"Failed to reset remote active data: {responseJson}");
                     callback?.Invoke(false);
                 }
             }));
        } else {
            // Reset local active data by saving the reset CurrentUserData object
            SaveLocalGuestData();
            callback?.Invoke(true);
        }
    }


    // --- PlayerPrefs Handling (Guest Mode) ---

    private void SaveLocalGuestData() {
        if (CurrentUserData == null) {
             Debug.LogWarning("Tried to save local data, but CurrentUserData is null.");
             return;
        }
        try {
            // Serialize the UserData object into a JSON string
            string json = JsonHelper.ToJsonString(CurrentUserData); // Use helper
            PlayerPrefs.SetString(PREFS_USER_DATA_GUEST, json);
            PlayerPrefs.Save(); // Good practice to call Save
            Debug.Log($"Local guest data saved. HasActiveGame: {CurrentUserData.has_active_game}");
        } catch (Exception e) {
            Debug.LogError($"Error saving local guest data: {e.Message}");
        }
    }

    private void LoadLocalGuestData() {
        if (PlayerPrefs.HasKey(PREFS_USER_DATA_GUEST)) {
            string json = PlayerPrefs.GetString(PREFS_USER_DATA_GUEST);
            try {
                 CurrentUserData = JsonUtility.FromJson<UserData>(json);
                 // Manually parse minigames status list after main parsing
                 if (CurrentUserData != null) {
                    JsonHelper.ParseMinigamesStatus(json, CurrentUserData); // Use helper
                    Debug.Log($"Local guest data loaded. HasActiveGame: {CurrentUserData.has_active_game}");
                 } else {
                    Debug.LogError("Failed to parse local user data JSON.");
                    CurrentUserData = new UserData(); // Fallback to default guest data
                 }

            } catch (Exception e) {
                Debug.LogError($"Error loading local guest data: {e.Message}");
                CurrentUserData = new UserData(); // Fallback to default guest data
            }
        } else {
            Debug.Log("No local guest data found. Creating default.");
            CurrentUserData = new UserData(); // Create default guest data if none exists
        }
         // Ensure IsLoggedIn is false when dealing with guest data
        IsLoggedIn = false;
        apiToken = null;
    }

    // --- API Call Methods (Signup / Login) ---

    public void Signup(string username, string password, Action<bool, string> callback) {
        // Construct JSON payload
        string jsonData = $"{{\"username\":\"{username}\", \"password\":\"{password}\"}}";
        Debug.Log($"Attempting signup for: {username}");

        StartCoroutine(PostRequest("/api/auth/signup", jsonData, null, (success, responseJson) => {
             ProcessAuthResponse(success, responseJson, callback);
        }));
    }

     public void Login(string username, string password, Action<bool, string> callback) {
        string jsonData = $"{{\"username\":\"{username}\", \"password\":\"{password}\"}}";
         Debug.Log($"Attempting login for: {username}");

         StartCoroutine(PostRequest("/api/auth/login", jsonData, null, (success, responseJson) => {
              ProcessAuthResponse(success, responseJson, callback);
         }));
    }

    // Helper to process login/signup response
    private void ProcessAuthResponse(bool success, string responseJson, Action<bool, string> callback) {
         if (success) {
                try {
                    AuthResponse authResponse = JsonUtility.FromJson<AuthResponse>(responseJson);
                    if (authResponse != null && !string.IsNullOrEmpty(authResponse.token)) {
                        SetLoggedIn(authResponse); // Handle successful login/signup
                        callback?.Invoke(true, authResponse.message ?? "Success");
                    } else {
                        Debug.LogError("Auth response JSON parsing failed or token missing.");
                        callback?.Invoke(false, "Invalid response from server.");
                    }
                } catch (Exception e) {
                    Debug.LogError($"Error parsing auth JSON: {e.Message}\nJSON: {responseJson}");
                    callback?.Invoke(false, "Error processing server response.");
                }
            } else {
                 // Try to parse error message from backend
                 string errorMessage = "Login/Signup failed."; // Default
                 try {
                     MessageResponse errorResponse = JsonUtility.FromJson<MessageResponse>(responseJson);
                     if(errorResponse != null && !string.IsNullOrEmpty(errorResponse.message)) {
                         errorMessage = errorResponse.message;
                     } else if (!string.IsNullOrEmpty(responseJson)) {
                         // If parsing fails, use the raw error string if available
                         errorMessage = responseJson;
                     }
                 } catch { /* Ignore parsing errors for error messages */ }

                Debug.LogError($"Auth failed: {errorMessage}");
                callback?.Invoke(false, errorMessage);
            }
    }

    // --- Generic Web Request Coroutines ---

    // For POST requests (Login, Signup)
    private IEnumerator PostRequest(string path, string jsonData, string token, Action<bool, string> callback) {
        string url = backendUrl + path;
        using (UnityWebRequest request = new UnityWebRequest(url, "POST")) {
            if (!string.IsNullOrEmpty(jsonData)) {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(token)) {
                request.SetRequestHeader("Authorization", $"Bearer {token}");
            }

            // Send the request and wait for a response
            yield return request.SendWebRequest();

            // Process response
            if (request.result == UnityWebRequest.Result.Success) {
                callback?.Invoke(true, request.downloadHandler.text);
            } else {
                Debug.LogError($"POST Error to {url}: {request.error} | Response: {request.downloadHandler.text}");
                callback?.Invoke(false, request.downloadHandler.text ?? request.error); // Send back error msg or code
            }
        }
    }

     // For PUT requests (Update Data, Reset Data)
    private IEnumerator PutRequest(string path, string jsonData, string token, Action<bool, string> callback) {
        string url = backendUrl + path;
        using (UnityWebRequest request = new UnityWebRequest(url, "PUT")) {
             if (!string.IsNullOrEmpty(jsonData)) { // jsonData can be null for Reset
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
             if (!string.IsNullOrEmpty(token)) { // Token is required for these PUTs
                request.SetRequestHeader("Authorization", $"Bearer {token}");
            } else {
                 Debug.LogError("PUT Request requires authorization token!");
                 callback?.Invoke(false, "Authorization token missing.");
                 yield break; // Stop the coroutine
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                callback?.Invoke(true, request.downloadHandler.text);
            } else {
                Debug.LogError($"PUT Error to {url}: {request.error} | Response: {request.downloadHandler.text}");
                callback?.Invoke(false, request.downloadHandler.text ?? request.error);
            }
        }
    }

    // For GET requests (Load Data)
    private IEnumerator GetRequest(string path, string token, Action<bool, string> callback) {
         string url = backendUrl + path;
        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            if (!string.IsNullOrEmpty(token)) { // Token required for getting game data
                 request.SetRequestHeader("Authorization", $"Bearer {token}");
            } else {
                 Debug.LogError("GET Request requires authorization token!");
                 callback?.Invoke(false, "Authorization token missing.");
                 yield break; // Stop the coroutine
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                callback?.Invoke(true, request.downloadHandler.text);
            } else {
                 Debug.LogError($"GET Error from {url}: {request.error} | Response: {request.downloadHandler.text}");
                callback?.Invoke(false, request.downloadHandler.text ?? request.error);
            }
        }
    }
}

// --- JSON Helper Class ---
// Needed because JsonUtility is limited (e.g., parsing root arrays, handling dictionaries)
public static class JsonHelper
{
    // Helper to manually parse the minigames_status list from JSON string
    // This assumes the JSON string for the UserData object has been fetched
    public static void ParseMinigamesStatus(string json, UserData userData)
    {
        try {
            // Find the key "minigames_status" or "minigames_status_array" in the raw JSON
            // This is a bit hacky, a full JSON parser like Newtonsoft.Json would be more robust
            string key = "\"minigames_status\":\""; // Look for the string version first
            int startIndex = json.IndexOf(key);
            char endChar;
            if (startIndex == -1) {
                 // Fallback if backend sends array directly (less likely with our backend setup)
                 key = "\"minigames_status_array\":";
                 startIndex = json.IndexOf(key);
                  if (startIndex == -1) {
                    Debug.LogWarning("minigames_status field not found in JSON for parsing.");
                    userData.minigames_status_array = new List<int> { 0, 0, 0 }; // Default
                    return;
                 }
                 startIndex += key.Length; // Adjust start index for array
                 endChar = ']';
                 char startChar = '[';
                  if(json[startIndex-1] != startChar) { // Basic check
                       Debug.LogWarning("minigames_status_array doesn't start with '['.");
                       userData.minigames_status_array = new List<int> { 0, 0, 0 }; return;
                  }

            } else {
                startIndex += key.Length; // Move past the key itself
                endChar = '"'; // String ends with quote
            }


            int endIndex = json.IndexOf(endChar, startIndex);
            if (endIndex == -1) {
                Debug.LogWarning("End of minigames_status value not found.");
                userData.minigames_status_array = new List<int> { 0, 0, 0 }; return;
            }

            string statusString = json.Substring(startIndex, endIndex - startIndex);
            // If it was a string field, remove escaping backslashes if any (e.g., "\[0,0,0\]")
            statusString = statusString.Replace("\\", "");

             // Trim brackets and split by comma
            string[] items = statusString.Trim('[', ']').Split(',');
            userData.minigames_status_array = new List<int>();
            foreach (string item in items) {
                if (int.TryParse(item.Trim(), out int value)) {
                    userData.minigames_status_array.Add(value);
                } else {
                     Debug.LogWarning($"Could not parse '{item}' as int in minigames_status.");
                     userData.minigames_status_array.Add(0); // Add default on failure
                }
            }
             // Ensure list has correct size (e.g., 3 elements)
            while (userData.minigames_status_array.Count < 3) userData.minigames_status_array.Add(0);
            while (userData.minigames_status_array.Count > 3) userData.minigames_status_array.RemoveAt(userData.minigames_status_array.Count - 1);


        } catch (Exception e) {
             Debug.LogError($"Error manually parsing minigames_status: {e.Message}");
             userData.minigames_status_array = new List<int> { 0, 0, 0 }; // Fallback
        }
    }

    // Helper to convert UserData to JSON, handling nullable best_time
     public static string ToJsonString(UserData data)
    {
        // JsonUtility doesn't handle nullable fields well, so we might need manual construction
        // or use a different JSON library if this becomes problematic.
        // For now, let's try a basic approach.

        // Convert list to string manually first for insertion
        string statusString = $"[{string.Join(",", data.minigames_status_array)}]";

        // Use string formatting or StringBuilder for more control
        var sb = new System.Text.StringBuilder();
        sb.Append("{");
        sb.Append($"\"id\":{data.id},");
        sb.Append($"\"username\":\"{data.username}\","); // Ensure strings are quoted
        sb.Append($"\"has_active_game\":{data.has_active_game.ToString().ToLower()},"); // bool to lowercase true/false
        sb.Append($"\"current_scene_index\":{data.current_scene_index},");
        sb.Append($"\"player_position_x\":{data.player_position_x},");
        sb.Append($"\"player_position_y\":{data.player_position_y},");
        sb.Append($"\"current_timer\":{data.current_timer},");
        sb.Append($"\"current_oxygen_level\":{data.current_oxygen_level},");
        // Insert the manually created list string - Needs quotes around it for backend JSON parser!
        sb.Append($"\"minigames_status_array\":{statusString},"); // Send as array for backend
        sb.Append($"\"dash_charges\":{data.dash_charges},");
        sb.Append($"\"difficulty_level\":{data.difficulty_level},");
        // Handle nullable best_time
        if (data.best_time.HasValue) {
            sb.Append($"\"best_time\":{data.best_time.Value},");
        } else {
            sb.Append($"\"best_time\":null,");
        }
        sb.Append($"\"high_score\":{data.high_score}");
        // Add is_win_condition and final_time if needed for the PUT request context
        // These aren't stored fields but transient data for the update logic
         /*sb.Append($",\"is_win_condition\":{data.is_win_condition.ToString().ToLower()}"); // Assuming you add these fields temporarily
         if(data.final_time.HasValue) {
            sb.Append($",\"final_time\":{data.final_time.Value}");
         }*/

        sb.Append("}");
        return sb.ToString();

        // Simple JsonUtility version (might fail with list or nullables):
        // return JsonUtility.ToJson(data);
    }

    // Add temporary fields to UserData class if needed for ToJsonString helper
    [System.NonSerialized] public static bool is_win_condition = false;
    [System.NonSerialized] public static float? final_time = null;

}