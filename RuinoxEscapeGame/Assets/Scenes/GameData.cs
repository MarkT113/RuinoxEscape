using System;
using UnityEngine;

public class GameData
{
    private static float currentTime = 570f;
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
    }
}
