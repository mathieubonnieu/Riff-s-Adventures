using System;
using RPGCharacterAnims.Actions;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    TriggerActions triggerActions;

    PlayerMovement playerMovement;
    PlayerStats playerStats;
    Camera mainCamera;

    public bool isAttacking = false;
    public bool isDodging = false;

    public int attackId = 0;

    public float dashSpeed = 1.0f;
    public float dashGravity = 1.0f;

    void Start()
    {
        triggerActions = GetComponent<TriggerActions>();
        playerMovement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerStats>();
        mainCamera = Camera.main;
    }

    private RaycastHit PerformRaycastFromMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        Physics.Raycast(ray, out RaycastHit hit);
        return hit;
    }

    void Update()
    {
        int triggerNumber = triggerActions.GetTriggerNumber();

        if (triggerNumber == 0)
        {
            isAttacking = false;
            isDodging = false;
        }
        else if (triggerNumber == 3)
        {
            isDodging = true;
        }
        else if (triggerNumber == 4 || triggerNumber == 12)
        {
            isAttacking = true;
        }
    }


    public void AttackStandard()
    {
        RaycastHit hit = PerformRaycastFromMouse();
        Vector3 currentPosition = playerMovement.transform.position;
        Vector3 targetPosition = hit.point;
        Vector3 direction = (targetPosition - currentPosition).normalized;

        int triggerNumber = triggerActions.GetTriggerNumber();
        if (triggerNumber != 0 && triggerNumber != 4)
        {
            return;
        }

        if (triggerActions.ActiveTriggerIfNoActionAndUpgrade(4, 3, 0.25f)) {
            Invoke(nameof(OnAttackCallback), 0.25f);
            attackId = UnityEngine.Random.Range(0, 9999);
        }
        triggerActions.whileAnimatingAction = () =>
        {
            playerMovement.directionOverride = direction;
        };
    }

    private void OnAttackCallback()
    {
        playerStats.OnAttack(gameObject);
    }

    public void SecondaryAttack()
    {
        RaycastHit hit = PerformRaycastFromMouse();
        Vector3 currentPosition = playerMovement.transform.position;
        Vector3 targetPosition = hit.point;
        Vector3 direction = (targetPosition - currentPosition).normalized;


        int triggerNumber = triggerActions.GetTriggerNumber();
        if (triggerNumber != 0)
        {
            return;
        }
        attackId = UnityEngine.Random.Range(0, 9999);

        if (triggerActions.ActiveTriggerIfNoAction(12))
        {
            Invoke(nameof(OnAttackCallback), 0.25f);
        }
        triggerActions.whileAnimatingAction = () =>
        {
            playerMovement.directionOverride = direction;
        };
    }

    public void Dodge()
    {
        int triggerNumber = triggerActions.GetTriggerNumber();
        if (triggerNumber != 0)
        {
            return;
        }

        isDodging = true;

        if (triggerActions.ActiveTriggerIfNoAction(3, -1)) {
            playerStats.OnDodge(gameObject);
        }
        triggerActions.whileAnimatingAction = () =>
        {
            float currentDashSpeed = dashSpeed;
            float animationCompletion = triggerActions.GetAnimationCompletion();
            animationCompletion = Mathf.Clamp(animationCompletion, 0f, 66f);
            if (animationCompletion > 55f)
            {
                currentDashSpeed = dashSpeed * (1f - (animationCompletion - 55f) / (66f - 55f));
                if (currentDashSpeed < 0f)
                {
                    print(animationCompletion);
                }
                // print(currentDashSpeed);
            }
            if (!triggerActions.hasAppliedGravity)
            {
                playerMovement.OverrideMovement(transform.forward * currentDashSpeed, dashGravity, applyGravity: true);
                triggerActions.hasAppliedGravity = true;
            }
            else
            {
                playerMovement.OverrideMovement(transform.forward * currentDashSpeed, 0f, applyGravity: false);
            }
        };
    }

    void OnDrawGizmos()
    {
        if (mainCamera == null) return;

        // draw circle where the mouse raycast hits
        Gizmos.color = Color.white;
        RaycastHit hit = PerformRaycastFromMouse();
        Gizmos.DrawSphere(hit.point, 0.1f);
    }
}
