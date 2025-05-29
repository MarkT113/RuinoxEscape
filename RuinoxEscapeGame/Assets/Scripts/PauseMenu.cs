using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void QuitToHome()
    {
        // Save data (i.e. player position, scene/level number, time remaining, oxygen, dash attempts, .......)
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void MainMap()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartLevel()
    {
        // Get/keep/retreieve/save/take the: time, oxygen, dash, ..........
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
