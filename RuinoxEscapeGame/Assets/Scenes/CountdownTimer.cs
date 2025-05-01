using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private float timeRemaining = 60f;
    private bool isTimerPaused = true;
    private bool isTimerVisible;
    
    public static CountdownTimer Instance {get; private set;}
    public TextMeshProUGUI timerText;
    public GameObject winScreen;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // timeRemaining = .....       based on game difficulty or saved data
        timerText.color = Color.green;
    }

    void Update()
    {
        if(!isTimerPaused)
        {
            if(timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                timerText.color = Color.red;
                ResetTimer();
                OnTimerEnd();
            }
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public void PauseTimer()
    {
        isTimerPaused = true;
    }

    public void ResumeTimer()
    {
        isTimerPaused = false;
    }

    public void ResetTimer()
    {
        timeRemaining = 600f; // Set depending on game difficulty
        isTimerPaused = true;
        isTimerVisible = false;
    }
    
    private void OnTimerEnd()
    {
        winScreen.SetActive(true);
        SceneManager.LoadScene(0);
        winScreen.SetActive(false);
    }

    public void SetTimerVisibility(bool selectedVisibility)
    {
        isTimerVisible = selectedVisibility;
        timerText.gameObject.SetActive(selectedVisibility);
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}