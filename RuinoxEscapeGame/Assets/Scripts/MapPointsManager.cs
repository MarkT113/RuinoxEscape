using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class MapPointsManager : MonoBehaviour
{
    public GameObject pinParentObject;
    public float detectionRadius;
    public SpriteRenderer gameMap;
    public SpriteRenderer player;

    /* Although there is only one map points manager in the scene (and entire game!) and it may seem overkill,
    I'm still using a singleton pattern to ensure there exists only one instance of this class. However,
    this can make it harder to test/mock and lead to tight coupling (always assuming game manager exists,
    other scripts are depend on the singleton instance).*/
    public static MapPointsManager Instance {get; private set;}
    
    public static event Action OnPositionsInitialized; // Can remove static, but must change other scripts.

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        /* Decided against making this game object persistent across scenes; very slightly better efficiency.
        Downside being - obviously, as with anything else - having to reload it every time the main map is opened*/
        //DontDestroyOnLoad(gameObject);
        generatePositions();
        OnPositionsInitialized?.Invoke();
    }

    void generatePositions()
    {
        if (!GameData.hasSpawnedPins)
        {
            List<Vector2> usedPositions = new List<Vector2>();
            float halfW = GetCombinedWidth(pinParentObject) / 2;
            float halfH = GetCombinedHeight(pinParentObject) / 2;
            for (int i = 0; i < 4; i++)
            {
                Vector2 spawnPos;
                do
                {
                    float x = Random.Range(gameMap.bounds.min.x + halfW, gameMap.bounds.max.x - halfW);
                    float y = Random.Range(gameMap.bounds.min.y + halfH, gameMap.bounds.max.y - halfH);
                    spawnPos = new Vector2(x, y);
                }
                while (!checkPositionValidity(spawnPos, usedPositions));
                usedPositions.Add(spawnPos);
            }
            GameData.SavePinPositions(usedPositions);
            GameData.hasSpawnedPins = true;
        }
    }

    bool checkPositionValidity(Vector2 pos, List<Vector2> existing)
    {
        foreach (var e in existing)
        {
            float minX = 2 * detectionRadius + player.bounds.size.x + 0.1f;
            float minY = 2 * detectionRadius + player.bounds.size.y + 0.1f;

            if (Mathf.Abs(pos.x - e.x) < minX && Mathf.Abs(pos.y - e.y) < minY)
                return false;
        }
        return true;
    }

    float GetCombinedWidth(GameObject obj)
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        foreach (var r in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            float left = r.bounds.min.x;
            float right = r.bounds.max.x;
            if (left < minX) minX = left;
            if (right > maxX) maxX = right;
        }
        return maxX - minX;
    }

    float GetCombinedHeight(GameObject obj)
    {
        float minY = float.MaxValue, maxY = float.MinValue;
        foreach (var r in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            float bottom = r.bounds.min.y;
            float top = r.bounds.max.y;
            if (bottom < minY) minY = bottom;
            if (top > maxY) maxY = top;
        }
        return maxY - minY;
    }
}