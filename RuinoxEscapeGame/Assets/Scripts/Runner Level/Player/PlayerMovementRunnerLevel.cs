using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMovementRunnerLevel : MonoBehaviour
{
    [SerializeField] private float sensitivity = 10f; // To mimic GetAxis()
    [SerializeField] private float moveSpeed = 50f;

    private float horizontalMoveInput;
    private Rigidbody2D rb;
    private Animator playerAnimator;
    private bool moveLeft;
    private bool moveRight;
    private int countA, countB, countC, countD, countE;
    private bool finalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 0f;
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        //horizontalMoveInput = Input.GetAxis("Horizontal"); // Keyboard movement (disabled as it causes errors)
        if (CollisionManager.Instance.isDead) return;
        
        if (moveLeft)
            horizontalMoveInput = Mathf.MoveTowards(horizontalMoveInput, -1f, sensitivity * Time.deltaTime);
        else if (moveRight)
            horizontalMoveInput = Mathf.MoveTowards(horizontalMoveInput, 1f, sensitivity * Time.deltaTime);
        else
            horizontalMoveInput = Mathf.MoveTowards(horizontalMoveInput, 0f, sensitivity * Time.deltaTime);
        
        playerAnimator.SetFloat("inputX", horizontalMoveInput);
    }

    private void FixedUpdate()
    {
        if (CollisionManager.Instance.isDead) rb.velocity = new Vector2(0, 0);
        else rb.velocity = new Vector2(horizontalMoveInput * moveSpeed, 0);
    }

    public void StartMoveLeft()
    {
        moveLeft = true;
    }
    
    public void EndMoveLeft()
    {
        moveLeft = false;
    }
    
    public void StartMoveRight()
    {
        moveRight = true;
    }

    public void EndMoveRight()
    {
        moveRight = false;
    }
}
