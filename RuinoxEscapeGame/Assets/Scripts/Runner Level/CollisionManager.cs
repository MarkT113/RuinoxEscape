using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public int maxDashCharges = 3; // Maximum number of dashes available (in the entire game)
    public int dashChargesDiscovered = 3;
    public int dashChargesLeft = 3; // Current number of dashes remaining
    public float dashDuration = 3f;
    public float dashCooldown = 5f;

    public Animator playerAnimator;
    public string dashAnimTrigger = "Dash";
    public string deathAnimTrigger = "Die";
    
    private float previousDashEndTime = -Mathf.Infinity;
    private float dashTimeLeft; // Time left for the current dash to complete
    private float cooldownTimeLeft; // Time remaining out of the cooldown
    private bool isDashActive; // Is the player currently dashing
    private bool isDead;

    private void Start()
    {
        dashChargesLeft = maxDashCharges; // Initialise dashes
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // If (isDead) return;
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
        if (!isDashActive && dashChargesLeft > 0 && cooldownTimeLeft <= 0) StartDash();
    }
    
    private void StartDash()
    {
        isDashActive = true;
        dashTimeLeft = dashDuration;
        cooldownTimeLeft = dashCooldown;
        dashChargesLeft--;
        playerAnimator.SetBool("dash", true);
    }

    private void EndDash()
    {
        isDashActive = false;
        playerAnimator.SetBool("dash", false);
    }

    public void ResetDashes()
    {
        dashChargesLeft = maxDashCharges;
    }

    private void OnCollisionEnter2D(Collider2D other)
    {
        if (isDead) return;
        if (other.CompareTag("Obstacle"))
        {
            if (isDashActive)
            {
                ObstacleBreakable breakable = other.GetComponent<ObstacleBreakable>();
                if (breakable != null)
                {
                    breakable.Break();
                }
                else
                {
                    Destroy(other.gameObject); // fallback
                }
            }
            else
            {
                StartCoroutine(DeathRoutine());
            }
        }
    }

    private IEnumerator DeathRoutine()
    {
        isDead = true;
        playerAnimator.SetTrigger(deathAnimTrigger);

        // Wait until animation finishes
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        float animLength = playerAnimator.runtimeAnimatorController.animationClips[0].length;

        // Optionally, better to get by name:
        foreach (AnimationClip clip in playerAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Death") // Adjust this to match your animation name
            {
                animLength = clip.length;
                break;
            }
        }

        yield return new WaitForSeconds(animLength + 0.1f); // small buffer

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}