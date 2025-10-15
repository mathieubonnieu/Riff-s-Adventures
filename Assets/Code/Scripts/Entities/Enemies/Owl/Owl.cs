using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Owl : EnemyBase
{
    public GameObject sleepWavePrefab;
    public float retreatDistance = 5f;
    public float moveSpeed = 2f;
    public float waveCooldown = 3f;
    public LayerMask obstructionLayer;
    bool canShoot = true;

    private Coroutine ambientSoundCoroutine;

    private void OnEnable()
    {
        ambientSoundCoroutine = StartCoroutine(PlayAmbientSounds());
    }

    private void OnDisable()
    {
        if (ambientSoundCoroutine != null)
        {
            StopCoroutine(ambientSoundCoroutine);
            ambientSoundCoroutine = null;
        }
    }

    private IEnumerator PlayAmbientSounds()
    {
        var soundSystem = GetComponent<Playsound>();
        while (true)
        {
            float delay = Random.Range(5f, 10f);
            yield return new WaitForSeconds(delay);

            if (soundSystem != null)
            {
                string clipName = "hooting" + Random.Range(1, 4);
                soundSystem.PlaySoundDetached(clipName);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        agent.updateRotation = false;
    }

    private void FireSleepWave()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        Vector3 spawnPos = transform.position + transform.forward + Vector3.up * 0.3f;
        Quaternion rotation = Quaternion.LookRotation(transform.forward);
        GameObject wave = Instantiate(sleepWavePrefab, spawnPos, rotation);

        SleepWave waveScript = wave.GetComponent<SleepWave>();
        waveScript.SetDirection(transform.forward);
    }

    private void AllowShooting()
    {
        canShoot = true;
    }

    protected override void ChasePlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);
        if (canShoot)
        {
            FireSleepWave();
            canShoot = false;
            Invoke(nameof(AllowShooting), waveCooldown);
        }
    }
    protected override void AttackPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        Vector3 retreatDirection = -directionToPlayer.normalized;
        Vector3 retreatTarget = transform.position + retreatDirection * moveSpeed * Time.deltaTime;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(retreatTarget, out NavMeshHit hitInfo, 1.0f, NavMesh.AllAreas))
            {
                agent.Move(retreatDirection * moveSpeed * Time.deltaTime);
            }
        }
    }
}
