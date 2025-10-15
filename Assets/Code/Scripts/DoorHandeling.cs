using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandeling : MonoBehaviour
{
    private Camera mainCamera;
    private OpenDoor[] openDoorScripts;
    private List<string> finishedRooms;

    [Serializable]
    public struct ItemSpawnData
    {
        public GameObject itemPrefab;
        public float spawnChance;
    }

    public List<ItemSpawnData> itemPrefabs;

    void Awake()
    {
        finishedRooms = new List<string>();
    }

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found. Please ensure there is a camera tagged as 'MainCamera' in the scene.");
        }
        // get all OpenDoor scripts in the scene
        openDoorScripts = FindObjectsOfType<OpenDoor>();
        if (openDoorScripts.Length == 0)
        {
            Debug.LogError("No OpenDoor scripts found in the scene. Please ensure there are OpenDoor components attached to GameObjects.");
        }
    }
    void Update()
    {
        GameObject CurrentRoom = GetCurrentRoom();
        if (CurrentRoom == null)
        {
            Debug.LogWarning("No room found for the current camera position.");
            return;
        }
        // check if there is an enemy in the room with the enemy tag
        EnemyBase[] enemies = CurrentRoom.GetComponentsInChildren<EnemyBase>();
        if (enemies.Length > 0)
        {
            CloseDoors(CurrentRoom);
        }
        else
        {
            if (!finishedRooms.Contains(CurrentRoom.name))
            {
                OpenDoors(CurrentRoom);
                RoomTrigger roomTrigger = CurrentRoom.GetComponent<RoomTrigger>();
                if (roomTrigger != null && roomTrigger.isVisited && roomTrigger.enemiesSpawned)
                    SpawnItem(CurrentRoom);
                finishedRooms.Add(CurrentRoom.name);
            }
        }
    }

    void SpawnItem(GameObject room)
    {
        if (itemPrefabs.Count == 0)
        {
            Debug.LogWarning("No item prefabs available for spawning.");
            return;
        }

        // Randomly select an item based on spawn chances
        float totalChance = 0f;
        foreach (var itemData in itemPrefabs)
        {
            totalChance += itemData.spawnChance;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        foreach (var itemData in itemPrefabs)
        {
            cumulativeChance += itemData.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                SpawnItemInRoom(itemData.itemPrefab, room.transform.position);
                break;
            }
        }
    }

    void SpawnItemInRoom(GameObject itemPrefab, Vector3 roomPosition)
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning("Item prefab is null. Cannot spawn item.");
            return;
        }

        // Instantiate the item at a random position within the room
        Vector3 spawnPosition = roomPosition + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0.5f, UnityEngine.Random.Range(-2f, 2f));
        Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
    }

    GameObject GetCurrentRoom()
    {
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            Debug.LogError("CameraFollow script not found on the main camera. Please ensure it is attached.");
            return null;
        }
        return cameraFollow.CurrentRoom;
    }

    void OpenDoors(GameObject room)
    {
        // foreach (OpenDoor door in openDoorScripts)
        // {
        //     if (door != null && !door.IsOpen())
        //     {
        //         door.Open();
        //     }
        // }
        foreach (OpenDoor door in room.GetComponentsInChildren<OpenDoor>())
        {
            if (door != null && !door.IsOpen())
            {
                door.Open();
            }
        }
    }

    void CloseDoors(GameObject room)
    {
        foreach (OpenDoor door in room.GetComponentsInChildren<OpenDoor>())
        {
            if (door != null && door.IsOpen())
            {
                door.Close();
            }
        }
    }
}
