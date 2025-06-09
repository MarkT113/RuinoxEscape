using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerLevelData : MonoBehaviour
{
    public static int id;
    public static string username = "Mark";
    public static bool hasActiveGame = false;
    public static int currentSceneIndex = 1;
    public static float playerPositionX = 0f;
    public static float playerPositionY = 0f;
    public static float platformerPositionX = 0f;
    public static float platformerPositionY = 0f;
    public static float runnerPositionX = 0f;
    public static float runnerPositionY = 0f;
    public static float combatPositionX = 0f;
    public static float combatPositionY = 0f;
    public static float shooterPositionX = 0f;
    public static float shooterPositionY = 0f;
    public static float currentTimer = 120f;
    public static int currentOxygenLevel = 100;
    public static int[] minigamesStatus = {0, 0, 0}; // Array to check for successfully completed levels
    public static int difficultyLevel = 3; // 1 = Easy, 2 = Medium, 3 = Hard
    public static readonly Dictionary<int, float> difficultyTimes = new Dictionary<int, float> {{1, 360}, {2, 240}, {3, 120}}; // 1 = 6min, 2 = 4min, 3 = 2min
    public static float? bestTime = null;
    public static int highScore = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
