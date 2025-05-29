using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMove : MonoBehaviour
{
    /*public CharacterController2D characterController;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    bool jump = false;
    public Animator animator;

    private bool moveLeft = false;
    private bool moveRight = false;

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (moveLeft) horizontalMove = -runSpeed;
        else if (moveRight) horizontalMove = runSpeed;
        animator.SetFloat("runSpeed", Mathf.Abs(horizontalMove));
        if (Input.GetButtonDown("Jump")) Jump();
    }
    
    private void FixedUpdate()
    {
        characterController.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    public void Jump()
    {
        if (!jump)
        {
            jump = true;
            animator.SetBool("jump", true);
        }
    }

    public void OnLanding()
    {
        animator.SetBool("jump", false);
    }

    public void MoveLeft()
    {
        moveLeft = true;
    }
    
    public void MoveRight()
    {
        moveRight = true;
    }
    
    public void StopMoving()
    {
        moveLeft = false;
        moveRight = false;
        horizontalMove = 0;
    }*/
}