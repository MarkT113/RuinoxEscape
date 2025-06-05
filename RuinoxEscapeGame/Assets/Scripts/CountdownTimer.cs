using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    //public GameObject winScreen;
    public float timeRemaining;
    public bool isTimerPaused;
    public static CountdownTimer Instance {get; private set;}

    void Start()
    {
        if (Instance == null)
            Instance = this;
        timeRemaining = GameData.currentTimer;
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
                //ResetTimer();
                //OnTimerEnd();
            }
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
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
    
    private void OnTimerEnd()
    {
        // Similar to quitting the game But you reset everything!
        /*winScreen.SetActive(true);
        SceneManager.LoadScene(0);
        winScreen.SetActive(false);*/
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}