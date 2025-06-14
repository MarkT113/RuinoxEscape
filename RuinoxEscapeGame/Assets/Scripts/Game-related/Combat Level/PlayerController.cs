using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Vector2 boxSize = new Vector2(0.55f, 0.001f);
    public Vector2 offset = new Vector2(0, -0.88f);
    [SerializeField] public LayerMask groundLayer;
    public Rigidbody2D rb;
    public float currentMoveInput;
    public int targetMoveInput;
    public float moveSensitivity = 4f;
    public float stopSensitivity = 8f;
    
    [Header("Combat")]
    public float attackCooldown = 5f;
    public float currentAttackCooldown;
    public float attackRange = 5f;
    public GameObject enemy;
    public int attackDamage = 1;
    public Animator animator;

    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth = 10;
    [SerializeField] public Image[] hearts;
    [SerializeField] public Sprite emptyHeart;
    
    [Header("Triggers")]
    public bool isGrounded = true;
    public bool isJumpTriggered;
    public bool isDead;
    public bool isAttacking;
    public bool isAttacked;
    public bool enemyWithinRange;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        UpdateHearts();
    }

    void Update()
    {
        if (isDead) return;

        // Handle attack cooldown timer
        if (currentAttackCooldown > 0)
            currentAttackCooldown -= Time.deltaTime;

        float sensitivity;
        if (targetMoveInput != 0) sensitivity = moveSensitivity;
        else sensitivity = stopSensitivity;

        if (isAttacked || isAttacking)
            currentMoveInput = 0;
        else
            currentMoveInput = Mathf.MoveTowards(currentMoveInput, targetMoveInput, sensitivity * Time.deltaTime);

        // Find distance between player and enemy (used later to detect whether or not the enemy is within attacking range)
        enemyWithinRange = Vector2.Distance(transform.position, enemy.transform.position) < attackRange ? true : false;

        // Animation
        animator.SetFloat("speed", currentMoveInput);
        animator.SetBool("isMoving", currentMoveInput != 0f);
        //animator.SetBool("IsGrounded", isGrounded);
        //animator.SetFloat("VerticalVelocity", rb.velocity.y);
    }

    void FixedUpdate()
    {
        if (isDead)
            return;
        // Check if grounded
        isGrounded = Physics2D.OverlapBox((Vector2)transform.position + offset, boxSize, 0, groundLayer);
        rb.velocity = new Vector2(currentMoveInput * moveSpeed, rb.velocity.y);
        if (isGrounded && isJumpTriggered)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumpTriggered = false;
        }
    }

    public void MoveLeft()
    {
        if (isDead || targetMoveInput == 1) return;
        targetMoveInput = -1;
        animator.SetBool("isFacingRight", false);
        
        // Flip sprite (sample code, not needed as it is implemented through animations; however, this is more efficient)
        /*if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }*/
    }

    public void MoveRight()
    {
        if (!isDead && targetMoveInput != -1)
        {
            targetMoveInput = 1;
            animator.SetBool("isFacingRight", true);
        }
    }

    public void StopMoving()
    {
        targetMoveInput = 0;
    }

    public void Jump()
    {
        if (!isDead && isGrounded && !isAttacked && !isAttacking)
        {
            isJumpTriggered = true;
            //animator.SetTrigger("Jump");
        }
    }

    public void Attack()
    {
        /* Instead of placing the detection range checker here, it is best to keep everything organised 
        and in the most suitable/appropriate place. Thus, I put it in the FixedUpdate() function. */
        if (!isDead && isGrounded && currentAttackCooldown <= 0 && !isAttacked && !isAttacking && enemyWithinRange)
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            //animator.SetTrigger("Attack");
            // Damage them (works for multiple enemies however for my level I will only have one)
            enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
            currentAttackCooldown = attackCooldown;
            isAttacking = false;
            animator.SetBool("isAttacking", false);
        }
    }

    public void TakeDamage(int damage)
    {
        isAttacked = true;
        animator.SetBool("isAttacked", true);
        currentHealth -= damage;
        UpdateHearts();
        //animator.SetTrigger("Hurt");
        // Transfer the following to another function
        isAttacked = false;
        animator.SetBool("isAttacked", false);
        if (currentHealth <= 0)
            Die();
    }

    void DamageContinued()
    {
        isAttacked = false;
        animator.SetBool("isAttacked", true);
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        // Pause level timer
        isDead = true;
        animator.SetBool("isDead", true);
        // Disable player controls and physics
        /*GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;*/
        //Invoke("RestartGame", 2f); // Restart game after delay
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateHearts()
    {
        for (int i = currentHealth; i < hearts.Length; i++)
        {
            hearts[i].sprite = emptyHeart;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + offset, boxSize);
    }
}