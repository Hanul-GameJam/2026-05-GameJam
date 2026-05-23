using System.Diagnostics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Tracking Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;

    [Header("Position Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Camera Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minY, maxY;
    [SerializeField] private float minX, maxX;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (target == null)
        {
            FindPlayerTarget();
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayerTarget();
            return;
        }

        Vector3 targetPosition = target.position + offset;

        if (useBounds && targetPosition.y < minY)
        {
            targetPosition.y = minY;
        }

        if (useBounds && targetPosition.y > maxY)
        {
            targetPosition.y = maxY;
        }

        if (useBounds && targetPosition.x < minX)
        {
            targetPosition.x = minX;
        }

        if (useBounds && targetPosition.x > maxX)
        {
            targetPosition.x = maxX;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void FindPlayerTarget()
    {
        GameObject playerObj = GameObject.Find("Player");

        if (playerObj == null)
        {
            playerObj = GameObject.FindWithTag("Player");
        }

        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Player Not Found.");
        }
    }
}