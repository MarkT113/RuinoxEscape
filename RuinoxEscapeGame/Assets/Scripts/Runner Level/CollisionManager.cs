using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CollisionManager : MonoBehaviour
{
    public int maxDashCharges = 3; // Maximum number of dashes available (in the entire game)
    public int dashChargesDiscovered = 3;
    public int dashChargesLeft = 3; // Current number of dashes remaining
    public float dashDuration = 3f;
    public float dashCooldown = 5f;
    public GameObject obstacleSpawnerPoint;
    public bool isDead {get; private set;}
    public static CollisionManager Instance {get; private set;}

    private Animator playerAnimator;
    private float dashTimeLeft; // Time left for the current dash to complete
    private float cooldownTimeLeft; // Time remaining out of the cooldown
    private bool isDashActive; // Is the player currently dashing
    
    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private void Start()
    {
        dashChargesLeft = maxDashCharges; // Initialise dashes
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead) return;
        /* If the player's dash ability is currently activated, then we will begin to decrease/decrement 'dashTimeLeft';
         ensuring overall that the dash lasts for 'dashDuration' seconds */
        if (isDashActive)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0) EndDash();
            return; // As there is no reason to further check anything else
            // (can't do any other action while the player is in dash mode)
        }
        /* Decrementing cooldown time; ensuring 'dashCooldown' seconds are passed before the
        dash ability can be used again (assuming all other conditions are met/satisfied) */
        if (cooldownTimeLeft > 0) cooldownTimeLeft -= Time.deltaTime;
        // Dash input - kept for testing
        if (Input.GetKeyDown(KeyCode.D) && dashChargesLeft > 0 && cooldownTimeLeft <= 0) StartDash();
    }

    public void onDashButtonPressed()
    {
        if (isDead) return;
        if (!isDashActive && dashChargesLeft > 0 && cooldownTimeLeft <= 0) StartDash();
    }
    
    private void StartDash()
    {
        isDashActive = true;
        dashTimeLeft = dashDuration;
        cooldownTimeLeft = dashCooldown;
        dashChargesLeft--;
        playerAnimator.SetBool("isDashing", true);
    }

    private void EndDash()
    {
        isDashActive = false;
        playerAnimator.SetBool("isDashing", false);
    }

    public void ResetDashes()
    {
        dashChargesLeft = maxDashCharges;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDashActive) other.GetComponent<Animator>().SetBool("break", true);
        else DeathRoutine();
    }

    private void DeathRoutine()
    {
        //this.enabled = false; // Redundant, disables the script. Likewise, 'Time.timeScale = 0'.
        CountdownTimer.Instance.PauseTimer();
        isDead = true; // To prevent anything else from running
        //playerAnimator.SetBool("die", true); // Alternatively, SetTrigger() can be used (since it is a one-time event)
        CameraMovement.cameraSpeed = 0;
        BackgroundMovement.speed = 0;
        obstacleSpawnerPoint.SetActive(false);
        Button[] allSceneButtons = FindObjectsOfType<Button>();
        foreach (Button btn in allSceneButtons)
        {
            btn.interactable = false;
            EventTrigger touchInput = btn.GetComponent<EventTrigger>();
            if (touchInput) touchInput.enabled = false;
        }
        //yield return new WaitForSeconds(6f); // small buffer (wait until animation finishes + time before resetting
                                             // level). Unconventional but efficient (instead of having to find out
                                             // how long the animation clip is)
        //playerAnimator.SetTrigger("die"); // Best method for this case/application as it allows for triggering
                                          // events after / at the end of the animation.
        //playerAnimator.SetBool("isDead", true);
        playerAnimator.Play("Die_N");
    }

    private void ResetScene()
    {
        PauseMenu.Instance.RestartLevel();
    }
}