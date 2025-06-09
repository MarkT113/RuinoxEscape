using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static float cameraSpeed = 15f;

    void Update()
    {
        transform.position += new Vector3(0, cameraSpeed * Time.deltaTime, 0);
    }
}