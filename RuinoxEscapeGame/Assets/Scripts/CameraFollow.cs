using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Collider2D cameraBounds;
    [SerializeField] private Transform playerPos;
    [SerializeField] private float smoothing = 5f;
    private Vector3 targetCamPos;

    void Start()
    {
        Debug.Log(GetComponent<Camera>().orthographicSize);
        Debug.Log(GetComponent<Camera>().aspect);
    }
    
    public void LateUpdate()
    {
        if (cameraBounds.OverlapPoint(playerPos.position))
            targetCamPos = playerPos.position - new Vector3(0, 0, 10);
        else
        {
            targetCamPos = cameraBounds.ClosestPoint(playerPos.position);
            targetCamPos = new Vector3(targetCamPos.x, targetCamPos.y, -10);
        }
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}