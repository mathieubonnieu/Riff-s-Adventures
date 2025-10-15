using System;
using UnityEngine;

public class Boar : EnemyBase
{
    public float chargeRange = 8f;
    private float chargeDistanceTravelled = 0f;
    public float chargeSpeed = 8f;
    private bool isCharging = false;
    private Vector3 chargeDirection;
    public float maxChargeTime = 1f;
    private float chargeTime = 0f;
    public float stunDuration = 2f;
    private float stunTime = 0f;

    public ParticleSystem ChargeParticles;
    public ParticleSystem StunParticles;

    protected override void Start()
    {
        base.Start();
        chargeTime = maxChargeTime;
    }

    protected override void Patroling()
    {
        if (isCharging)
            Charging();
    }

    protected override void ChasePlayer()
    {
        // ChasePlayer should never be called
    }
    protected override void AttackPlayer()
    {
        speedOverride = 0f;
        if (stunTime > 0)
        {
            stunTime -= Time.deltaTime;
            return;
        }
        if (!isCharging)
            PrepareCharge();
        else
            Charging();
    }

    void Charging()
    {
        agent.Move(chargeDirection * chargeSpeed * Time.deltaTime);
        chargeDistanceTravelled += chargeSpeed * Time.deltaTime;
        speedOverride = chargeSpeed;
        if (chargeDistanceTravelled >= chargeRange)
        {
            ChargeParticles.Stop();
            isCharging = false;
        }
    }

    void Charge()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f;
        chargeDirection = directionToPlayer.normalized;
        chargeDistanceTravelled = 0f;

        agent.speed = chargeSpeed;
        agent.isStopped = false;
        isCharging = true;
    }

    protected override void CustomDamageBehavior(float damage)
    {
        stunTime = stunDuration;
    }

    void Stun()
    {
        agent.isStopped = false;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        animator.SetTrigger("Stun");

        isCharging = false;
        ChargeParticles.Stop();
        stunTime = stunDuration;
        StunParticles.Play();
    }
void PrepareCharge()
{
    Vector3 directionToPlayer = player.position - transform.position;
    directionToPlayer.y = 0f;

    if (directionToPlayer != Vector3.zero)
    {
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
    if (!ChargeParticles.isPlaying)
        ChargeParticles.Play();
    chargeTime -= Time.deltaTime;
    if (chargeTime <= 0f)
    {
        chargeTime = maxChargeTime;
        Charge();
    }
}


    protected override void OnTriggerEnter(Collider other)
    {

        if (isCharging)
        {
            if (other.CompareTag("Player"))
            {
                PlayerLife playerLife = other.GetComponent<PlayerLife>();
                if (playerLife != null)
                {
                    playerLife.TakeDamage(damageToGive);
                }
            }
            else if (other.CompareTag("Obstacle"))
            {
                Stun();
            }
        }
        else
        {
            base.OnTriggerEnter(other);
        }
    }
}
