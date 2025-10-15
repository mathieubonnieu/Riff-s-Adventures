using UnityEngine;

public class CameraProjection : MonoBehaviour
{
    public float targetProjection = 1.0f;
    public Transform player;
    public float rotationTimeInSeconds = 10.0f;
    public float distanceFromPlayer = 10.0f;
    public float heightOffset = 5.0f;
    public float targetHeightOffset = 1.0f;

    void Update()
    {
        // Smoothly update orthographic size
        if (Camera.main != null && Camera.main.orthographic)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetProjection, Time.deltaTime * 2);
        }

        // Rotate around the player
        if (player != null)
        {
            float angle = (Time.time / rotationTimeInSeconds) * 360f;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(radians) * distanceFromPlayer,
                heightOffset,
                Mathf.Sin(radians) * distanceFromPlayer
            );

            transform.position = player.position + offset;

            // Adjust the target look point using targetHeightOffset
            Vector3 lookTarget = player.position + Vector3.up * targetHeightOffset;
            transform.LookAt(lookTarget);
        }
    }
}
