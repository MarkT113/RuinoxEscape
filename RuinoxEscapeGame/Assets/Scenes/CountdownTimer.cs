using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining;
    public bool timerPaused = true;
    public TextMeshProUGUI timerText;
   
    void Start()
    {
        timerPaused = false;
    }

    void Update()
    {
        if(!timerPaused)
        {
            if(timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                timerText.color = Color.red;
                // Game over ---> back to main screen + start new game?
            }
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            /*if ()
            {
                timerText.text = "Timer: " + timeRemaining.ToString();
            }*/
        }
    }

    private void Awake()
    {
        throw new NotImplementedException();
    }

    public void PauseTimer()
    {
        timerPaused = true;
    }

    public void ResumeTimer()
    {
        timerPaused = false;
    }

    public void ResetTimer()
    {
        timeRemaining = 600f;
    }
}