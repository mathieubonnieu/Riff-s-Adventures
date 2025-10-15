using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
// [RequireComponent(typeof(Collider))]
// [RequireComponent(typeof(Renderer))]
// [RequireComponent(typeof(Animator))]
public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    protected NavMeshAgent agent;
    protected Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    protected Animator animator;
    public Material damageMaterial;
    private Dictionary<string, Material> defaultMaterials = new Dictionary<string, Material>();
    public List<Renderer> meshes;
    GameObject playerObject;
    public GameObject DamageParticle;

    public GameObject DeathParticle;
    public GameObject DeathExplosionParticle;
    private float timeUntilExplosion = 1.3f;
    Rigidbody rb;

    private int lastAttack = 0;
    protected float health;

    [Header("Health Bar Settings")]
    public bool useHealthBar = true;
    public float maxHealth = 100f;
    public GameObject healthBarPrefab;
    public Vector3 healthBarScale = new Vector3(0.01f, 0.01f, 0.01f);
    public Vector3 healthBarOffset = new Vector3(0f, 0.6f, 0f);
    public Color healthBarColor = Color.red;
    public Color healthBarDamageColor = Color.white;
    public float shakeDuration = 0.2f;
    public float ShakeMagnitude = 0.1f;
    private GameObject healthBarInstance;
    private HealthBar3D healthBar;

    [Header("AI Settings")]
    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public int damageToGive;
    private bool isGoingToAttack = false;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private bool died = false;
    public float timeForAttack; // This time should be adjusted based on the attack animation

    public float speedOverride = 0f;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerObject = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        for (int i = 0; i < meshes.Count; i++)
        {
            defaultMaterials[meshes[i].name] = meshes[i].materials[0];
        }
        player = playerObject.transform;
    }

    protected virtual void Start()
    {
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
            }
        }

        if (useHealthBar && healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            healthBar = healthBarInstance.GetComponent<HealthBar3D>();
            if (healthBar != null)
            {
                healthBar.SetTarget(transform.position, health, maxHealth);
                healthBar.SetHealthBarOffset(healthBarOffset);
                healthBar.SetBarColor(healthBarColor);
                healthBar.SetBarDamageColor(healthBarDamageColor);
                healthBar.SetShakeDuration(shakeDuration);
                healthBar.SetShakeMagnitude(ShakeMagnitude);
                healthBar.transform.localScale = healthBarScale;
            }
        }
        health = maxHealth;
    }

    private void Update()
    {
        if (useHealthBar && healthBarPrefab != null && healthBar != null)
        {
            healthBar.SetTarget(transform.position, health, maxHealth);
        }
        //Check for sight and attack range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        playerInSightRange = distanceToPlayer < sightRange;
        playerInAttackRange = distanceToPlayer < attackRange;
        float speed = agent.velocity.magnitude * 0.5f;
        if (animator != null) {
            if (speedOverride > 0f)
                speed = speedOverride;
            animator.SetFloat("Speed", speed);
        }
        if (died) return;

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        else if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        else if (playerInSightRange && playerInAttackRange) AttackPlayer();
        else Debug.Log("Error in Enemy script");
    }

    protected virtual void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && agent.isOnNavMesh)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    protected virtual void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }
    }

    protected virtual void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        if (!alreadyAttacked)
        {
            Attack();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    //draw gizmo in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    protected virtual void Attack()
    {
        animator.SetTrigger("Attack");
        isGoingToAttack = true;
        Invoke(nameof(DealDamage), timeForAttack);
    }

    private void DealDamage()
    {
        if (isGoingToAttack == false)
            return;
        player.GetComponent<PlayerLife>().TakeDamage(damageToGive);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        isGoingToAttack = false;
    }

    protected virtual void ResetMaterial()
    {
        foreach (var mesh in meshes)
        {
            if (defaultMaterials.TryGetValue(mesh.name, out Material defaultMaterial))
            {
                mesh.material = defaultMaterial;
            }
        }
    }

    public virtual void TakeDamage(float damage, float knockback)
    {
        ResetAttackDelay();
        isGoingToAttack = false;
        health -= damage;


        if (died) return;

        CustomDamageBehavior(damage);

        if (damageMaterial != null) {
            foreach (var mesh in meshes)
            {
                mesh.material = damageMaterial;
                Invoke(nameof(ResetMaterial), 0.2f);
            }
        }

        if (health <= 0)
            {
                died = true;
                Die();
                return;
            }
            else if (animator)
            {
                animator.SetTrigger("Damage");
            }

        // knockback from the player position
        Vector3 knockbackDirection = (transform.position - player.position).normalized;
        if (rb != null && rb.isKinematic == false)
        {
            rb.AddForce(knockbackDirection * knockback, ForceMode.Impulse);
            if (agent.isOnNavMesh) {
                agent.isStopped = true;
                Invoke(nameof(ResetFromKnockback), 0.5f);
            }
        }
        if (DamageParticle != null)
        {
            GameObject damageParticle = Instantiate(DamageParticle, transform.position, Quaternion.identity);
            damageParticle.AddComponent<AutoDestroyParticles>();
        }
    }

    protected virtual void CustomDamageBehavior(float damage)
    {

    }

    protected virtual void Die()
    {
        if (useHealthBar && healthBar != null)
        {
            healthBar.SetHealth(health);
            healthBar.FadeAndDestroy(0.7f);
        }
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        GetComponent<Playsound>()?.PlaySoundDetached("death");
        if (DeathParticle != null)
        {
            GameObject deathParticle = Instantiate(DeathParticle, transform.position, DeathParticle.transform.rotation);
            deathParticle.AddComponent<AutoDestroyParticles>();
        }
        agent.enabled = false;
        Invoke(nameof(DeathExplosion), timeUntilExplosion);
    }

    private void DeathExplosion()
    {
        if (DeathExplosionParticle != null)
        {
            GameObject deathExplosionParticle = Instantiate(DeathExplosionParticle, transform.position, DeathExplosionParticle.transform.rotation);
            deathExplosionParticle.AddComponent<AutoDestroyParticles>();
        }
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            PlayerActions playerActions = playerObject.GetComponent<PlayerActions>();
            PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
            PlayerStats.stats stats = playerStats.GetModifiedStats();
            if (lastAttack == playerActions.attackId)
                return;
            if (playerActions != null && playerActions.isAttacking)
            {
                TakeDamage(stats.damage, stats.knockback);
            }
            lastAttack = playerActions.attackId;
        }
    }

    void ResetAttackDelay()
    {
        CancelInvoke(nameof(ResetAttack));
        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    void ResetFromKnockback()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            agent.isStopped = false;
        }
    }
}
