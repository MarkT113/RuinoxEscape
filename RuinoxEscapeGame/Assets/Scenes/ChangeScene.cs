using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadScreen(int index)
    {
        SceneManager.LoadScene(index);
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
