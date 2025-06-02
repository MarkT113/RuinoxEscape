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
    [SerializeField] private Vector2 boxSize = new Vector2(0.55f, 0.001f);
    [SerializeField] private Vector2 offset = new Vector2(0, -0.88f);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private SpriteRenderer gameMap;
    
    private float horizontalMoveInput;
    private float verticalMoveInput;
    private int currentGameLevel;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    private bool isFacingRight;
    private bool moveUp;
    private bool moveDown;
    private bool moveLeft;
    private bool moveRight;
    private Vector2 playerSize;
    private float playerHalfWidth, playerHalfHeight;
    private float minBoundaryX, maxBoundaryX, minBoundaryY, maxBoundaryY;

    void Start()
    {
        currentGameLevel = SceneManager.GetActiveScene().buildIndex;
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // Prevent object/sprite from rotating [... upon collisions and other such things]
        // There must be a gravity force only in the platformer and combat/fighter games s.t. the player falls
        if (currentGameLevel != 2 && currentGameLevel != 4)
        {
            rb.gravityScale = 0f;
        }
        // The following code sets the bounds/limits of the map
        playerSize = GetComponent<SpriteRenderer>().bounds.size;
        playerHalfHeight = playerSize.y / 2;
        playerHalfWidth = playerSize.x / 2;
        var gameMapBounds = gameMap.bounds;
        minBoundaryX = gameMapBounds.min.x + playerHalfWidth;
        maxBoundaryX = gameMapBounds.max.x - playerHalfWidth;
        minBoundaryY = gameMapBounds.min.y + playerHalfHeight;
        maxBoundaryY = gameMapBounds.max.y - playerHalfHeight;
    }

    void Update()
    {
        // Check map boundaries (ensure player does not go out of them)
        float newPosX = Mathf.Clamp(transform.position.x, minBoundaryX, maxBoundaryX);
        float newPosY = Mathf.Clamp(transform.position.y, minBoundaryY, maxBoundaryY);
        transform.position = new Vector2(newPosX, newPosY);
        
        // Keyboard Movement
        horizontalMoveInput = Input.GetAxis("Horizontal");
        if (currentGameLevel == 1 || currentGameLevel == 5)
            verticalMoveInput = Input.GetAxis("Vertical");
        else if (currentGameLevel == 2 || currentGameLevel == 4)
        {
            // Create a box and checks whether it overlaps with any game object that is part of the ground Layer
            // (i.e. is the player character on/touching the ground?)
            isGrounded = Physics2D.OverlapBox((Vector2)transform.position + offset, boxSize, 0, groundLayer);
            // Sets the jump state to true if the player is on the ground and presses the jump key
            // For button equivalent, see onJumpButtonPress() function
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
                isJumping = true;
        }
        
        // Button Movement
        if (moveUp)
        {
            verticalMoveInput = moveSpeed;
        }
        else if (moveDown)
        {
            verticalMoveInput = -moveSpeed;
        }
        if (moveLeft)
        {
            horizontalMoveInput = -moveSpeed;
            /* Else (i.e. moveLeft == false): horizontalMoveInput = 0...... which is already
             automatically implied by the line 'Input.GetAxis("Horizontal")'. Thus, it is vital
             to keep it (don't delete). If removed, the aforementioned code must be implemented.
             Same case applies for vertical movement (and its corresponding line 'Input.GetAxis("Vertical")'). */
        }
        else if (moveRight)
        {
            horizontalMoveInput = moveSpeed;
        }
    }

    private void FixedUpdate()
    {
        if (currentGameLevel == 1 || currentGameLevel == 5)
            rb.velocity = new Vector2(horizontalMoveInput * moveSpeed, verticalMoveInput * moveSpeed);
        else
        {
            rb.velocity = new Vector2(horizontalMoveInput * moveSpeed, rb.velocity.y);
            if ((currentGameLevel == 2 || currentGameLevel == 4) && isJumping)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isJumping = false;
            }
        }
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
    
    public void StartMoveUp()
    {
        moveUp = true;
    }
    
    public void EndMoveUp()
    {
        moveUp = false;
    }
    
    public void StartMoveDown()
    {
        moveDown = true;
    }
    
    public void EndMoveDown()
    {
        moveDown = false;
    }

    public void OnJumpButtonPress()
    {
        if (isGrounded) isJumping = true;
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