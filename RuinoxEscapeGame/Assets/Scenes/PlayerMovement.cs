using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float groundCheckRadius = 0.2f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float horizontalMoveInput;
    public float verticalMoveInput;

    private int currentGameLevel;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    
    void Start()
    {
        currentGameLevel = SceneManager.GetActiveScene().buildIndex;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        horizontalMoveInput = Input.GetAxis("Horizontal");
        if (currentGameLevel == 1 || currentGameLevel == 4)
            verticalMoveInput = Input.GetAxis("Vertical");
        else if (currentGameLevel == 2)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
                isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        if (currentGameLevel == 1 || currentGameLevel == 4)
            rb.velocity = new Vector2(horizontalMoveInput * moveSpeed, verticalMoveInput * moveSpeed);
        else if (currentGameLevel == 2)
        {
            rb.velocity = new Vector2(horizontalMoveInput * moveSpeed, rb.velocity.y);
            //if (!isJumping) return;
            if (isJumping)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isJumping = false;
            }
        }
        else
            rb.velocity = new Vector2(horizontalMoveInput * moveSpeed, rb.velocity.y);
    }
}