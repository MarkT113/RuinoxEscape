// using UnityEngine;
// using UnityEngine.Networking;
// using System.Collections;
// using TMPro;
// using System;
//
// public class NetworkManager : MonoBehaviour
// {
//     public TMP_InputField usernameInput;
//     public TMP_InputField emailInput;
//     public TMP_Text resultText;
//     //public static NetworkManager Instance { get; private set; }
//     
//     private static NetworkManager _instance;
//     private readonly string serverUrl = "http://localhost:3000"; // Change to local IP for mobile testing
//
//     public static NetworkManager Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 GameObject obj = new GameObject("NetworkManager");
//                 _instance = obj.AddComponent<NetworkManager>();
//                 DontDestroyOnLoad(obj);
//             }
//             return _instance;
//         }
//     }
//     
//     /*void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject); // Keep across scenes
//         }
//         else
//             Destroy(gameObject); // Prevent duplicates
//     }*/
//
//     public void RegisterUser()
//     {
//         string username = usernameInput.text;
//         string email = emailInput.text;
//
//         StartCoroutine(Instance.RegisterUser(username, email, (response) =>
//         {
//             /*if (response)
//             {
//                 resultText.text = "Register Success";
//                 Debug.Log("Register Success");
//             }
//             else
//             {
//                 resultText.text = "Register Failed";
//                 Debug.Log("Register Failed");
//             }*/
//             resultText.text = "Register Success";
//             Debug.Log("Register Success");
//         }));
//     }
//     
//     // Register a new user
//     public IEnumerator RegisterUser(string username, string email, Action<string> callback)
//     {
//         string jsonData = $"{{\"username\":\"{username}\", \"email\":\"{email}\"}}";
//         //string jsonData = $"username: \"{username}\", email: \"{email}\"";
//         // Communicating with the server using UnityWebRequest
//         using (UnityWebRequest request = new UnityWebRequest(serverUrl + "/register", "POST"))
//         {
//             byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
//             request.uploadHandler = new UploadHandlerRaw(bodyRaw);
//             request.downloadHandler = new DownloadHandlerBuffer();
//             request.SetRequestHeader("Content-Type", "application/json");
//             yield return request.SendWebRequest();
//             callback(request.result == UnityWebRequest.Result.Success ? request.downloadHandler.text : request.error);
//             //callback(request.result == UnityWebRequest.Result.Success);
//         }
//     }
// }

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System;

public class NetworkManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField ageInput;
    public TMP_Text resultText;

    private static NetworkManager _instance;
    private readonly string serverUrl = "http://localhost:3000";

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("NetworkManager");
                _instance = obj.AddComponent<NetworkManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    // Called when Register button is clicked
    public void RegisterUser()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;
        string age = ageInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(age))
        {
            resultText.text = "Please fill in all fields.";
            return;
        }

        StartCoroutine(RegisterUserCoroutine(username, email, password, age, (response) =>
        {
            resultText.text = "Register Response: " + response;
            Debug.Log("Register: " + response);
        }));
    }

    public IEnumerator RegisterUserCoroutine(string username, string email, string password, string age, Action<string> callback)
    {
        string jsonData = JsonUtility.ToJson(new RegisterRequest { username = username, email = email, password = password, age = age });

        using (UnityWebRequest request = new UnityWebRequest(serverUrl + "/register2", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            string result = request.result == UnityWebRequest.Result.Success
                ? request.downloadHandler.text
                : request.error;

            callback(result);
        }
    }

    // Called when Login button is clicked
    public void LoginUser()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            resultText.text = "Enter both username and password.";
            return;
        }

        StartCoroutine(LoginUserCoroutine(username, password, (response) =>
        {
            resultText.text = "Login Response: " + response;
            Debug.Log("Login: " + response);
        }));
    }

    public IEnumerator LoginUserCoroutine(string username, string password, Action<string> callback)
    {
        string jsonData = JsonUtility.ToJson(new LoginRequest { username = username, password = password });

        using (UnityWebRequest request = new UnityWebRequest(serverUrl + "/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            string result = request.result == UnityWebRequest.Result.Success
                ? request.downloadHandler.text
                : request.error;

            callback(result);
        }
    }

    [Serializable]
    private class RegisterRequest
    {
        public string username;
        public string email;
        public string password;
        public string age;
    }

    [Serializable]
    private class LoginRequest
    {
        public string username;
        public string password;
    }
}