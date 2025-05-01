using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OxygenController : MonoBehaviour
{
    private static OxygenController instance;
    public TMP_Text oxygenText;
    public int currentOxygenLevel = 100;
    
    public static OxygenController Instance
    {
        get => instance; set => instance = value;
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        currentOxygenLevel = PlayerPrefs.GetInt("OxygenLevel", 100);
        UpdateOxygenText();
    }

    public void IncreaseOxygen(int value)
    {
        currentOxygenLevel += value;
        PlayerPrefs.SetInt("OxygenLevel", currentOxygenLevel);
        PlayerPrefs.Save();
        UpdateOxygenText();
    }

    public void UpdateOxygenText()
    {
        if (oxygenText == null) FindOxygenText();
        if (oxygenText != null) oxygenText.text = "Oxygen: " + currentOxygenLevel.ToString();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindOxygenText();
        UpdateOxygenText();
    }

    private void FindOxygenText()
    {
        GameObject textObject = GameObject.FindWithTag("OxygenText");
        if (textObject != null) oxygenText = textObject.GetComponent<TMP_Text>();
    }
}
