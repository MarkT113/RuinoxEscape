using System;
using UnityEngine;
using System.Collections.Generic;

public class GameData
{
    /*private static float currentTime = 570f;
    private static int currentScore = 1436;
    private static bool isNewGame = false;
    private static int currentGameScene = 1;
    private static int[] currentPlayerPosition;
    private static int currentOxygenLevel = 82;
    private static int difficulty = 2;
    private static int bestGameDifficulty = 1;
    private static float bestTime = 30f;
    private static int highScore = 12803812;
    
    private static readonly GameData instance = new GameData();
    public static GameData Instance => instance;

    private GameData()
    {
        // Read from database/server or locally/shared-pref
    }*/
    
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
    public static int dashCharges = 3; // Number of dash attemps remaining
    public static int difficultyLevel = 3; // 1 = Easy, 2 = Medium, 3 = Hard
    public static readonly Dictionary<int, float> difficultyTimes = new Dictionary<int, float> {{1, 360}, {2, 240}, {3, 120}}; // 1 = 6min, 2 = 4min, 3 = 2min
    public static float? bestTime = null;
    public static int highScore = 0;


    public static bool hasSpawnedPins;
    public static bool isFirstLevelRevealed;
    public static List<Vector2> pinPositions = new List<Vector2>(); // Get saved data
    
    public static void LoadPinPositions(List<MapPointsManager.LevelPin> positions)
    {
        for(int i = 0; i < 4; i++)
        {
            positions[i].position = pinPositions[i];
        }
    }

    public static void SavePinPositions(List<MapPointsManager.LevelPin> positions)
    {
        foreach (var pos in positions)
        {
            pinPositions.Add(pos.position);
        }
    }

    public void saveData(int sceneNum, float  posX, float posY, float timeRemaining, int oxLevel, int oyLevel)
    {
        currentSceneIndex = sceneNum;
        playerPositionX = posX;
        playerPositionY = posY;
        currentTimer = timeRemaining;
        currentOxygenLevel = oxLevel;
        //dashCharges = 3;
    }

    // For 'Lose / Game Over'
    public void resetData()
    {
        hasActiveGame = false;
        currentSceneIndex = 1;
        playerPositionX = 0f;
        playerPositionY = 0f;
        currentTimer = difficultyTimes[difficultyLevel];
        currentOxygenLevel = 100;
        isFirstLevelRevealed = false;
        Array.Clear(minigamesStatus, 0,  minigamesStatus.Length);
        dashCharges = 3;
    }
}
