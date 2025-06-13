using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PrizeController : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject player;
    public float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player != null) // Quick check
        {
            Vector2 newPosition = Vector2.MoveTowards(rb.position, player.GetComponent<Rigidbody2D>().position, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameData.minigamesStatus[SceneManager.GetActiveScene().buildIndex - 2] = 2;
            SceneManager.LoadScene(1);
        }
    }
}