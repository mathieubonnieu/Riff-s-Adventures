using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ParticleSystem))]
public class WolfBoss : EnemyBase
{
    enum Phase
    {
        Normal,
        Enraged
    };

    Phase currentPhase = Phase.Normal;

    ParticleSystem enragedParticleSystem;
    public GameObject meteorShowerPrefab;

    public abstract class IAttack
    {
        public abstract void Start(WolfBoss wolf);

        public abstract void Update(WolfBoss wolf);

        public abstract bool IsFinished(WolfBoss wolf);
    }

    public class DefaultAttack : IAttack
    {
        private readonly int damageToGive = 15;
        private bool isAttackFinished = false;
        private float attackDuration = 1.0f;

        private bool DamageGiven = false;
        public override void Start(WolfBoss wolf)
        {
            wolf.agent.SetDestination(wolf.transform.position);

            wolf.transform.LookAt(new Vector3(wolf.player.position.x, wolf.transform.position.y, wolf.player.position.z));
            wolf.animator.SetTrigger("Attack");
        }

        public override void Update(WolfBoss wolf)
        {
            attackDuration -= Time.deltaTime;
            if (attackDuration <= 0.6f && !DamageGiven)
            {
                DamageGiven = true;
                if (wolf.isAttackRange())
                    wolf.player.GetComponent<PlayerLife>().TakeDamage(damageToGive);
            }
            if (attackDuration <= 0)
            {
                isAttackFinished = true;
                DamageGiven = false;
            }
        }


        public override bool IsFinished(WolfBoss wolf)
        {
            return isAttackFinished;
        }
    }

    public class MeteorShower : IAttack
    {
        private bool isAttackFinished = false;
        private float attackDuration = 3.0f;
        private float meteorSpawnInterval = 0.5f;
        private float nextMeteorSpawnTime = 0.0f;
        private int meteorCount = 5; // Number of meteors to spawn
        private int currentMeteorCount = 0;
        static public GameObject meteorShowerPrefab;
        public override void Start(WolfBoss wolf)
        {
            wolf.agent.SetDestination(wolf.transform.position);

            wolf.transform.LookAt(new Vector3(wolf.player.position.x, wolf.transform.position.y, wolf.player.position.z));
            wolf.animator.SetTrigger("Howl");
            nextMeteorSpawnTime = meteorSpawnInterval;
            Vector3 spawnPosition = new Vector3(
                0f,
                15f,
                0f
            );
            spawnPosition += wolf.transform.position;
            Instantiate(meteorShowerPrefab, spawnPosition, Quaternion.identity, wolf.transform).transform.parent = null;
        }

        public override void Update(WolfBoss wolf)
        {
            attackDuration -= Time.deltaTime;
            if (attackDuration <= 0)
            {
                isAttackFinished = true;
            }
        }


        public override bool IsFinished(WolfBoss wolf)
        {
            return isAttackFinished;
        }
    }

    public bool isAttackRange()
    {
        return playerInAttackRange;
    }

    private Dictionary<Phase, List<Type>> attackByPhase = new Dictionary<Phase, List<Type>>()
    {
        { Phase.Normal, new List<Type>() {
            typeof(DefaultAttack)
        } },
        { Phase.Enraged, new List<Type>() {
            typeof(DefaultAttack),
            typeof(DefaultAttack),
            typeof(DefaultAttack),
            typeof(MeteorShower)
        } }
    };

    private int current_attackIndex = 0;

    private IAttack currentAttack = null;

    protected override void Awake()
    {
        base.Awake();
        enragedParticleSystem = GetComponent<ParticleSystem>();
        enragedParticleSystem.Stop();
        MeteorShower.meteorShowerPrefab = meteorShowerPrefab;
    }

    protected override void Patroling()
    {
        if (currentAttack != null)
        {
            currentAttack.Update(this);
            if (currentAttack.IsFinished(this))
            {
                currentAttack = null;
            }
            return;
        }
        base.Patroling();
    }

    protected override void ChasePlayer()
    {
        if (currentAttack != null)
        {
            currentAttack.Update(this);
            if (currentAttack.IsFinished(this))
            {
                currentAttack = null;
            }
            return;
        }
        base.ChasePlayer();
    }

    protected override void AttackPlayer()
    {
        if (currentAttack == null || currentAttack.IsFinished(this))
        {
            Type attackType = attackByPhase[currentPhase][current_attackIndex];
            currentAttack = (IAttack)Activator.CreateInstance(attackType);
            currentAttack.Start(this);
            current_attackIndex = (current_attackIndex + 1) % attackByPhase[currentPhase].Count;
        }
        currentAttack.Update(this);
    }

    protected override void CustomDamageBehavior(float damage)
    {
        switch (currentPhase)
        {
            case Phase.Normal:
                if (maxHealth / 2 > health)
                {
                    SwitchToNextPhase();
                }
                break;
            case Phase.Enraged:
                break;
        }
    }

    void SwitchToNextPhase()
    {
        current_attackIndex = 0;
        switch (currentPhase)
        {
            case Phase.Normal:
                enragedParticleSystem.Play();
                currentPhase = Phase.Enraged;
                break;
            case Phase.Enraged:
                break;
        }
    }

    public override void TakeDamage(float damage, float knockback)
    {
        knockback *= 0.5f;
        base.TakeDamage(damage, knockback);
    }
}
