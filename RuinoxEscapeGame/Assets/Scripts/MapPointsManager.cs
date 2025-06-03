using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapPointsManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelPin
    {
        public int levelNum;
        public GameObject parentPointObject;
        public Animator arrowAnimator;
        public float detectionRadius;
        public Vector2 position;
    }
    
    public List<LevelPin> levelPins;
    public SpriteRenderer mapSize;
    public SpriteRenderer playerSize;
    public GameObject player;

    private int[] levelStatus => GameData.minigamesStatus;
    private bool isFirstLevelRevealed => GameData.isFirstLevelRevealed;

    void Start()
    {
        SetupPins();
    }

    void SetupPins()
    {
        if (!GameData.hasSpawnedPins)
        {
            List<Vector2> usedPositions = new List<Vector2>();
            foreach (var pin in levelPins)
            {
                Vector2 spawnPos;
                do
                {
                    float halfW = GetCombinedWidth(pin.parentPointObject) / 2;
                    float halfH = GetCombinedHeight(pin.parentPointObject) / 2;

                    float x = Random.Range(mapSize.bounds.min.x + halfW, mapSize.bounds.max.x - halfW);
                    float y = Random.Range(mapSize.bounds.min.y + halfH, mapSize.bounds.max.y - halfH);
                    spawnPos = new Vector2(x, y);
                }
                while (!CheckPositionValidity(spawnPos, usedPositions));
                pin.parentPointObject.transform.position = spawnPos;
                pin.position = spawnPos;
                usedPositions.Add(spawnPos);
            }
            GameData.SavePinPositions(levelPins);
            GameData.hasSpawnedPins = true;
        }
        else
        {
            GameData.LoadPinPositions(levelPins);
        }
        ApplyInitialVisibility();
    }

    void ApplyInitialVisibility()
    {
        for (int i = 0; i < levelPins.Count; i++)
        {
            if (!isFirstLevelRevealed)
            {
                if (i == 0) // Only show platformer halftone dots initially
                {
                    ShowHalftoneDots(levelPins[i], true);
                    ShowArrow(levelPins[i], false);
                }
                else
                {
                    levelPins[i].parentPointObject.SetActive(false);
                }
            }
            else
            {
                ShowHalftoneDots(levelPins[i], true);
                ShowArrow(levelPins[i], true);
            }
        }
    }

    public void OnInteractButtonPress()
    {
        StartCoroutine(CheckProximity());
    }
    
    IEnumerator CheckProximity()
    {
        LevelPin nearestPin = null;
        for (int i = 0; i < levelPins.Count; i++)
        {
            float dist = Vector2.Distance(player.transform.position, levelPins[i].parentPointObject.transform.Find("EntranceDots").position);
            if (dist <= levelPins[i].detectionRadius)
            {
                nearestPin = levelPins[i];
            }
        }
        if (nearestPin != null)
        {
            if (!isFirstLevelRevealed)
            {
                if (nearestPin.levelNum == 2)
                {
                    // Reveal all pins
                    foreach (var pin in levelPins)
                    {
                        pin.parentPointObject.SetActive(true);
                        ShowHalftoneDots(pin, true);
                        yield return StartCoroutine(PlayArrowPopAnimation(pin));
                        ShowArrow(pin, true);
                    }
                    GameData.isFirstLevelRevealed = true;
                    SceneManager.LoadScene(nearestPin.levelNum);
                }
            }
            else SceneManager.LoadScene(nearestPin.levelNum);
        }
    }

    bool CheckPositionValidity(Vector2 pos, List<Vector2> existingPositions)
    {
        for (int i = 0; i < existingPositions.Count; i++)
        {
            float minX = 2 * levelPins[i].detectionRadius + playerSize.bounds.size.x + 0.1f;
            float minY = 2 * levelPins[i].detectionRadius + playerSize.bounds.size.y + 0.1f;
            if (Mathf.Abs(pos.x - existingPositions[i].x) < minX && 
                Mathf.Abs(pos.y - existingPositions[i].y) < minY) return false;
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

    void ShowHalftoneDots(LevelPin pin, bool show)
    {
        // referencing tagged dots child sprite
        Transform dots = pin.parentPointObject.transform.Find("EntranceDots"); // Halftone dots
        if (dots != null) dots.gameObject.SetActive(show);
    }

    void ShowArrow(LevelPin pin, bool show)
    {
        Transform arrow = pin.parentPointObject.transform.Find("PreviewPoint"); // Map arrow
        if (arrow != null) arrow.gameObject.GetComponent<SpriteRenderer>().enabled = show;
    }

    IEnumerator PlayArrowPopAnimation(LevelPin pin)
    {
        if (pin.arrowAnimator != null) pin.arrowAnimator.Play("MapPointAnimation");
        yield return null;
    }
}