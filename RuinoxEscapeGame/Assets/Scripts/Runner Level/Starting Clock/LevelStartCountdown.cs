using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelStartCountdown : MonoBehaviour
{
    public GameObject dimOverlay;
    public int countdownFrom = 3; // Number from which to begin/start the countdown
    public float displayDuration = 1f; // Real-time (seconds) between each number
    
    private TextMeshProUGUI countdownText;
    
    void Start()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(CountdownSequence());
    }

    IEnumerator CountdownSequence()
    {
        Time.timeScale = 0;
        dimOverlay.SetActive(true);
        // Disable gameplay (i.e. all buttons)
        Button[] allSceneButtons = FindObjectsOfType<Button>();
        yield return StartCoroutine(DisableAllSceneElements(allSceneButtons, false));
        for (int i = countdownFrom; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(displayDuration);
        }
        countdownText.text = "GO!";
        // Hide UI
        countdownText.enabled = false;
        dimOverlay.SetActive(false);
        Time.timeScale = 1;
        // Enable gameplay
        yield return StartCoroutine(DisableAllSceneElements(allSceneButtons, true));
        Destroy(gameObject); // Optional but recommended
    }

    IEnumerator DisableAllSceneElements(Button[] allButtons, bool isEnabled)
    {
        foreach (Button btn in allButtons)
        {
            btn.interactable = isEnabled;
            EventTrigger touchInput = btn.GetComponent<EventTrigger>();
            if (touchInput) touchInput.enabled = isEnabled;
        }
        yield return null;
    }
}