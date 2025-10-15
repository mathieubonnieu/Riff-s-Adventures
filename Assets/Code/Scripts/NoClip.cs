using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoClip : MonoBehaviour
{
    public float clipZoneY = -30.0f;

    private Vector3 fallbackPosition;
    private Transform objTransform;
    private PlayerMovement playerMovement;

    void Start()
    {
        objTransform = GetComponent<Transform>();
        playerMovement = GetComponent<PlayerMovement>();

        if (objTransform)
            fallbackPosition = objTransform.position;
    }

    void Update()
    {
        if (objTransform && objTransform.position.y <= clipZoneY)
        {
            playerMovement.resetPositionAt(fallbackPosition);
        }
    }
}
