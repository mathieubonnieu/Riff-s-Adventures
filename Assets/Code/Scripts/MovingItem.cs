using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingItem : MonoBehaviour
{
    public float speed = 5f;
    public float amplitude = 0.5f;
    public float rotationSpeed = 50f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the item in a circular motion
        float y = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(startPosition.x, y, startPosition.z);

        // Rotate the item
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
