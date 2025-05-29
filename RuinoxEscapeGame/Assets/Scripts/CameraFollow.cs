using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer gameMap;
    [SerializeField] private Transform playerPos;
    [SerializeField] private float smoothing = 5f;

    private Camera cam;
    private float camHalfWidth, camHalfHeight;
    private float minBoundaryX, maxBoundaryX, minBoundaryY, maxBoundaryY;

    void Start()
    {
        cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
        var gameMapBounds = gameMap.bounds;
        minBoundaryX = gameMapBounds.min.x + camHalfWidth;
        maxBoundaryX = gameMapBounds.max.x - camHalfWidth;
        minBoundaryY = gameMapBounds.min.y + camHalfHeight;
        maxBoundaryY = gameMapBounds.max.y - camHalfHeight;
    }
    
    public void LateUpdate()
    {
        var targetCamPos = playerPos.position - new Vector3(0, 0, 10);
        targetCamPos.x = Mathf.Clamp(targetCamPos.x, minBoundaryX, maxBoundaryX);
        targetCamPos.y = Mathf.Clamp(targetCamPos.y, minBoundaryY, maxBoundaryY);
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}