using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelPin
    {
        public int levelScene;
        public GameObject pinObject; // Empty GO holding the two sprites
        public Animator arrowAnimator;
        public Vector2 position;
        public float detectionRadius;
    }

    public List<LevelPin> levelPins;
    public Rect mapBounds; // Set in Inspector: (x, y, width, height)
    public float playerWidth = 1f;
    public float playerHeight = 1f;
    public GameObject player;
    public Button uiButton;

    private int[] levelStatus => GameData.minigamesStatus;
    private bool isFirstLevelRevealed => GameData.isFirstLevelRevealed;

    void Start()
    {
        SetupPins();
    }

    void Update()
    {
        CheckProximity();
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
                    float halfW = GetCombinedWidth(pin.pinObject) / 2;
                    float halfH = GetCombinedHeight(pin.pinObject) / 2;

                    float x = Random.Range(mapBounds.xMin + halfW, mapBounds.xMax - halfW);
                    float y = Random.Range(mapBounds.yMin + halfH, mapBounds.yMax - halfH);
                    spawnPos = new Vector2(x, y);
                }
                while (!IsFarEnough(spawnPos, usedPositions, pin.detectionRadius));

                pin.pinObject.transform.position = spawnPos;
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
                    levelPins[i].pinObject.SetActive(false);
                }
            }
            else
            {
                ShowHalftoneDots(levelPins[i], true);
                ShowArrow(levelPins[i], true);
            }
        }
    }

    void CheckProximity()
    {
        float minDist = float.MaxValue;
        LevelPin nearestPin = null;

        foreach (var pin in levelPins)
        {
            if (levelStatus[GetPinIndex(pin)] == 1) continue; // skip completed

            float dist = Vector2.Distance(player.transform.position, pin.pinObject.transform.position);
            if (dist <= pin.detectionRadius && dist < minDist)
            {
                minDist = dist;
                nearestPin = pin;
            }
        }

        if (nearestPin != null)
        {
            EnableButton(true, nearestPin);
        }
        else
        {
            EnableButton(false, null);
        }
    }

    void EnableButton(bool enable, LevelPin pin)
    {
        ColorBlock colors = uiButton.colors;
        colors.normalColor = enable ? Color.white : Color.gray;
        uiButton.colors = colors;

        uiButton.onClick.RemoveAllListeners();

        if (enable)
        {
            uiButton.onClick.AddListener(() => OnLevelButtonClick(pin));
        }
    }

    void OnLevelButtonClick(LevelPin pin)
    {
        int index = GetPinIndex(pin);

        if (!isFirstLevelRevealed)
        {
            RevealAllPins();
            GameData.isFirstLevelRevealed = true;
        }

        SceneManager.LoadScene(pin.levelScene);
    }

    void RevealAllPins()
    {
        foreach (var pin in levelPins)
        {
            pin.pinObject.SetActive(true);
            ShowHalftoneDots(pin, true);
            PlayArrowPopAnimation(pin);
        }
    }

    bool IsFarEnough(Vector2 pos, List<Vector2> existing, float radius)
    {
        foreach (var e in existing)
        {
            float minX = 2 * radius + playerWidth + Random.Range(0.1f, 0.5f);
            float minY = 2 * radius + playerHeight + Random.Range(0.1f, 0.5f);

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

    int GetPinIndex(LevelPin pin)
    {
        return levelPins.IndexOf(pin);
    }

    void ShowHalftoneDots(LevelPin pin, bool show)
    {
        // Assume you tagged the dots child or can reference it
        Transform dots = pin.pinObject.transform.Find("HalftoneDots");
        if (dots != null) dots.gameObject.SetActive(show);
    }

    void ShowArrow(LevelPin pin, bool show)
    {
        Transform arrow = pin.pinObject.transform.Find("MapArrow");
        if (arrow != null) arrow.gameObject.SetActive(show);
    }

    void PlayArrowPopAnimation(LevelPin pin)
    {
        if (pin.arrowAnimator != null)
        {
            pin.arrowAnimator.Play("PopAnimation");
        }
    }
}
