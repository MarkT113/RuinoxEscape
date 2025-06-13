using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1f;

    [Header("Combat")]
    public float attackCooldown = 5f;
    public float attackRange = 5f;
    public GameObject player;
    public int attackDamage = 1;
    public Animator animator;

    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth = 10;

    public Rigidbody2D rb;
    public bool isGrounded;
    public int moveInput;
    public float currentAttackCooldown;
    public bool isDead;
    public bool isAttacking;
    public bool isAttacked;
    public bool playerWithinRange;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        // Handle attack cooldown timer
        if (currentAttackCooldown > 0)
            currentAttackCooldown -= Time.deltaTime;

        // Find distance between both characters
        playerWithinRange = Mathf.Abs(transform.position.x - player.transform.position.x) <= attackRange;
        if (playerWithinRange || isAttacked || isAttacking)
            moveInput = 0;
        else if (!playerWithinRange)
        {
            moveInput = (transform.position.x > player.transform.position.x) ? -1 : 1;
            //animator.SetBool("isFacingRight", moveInput == 1);
            transform.localScale = moveInput == -1 ? new Vector3(-10, 10, 10) : new Vector3(10, 10, 10);
        }
        
        animator.SetBool("isRunning", moveInput != 0);
        
        isGrounded = player.GetComponent<PlayerController>().isGrounded;
        if (isGrounded && currentAttackCooldown <= 0 && !isAttacked && !isAttacking && playerWithinRange)
            Attack();
    }

    void FixedUpdate()
    {
        if (isDead)
            return;
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Attack()
    {
            isAttacking = true;
            animator.SetBool("attack", true);
            player.GetComponent<PlayerController>().TakeDamage(attackDamage);
            currentAttackCooldown = attackCooldown;
            isAttacking = false;
            animator.SetBool("attack", false);
    }

    public void TakeDamage(int healthPoints)
    {
        isAttacked = true;
        animator.SetBool("damage", true);
        currentHealth -= healthPoints;
        isAttacked = false;
        animator.SetBool("damage", false);
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("death", true);
    }

    void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }
}