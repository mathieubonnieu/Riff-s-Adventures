using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OpacityOverTime : MonoBehaviour
{
    public AnimationCurve curve;
    private Light lightSource;
    private float timeElapsed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        lightSource = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        float curveValue = curve.Evaluate(timeElapsed);
        lightSource.intensity = curveValue;
    }
}
