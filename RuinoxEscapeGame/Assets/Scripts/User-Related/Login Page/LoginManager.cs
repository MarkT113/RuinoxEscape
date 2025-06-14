/*using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text resultText;
    
    private static NetworkManager _instance;
    private readonly string serverUrl = "http://localhost:3000";

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("LoginManager");
                _instance = obj.AddComponent<LoginManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public void LoginUser()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        StartCoroutine(Instance.RegisterUser(username, password, (response) =>
        {
            resultText.text = "Register Success";
            Debug.Log("Register Success");
        }));
    }
    
    // Register a new user
    public IEnumerator LoginUser(string username, string password, Action<string> callback)
    {
        string jsonData = $"{{\"username\":\"{username}\", \"password\":\"{password}\"}}";
        using (UnityWebRequest request = new UnityWebRequest(serverUrl + "/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            callback(request.result == UnityWebRequest.Result.Success ? request.downloadHandler.text : request.error);
            //callback(request.result == UnityWebRequest.Result.Success);
        }
    }
}*/