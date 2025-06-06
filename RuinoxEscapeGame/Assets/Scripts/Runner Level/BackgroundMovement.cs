using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    public float speed;
    public Renderer backgroundRenderObject;

    void Update()
    {
        backgroundRenderObject.material.mainTextureOffset += new Vector2(0f, speed * Time.deltaTime);
    }
}
