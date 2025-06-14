using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LootBoxController : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public GameObject spaceshipPiece;
    public float openRange = 0.5f;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnInteractButtonPress()
    {
        bool isValidRange = Vector2.Distance(transform.position, player.transform.position) < openRange;
        if (enemy.GetComponent<EnemyController>().isDead && isValidRange)
            animator.SetTrigger("Open");
    }

    void RevealPart()
    {
        spaceshipPiece.SetActive(true);
    }
}