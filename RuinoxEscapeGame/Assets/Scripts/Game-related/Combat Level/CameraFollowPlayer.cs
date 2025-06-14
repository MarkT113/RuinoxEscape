using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;  // Smooth follow speed
    private Vector3 offset = new Vector3(0, 4, -10);

    void LateUpdate()
    {
        if (!player) return;

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}