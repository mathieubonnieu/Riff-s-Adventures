using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject Door;
    // default rotation angles
    public AnimationCurve openCurve;
    private float defaultAngle;
    public float openAngle = 90.0f; // Angle to open the door
    public float closeAngle = 0.0f; // Angle to close the door
    public float openDelay = 0.5f; // Delay before opening the door
    public float animationSpeed = 1.0f; // Speed of the door opening/closing animation
    private bool openDoor = false;

    private void Start()
    {
        if (Door == null)
        {
            Debug.LogError("Door GameObject is not assigned.");
        }
        defaultAngle = Door.transform.eulerAngles.y;
    }

    private void Update()
    {
        // press "O" to open the door
        if (Input.GetKeyDown(KeyCode.O))
        {
            Open();
        }
        // press "C" to close the door
        if (Input.GetKeyDown(KeyCode.C))
        {
            Close();
        }
    }

    private IEnumerator RotateDoor(float targetAngle)
    {
        if (Door == null) yield break;

        yield return new WaitForSeconds(openDelay);

        Quaternion startRotation = Door.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(Door.transform.eulerAngles.x, defaultAngle + targetAngle, Door.transform.eulerAngles.z);
        float elapsedTime = 0f;
        while (elapsedTime < animationSpeed)
        {
            float t = elapsedTime / animationSpeed;
            float curveValue = openCurve.Evaluate(t);
            Door.transform.rotation = Quaternion.Slerp(startRotation, endRotation, curveValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Door.transform.rotation = endRotation;
    }

    public void Open()
    {
        if (openDoor) return;

        openDoor = true;
        StartCoroutine(RotateDoor(openAngle));
    }

    public void Close()
    {
        if (!openDoor) return;

        openDoor = false;
        StartCoroutine(RotateDoor(closeAngle));
    }

    public bool IsOpen()
    {
        return openDoor;

    }

    public void disableDoor()
    {
        if (Door != null)
        {
            Door.SetActive(false);
        }
    }
}
