using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : MonoBehaviour
{
    /*public float dashesRemaining;
    public static DashController Instance {get; private set;}

    void Start()
    {
        if (Instance == null)
            Instance = this;
        dashesRemaining = GameData.dashChargesRemaining;
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
    }*/
    
    private void OnTimerEnd()
    {
        // Similar to quitting the game But you reset everything!
        /*winScreen.SetActive(true);
        SceneManager.LoadScene(0);
        winScreen.SetActive(false);*/
    }
}