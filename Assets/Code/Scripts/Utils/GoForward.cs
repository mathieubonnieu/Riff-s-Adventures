using System;
using UnityEngine;

public class GoForward : MonoBehaviour
{
    public AnimationCurve speedCurve;
    public float speed = 1f;
    private float timeElapsed = 0f;

    void FixedUpdate()
    {
        timeElapsed += Time.fixedDeltaTime;
        float curveValue = speedCurve.Evaluate(timeElapsed);
        transform.Translate(curveValue * Time.fixedDeltaTime * Vector3.forward * speed);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.rotation * Vector3.forward * 2f);
    }
}
