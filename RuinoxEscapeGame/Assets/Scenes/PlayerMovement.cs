using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Vector2 boxSize = new Vector2(0.2f, 0.001f);
    [SerializeField] private Vector2 offset = new Vector2(0, -0.84f);
    [SerializeField] private LayerMask groundLayer;
    
    private float horizontalMoveInput;
    private float verticalMoveInput;
    private int currentGameLevel;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    
    void Start()
    {
        currentGameLevel = SceneManager.GetActiveScene().buildIndex;
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        if (currentGameLevel != 2)
        {
            rb.gravityScale = 0f;
        }
    }

    void Update()
    {
        horizontalMoveInput = Input.GetAxis("Horizontal");
        if (currentGameLevel == 1 || currentGameLevel == 4)
            verticalMoveInput = Input.GetAxis("Vertical");
        else if (currentGameLevel == 2)
        {
            isGrounded = Physics2D.OverlapBox((Vector2)transform.position + offset, boxSize, 0, groundLayer);
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
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        DrawBox();
    }

    void DrawBox()
    {
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + (Vector3)offset, Quaternion.Euler(0, 0, 0), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}