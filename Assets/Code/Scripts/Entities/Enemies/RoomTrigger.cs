using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public bool isVisited = false;
    public bool enemiesSpawned = false;
    
    DungeonGeneration dungeonGeneration;

    private void Start()
    {
        dungeonGeneration = FindObjectOfType<DungeonGeneration>();
        if (dungeonGeneration == null)
        {
            Debug.LogError("DungeonGeneration not found in the scene.");
        }
    }

    public bool IsThereEnemies()
    {
        EnemyBase[] enemies = GetComponentsInChildren<EnemyBase>();
        foreach (EnemyBase enemy in enemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetSpawnersActive(true);
            if (!isVisited && !IsThereEnemies())
            {
                enemiesSpawned = dungeonGeneration.SpawnEnemies(gameObject);
            }
            isVisited = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetSpawnersActive(false);
        }
    }

    void SetSpawnersActive(bool isActive)
    {
        foreach (EnemySpawner spawner in GetComponentsInChildren<EnemySpawner>())
        {
            spawner.enabled = isActive;

            // Si on veut stopper la coroutine proprement (optionnel)
            if (!isActive)
                spawner.StopSpawning();
        }
    }
}
