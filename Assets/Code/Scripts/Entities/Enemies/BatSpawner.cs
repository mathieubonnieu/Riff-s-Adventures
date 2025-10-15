using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : EnemyBase
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public int maxEnemies = 10;

    private int spawnedEnemies = 0;
    private Coroutine spawningCoroutine;

    protected override void Start()
    {
        base.Start();
        if (agent.isOnNavMesh)
            agent.isStopped = true;
    }

    void OnEnable()
    {
        if (spawningCoroutine == null)
            spawningCoroutine = StartCoroutine(SpawnRoutine());
    }

    void OnDisable()
    {
        StopSpawning();
    }

    private IEnumerator SpawnRoutine()
    {
        while (spawnedEnemies < maxEnemies)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            Instantiate(enemyPrefab, hit.position, Quaternion.identity);
            spawnedEnemies++;
        }
    }

    public void StopSpawning()
    {
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
            spawningCoroutine = null;
        }
    }
    protected override void Patroling() { }
    protected override void AttackPlayer() { }

    protected override void CustomDamageBehavior(float damage)
    {
        GetComponent<Playsound>()?.PlaySound("hit" + Random.Range(1, 4));
    }
}
