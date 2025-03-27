using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    //public float jumpForce = 10f;
    //public float groundCheckRadius = 0.2f;
    //public Transform groundCheck;

    private Rigidbody2D rb;
    //private bool isGrounded;

    public float horizontalMoveInput;
    public float verticalMoveInput;
    
    //public LayerMask groundLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        horizontalMoveInput = Input.GetAxisRaw("Horizontal");
        verticalMoveInput = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(horizontalMoveInput * moveSpeed, verticalMoveInput * moveSpeed);

        /*if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);*/
    }
}
