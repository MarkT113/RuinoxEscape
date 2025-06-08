using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    public static float speed = 1f;
    
    private Renderer backgroundRenderObject;

    void Start()
    {
        backgroundRenderObject = GetComponent<Renderer>();
    }
    
    void Update()
    {
        backgroundRenderObject.material.mainTextureOffset += new Vector2(0f, speed * Time.deltaTime);
    }
}
