using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothing = 5f;
    [SerializeField] private Transform minBoundary;
    [SerializeField] private Transform maxBoundary;
    
    private Vector3 targetCamPos;
    
    void Start()
    {
        targetCamPos = target.position;
    }
    
    public void LateUpdate()
    {
        targetCamPos.x = Mathf.Clamp(targetCamPos.x, minBoundary.position.x, maxBoundary.position.x);
        targetCamPos.y = Mathf.Clamp(targetCamPos.y, minBoundary.position.y, maxBoundary.position.y);
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}