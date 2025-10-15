using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrailEffect : MonoBehaviour
{
    public GameObject trailObject;
    private TrailRenderer trailRenderer;
    private SmoothTrail smoothTrail;
    private TriggerActions triggerActions;
    // Start is called before the first frame update
    void Awake()
    {
        if (trailObject == null)
        {
            Debug.LogError("Sword GameObject is not assigned in the inspector.");
            return;
        }
        trailRenderer = trailObject.GetComponent<TrailRenderer>();
        if (trailRenderer == null)
        {
            Debug.LogError("TrailRenderer component not found on the sword object.");
            return;
        }
        triggerActions = GetComponent<TriggerActions>();
        if (triggerActions == null)
        {
            Debug.LogError("TriggerActions component not found on the GameObject.");
            return;
        }
        smoothTrail = trailObject.GetComponent<SmoothTrail>();
        if (smoothTrail == null)
        {
            Debug.LogError("SmoothTrail component not found on the sword object.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int triggerNumber = triggerActions.GetTriggerNumber();

        if (triggerNumber == 4 || triggerNumber == 3)
        {
            if (smoothTrail.activated == false)
            {
                smoothTrail.ActivateTrail();
            }
        }
        else
        {
            if (smoothTrail.activated == true)
            {
                smoothTrail.DeactivateTrail();
            }
        }
    }
}
