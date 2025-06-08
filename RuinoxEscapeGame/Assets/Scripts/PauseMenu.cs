using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance {get; private set;}

    void Awake()
    {
        Instance = Instance ? this :  null;
    }
    
    public void Pause()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void QuitToHome()
    {
        // Save data (i.e. player position, active scene/level number,
        // time remaining, oxygen, dash attempts, .......)
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void QuitToMainMap()
    {
        // Save data (i.e. player position, scene-related data
        // [e.g. dash attempts remaining or health level or enemy damage or ...], .......)
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void Resume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartLevel()
    {
        // Get/keep/retreieve/save/take the: time, oxygen, dash, ..........
        GameData.currentTimer = CountdownTimer.Instance.timeRemaining;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        // Reset all data (..........)
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }
}
