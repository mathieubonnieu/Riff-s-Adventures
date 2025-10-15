using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUp : MonoBehaviour
{
    public AnimationCurve speedCurve;
    private float timeElapsed = 0f;

    void FixedUpdate()
    {
        timeElapsed += Time.fixedDeltaTime;
        float curveValue = speedCurve.Evaluate(timeElapsed);
        transform.localScale = new Vector3(curveValue, curveValue, curveValue);
    }
}
