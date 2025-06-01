using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public event Action<int> OnSceneChange;
    
    public void LoadScreen(int index)
    {
        OnSceneChange?.Invoke(index);
        SceneManager.LoadScene(index);
    }
}
