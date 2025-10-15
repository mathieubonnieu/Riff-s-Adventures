using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotate : MonoBehaviour
{
    private Vector3 rotation = new Vector3(0, 1, 0);
    public float speed = 1.0f;
    void Start()
    {
        rotation = Random.onUnitSphere;
    }

    void Update()
    {
        transform.Rotate(rotation * speed * Time.deltaTime, Space.Self);
    }
}
