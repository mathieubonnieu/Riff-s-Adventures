using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TriggerActions : MonoBehaviour
{
    [Header("Trigger Settings")]
    public float dashSpeed = 1.0f;
    public float gravity = 1.0f;

    [Header("Scripts")]
    public PlayerMovement playerMovement;

    private Animator anim;
    private int activeTriggerStateHash;
    private bool isTriggerActive = false;
    public bool hasAppliedGravity = false;
    private Dictionary<Action, KeyCode> triggerEvents;

    public Action whileAnimatingAction = null;

    public Action atAnimationEndAction = null;

    void OnEnable()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger("Action", 0);
        PlayerActions playerAction = GetComponent<PlayerActions>();
        triggerEvents = new Dictionary<Action, KeyCode>
        {
            { () => playerAction.Dodge(), KeyCode.LeftShift },
            { () => playerAction.AttackStandard(), KeyCode.Mouse0 },
            { () => playerAction.SecondaryAttack(), KeyCode.Mouse1 }
        };
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        int currentAction = anim.GetInteger("Action");

        foreach (var triggerEvent in triggerEvents)
        {
            if (Input.GetKeyDown(triggerEvent.Value))
            {
                triggerEvent.Key.Invoke();
                break;
            }
        }

        if (isTriggerActive)
        {
            if (stateInfo.shortNameHash != activeTriggerStateHash && !anim.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Getup1"))
            {
                ResetTrigger();
                isTriggerActive = false;
            }
        }
        if (whileAnimatingAction != null && currentAction != 0)
        {
            whileAnimatingAction.Invoke();
        }
    }

    private void ActivateTrigger(int triggerId, int action = 1)
    {
        anim.SetInteger("Action", action);
        anim.SetInteger("TriggerNumber", triggerId);
        anim.SetBool("Trigger", true);

        playerMovement.SetMovementLock(true);

        StartCoroutine(CaptureTriggerStateNextFrame());
    }

    private bool ActivateTriggerAndUpgrade(int triggerId, int action = 2, float space = 0.25f)
    {
        isTriggerActive = false;
        hasAppliedGravity = false;
        int currentAction = anim.GetInteger("Action");
        int triggerNumber = anim.GetInteger("TriggerNumber");
        int IsInTransition = anim.GetCurrentAnimatorStateInfo(0).shortNameHash;

        float completion = GetAnimationCompletion();
        if (completion < space * 100f && triggerId == triggerNumber && IsInTransition != 0)
        {
            return false;
        }

        currentAction++;
        if (currentAction > action)
            return false;

        CancelCurrentTriggerAndActivate(triggerId, currentAction, currentAction == 3 ? 1 : 0.2f);
        playerMovement.SetMovementLock(true);

        // StartCoroutine(CaptureTriggerStateNextFrame());
        return true;
    }

    public bool ActiveTriggerIfNoAction(int triggerId, int action = 1)
    {
        if (anim.GetInteger("Action") == 0)
        {
            ActivateTrigger(triggerId, action);
            return true;
        }
        return false;
    }

    public bool ActiveTriggerIfNoActionAndUpgrade(int triggerId, int action = 2, float space = 0.25f)
    {
        if (anim.GetInteger("Action") == 0 || anim.GetInteger("TriggerNumber") == triggerId)
        {
            return ActivateTriggerAndUpgrade(triggerId, action, space);
        }
        return false;
    }


    private IEnumerator CaptureTriggerStateNextFrame(float time = 0.2f)
    {
        if (time < 0f)
            yield return WaitForNextState();
        else
            yield return new WaitForSeconds(time);
        yield return WaitForNextState();
        ResetTrigger();
    }

    private IEnumerator WaitForNextState(float timeout = 100f)
    {
        int previousHash = anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
        float elapsedTime = 0f;

        yield return new WaitUntil(() =>
        {
            elapsedTime += Time.deltaTime;
            return (anim.GetCurrentAnimatorStateInfo(0).shortNameHash != previousHash &&
                    !anim.IsInTransition(0)) || elapsedTime >= timeout;
        });
    }

    public void ResetTrigger()
    {
        anim.SetInteger("Action", 0);
        anim.SetInteger("TriggerNumber", 0);
        anim.SetBool("Trigger", false);
        hasAppliedGravity = false;
        playerMovement.SetMovementLock(false);
        whileAnimatingAction = null;
        if (atAnimationEndAction != null)
            atAnimationEndAction.Invoke();
        atAnimationEndAction = null;
    }

    public bool IsTriggerActive()
    {
        return isTriggerActive;
    }

    public int GetCurrentTriggerNumber()
    {
        return anim.GetInteger("TriggerNumber");
    }

    public float GetAnimationCompletion()
    {
        float completion = anim.GetCurrentAnimatorStateInfo(0).normalizedTime * 100;
        while (completion > 100f)
        {
            completion -= 100f;
        }
        return completion;
    }

    public void CancelCurrentTriggerAndActivate(int newTriggerId, int action = 1, float time = 0.2f)
    {
        StopAllCoroutines();
        whileAnimatingAction = null;
        isTriggerActive = false;
        hasAppliedGravity = false;

        anim.SetInteger("Action", action);
        anim.SetInteger("TriggerNumber", newTriggerId);
        anim.SetBool("Trigger", true);

        StartCoroutine(CaptureTriggerStateNextFrame(time));
    }

    public int GetTriggerNumber()
    {
        return anim.GetInteger("TriggerNumber");
    }
}
