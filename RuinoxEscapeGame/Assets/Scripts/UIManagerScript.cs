using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance {get; private set;}
    
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
    {
        SceneManager.LoadScene(0);
        // Get objects from loaded scene
    }

    public void PauseGame()
    {
        
    }

    public void RestartGame()
    {
        
    }

    public void QuitGame()
    {
        
    }
}
