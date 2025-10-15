using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCamera : MonoBehaviour
{
    private GameObject mainCamera;
    public Vector3 offest = new Vector3(50, 0, 50);
    void Start()
    {
        mainCamera = Camera.main.gameObject;
    }

    void Update()
    {
        Vector3 miniCameraPosition = mainCamera.transform.position / 9 + offest;
        transform.position = miniCameraPosition;
        transform.rotation = Camera.main.transform.rotation;
        // add a more disanace to the camera with the rotation
        transform.position += transform.rotation * new Vector3(0, 0, -10);
    }
}
