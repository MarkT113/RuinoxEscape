using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    public Transform player;

    private float initialPlayerX;
    private float initialBackgroundX;
    private Renderer backgroundRenderObject;

    void Start()
    {
        if (!player)
        {
            Debug.LogError("ParallaxBackground: Player transform not assigned.");
            enabled = false;
            return;
        }

        backgroundRenderObject = GetComponent<Renderer>();
        initialPlayerX = player.position.x;
        initialBackgroundX = transform.position.x;
    }

    void Update()
    {
        float playerDeltaX = player.position.x - initialPlayerX;

        // Keep background at same X offset relative to player
        transform.position = new Vector3(initialBackgroundX + playerDeltaX, transform.position.y, transform.position.z);

        // Scroll texture in the opposite X direction
        backgroundRenderObject.material.mainTextureOffset = new Vector2(-playerDeltaX * 0.5f, 0f);
    }
}