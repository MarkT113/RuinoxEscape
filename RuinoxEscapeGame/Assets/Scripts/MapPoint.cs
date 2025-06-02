using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapPoint : MonoBehaviour
{
    public int levelScene; // Scene number of minigame/level
    public GameObject locationObject; // Empty game object holding the two sprites
    public Animator pinAnimator; // Animation for the map pin
    public int indexInPosLst; // Or can set it to levelScene - 2
    public float detectionRadius;
    public GameObject player;
    public Button uiButton;

    private int[] levelStatus => GameData.minigamesStatus;
    private bool isFirstLevelRevealed => GameData.isFirstLevelRevealed;

    void Start()
    {
        // Workaround to avoid errors and glitches due to random execution of scripts
        MapPointsManager.OnPositionsInitialized += SetupPin;
    }

    void Update()
    {
        CheckProximity();
    }

    void SetupPin()
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
                levelPins[i].locationObject.SetActive(false);
            }
        }
        else
        {
            ShowHalftoneDots(levelPins[i], true);
            ShowArrow(levelPins[i], true);
        }
    }

    void CheckProximity()
    {
        float minDist = float.MaxValue;
        LevelPin nearestPin = null;

        foreach (var pin in levelPins)
        {
            if (levelStatus[GetPinIndex(pin)] == 1) continue; // skip completed

            float dist = Vector2.Distance(player.transform.position, pin.locationObject.transform.position);
            if (dist <= detectionRadius && dist < minDist)
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
            pin.locationObject.SetActive(true);
            ShowHalftoneDots(pin, true);
            PlayArrowPopAnimation(pin);
        }
    }

    bool IsFarEnough(Vector2 pos, List<Vector2> existing)
    {
        foreach (var e in existing)
        {
            float minX = 2 * detectionRadius + playerWidth + 0.1f;
            float minY = 2 * detectionRadius + playerHeight + 0.1f;

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
        // Referencing the dots child sprite using tags
        Transform dots = pin.locationObject.transform.Find("HalftoneDots");
        if (dots != null) dots.gameObject.SetActive(show); // Simple check / error handling
    }

    void ShowArrow(LevelPin pin, bool show)
    {
        Transform arrow = pin.locationObject.transform.Find("MapArrow");
        if (arrow != null) arrow.gameObject.SetActive(show);
    }

    void PlayArrowPopAnimation(LevelPin pin)
    {
        if (pin.pinAnimator != null)
        {
            pin.pinAnimator.Play("PopAnimation");
        }
    }
}