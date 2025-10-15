using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    [Header("Sturcture")]
    public int numberOfRooms = 20;
    public float doubleRoomChance = 0.5f;
    public GameObject roomPrefab;
    public GameObject doubleRoomPrefab;
    public GameObject startRoomPrefab;
    public GameObject itemRoomPrefab;

    public GameObject bossRoomPrefab;
    private List<Room> rooms = new List<Room>();

    [Header("Enemy")]
    public bool enemySpawningEnabled = true;
    public GameObject[] enemyPrefabs;
    public GameObject bossPrefab;

    class Room
    {
        public Vector2 position = Vector2.zero;
        public GameObject roomObject = null;

        public bool isDoubleRoom = false;

        public Vector2 secondPosition = new Vector2();
    }

    void createRoom(Vector2 position, GameObject roomPrefab, int direction = -1, bool isDoubleRoom = false, Vector2 secondPosition = new Vector2())
    {
        Vector3 position3 = new Vector3(position.x, 0, position.y);
        if (direction == -1)
            direction = Random.Range(0, 4);
        Quaternion rotation = intToQuaternion(direction);

        GameObject newRoom = Instantiate(roomPrefab, position3 * 9, rotation);
        rooms.Add(new Room
        {
            position = position,
            roomObject = newRoom,
            isDoubleRoom = isDoubleRoom,
            secondPosition = secondPosition
        });
        newRoom.name = (isDoubleRoom ? "Double" : "") + "Room_" + position3.x.ToString("F0") + "_" + position3.z.ToString("F0");

        // Set the parent of the room object to this script's GameObject
        newRoom.transform.parent = this.transform;
        newRoom.AddComponent<RoomTrigger>();
        SetSpawnersActive(newRoom, false);

        foreach (var spawner in newRoom.GetComponentsInChildren<EnemySpawner>())
        {
            spawner.enabled = false;
        }
    }

    public bool isAvailable(Vector2 position)
    {
        foreach (Room room in rooms)
        {
            if (room.position == position || room.secondPosition == position)
            {
                return false;
            }
        }
        return true;
    }

    public int placeForDoubleRoom(Vector2 position)
    {
        int valid = 0b1111;
        if (!isAvailable(position))
        {
            return 0;
        }
        for (int i = 0; i < 4; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos(i * Mathf.PI / 2), Mathf.Sin(i * Mathf.PI / 2));
            if (numberOfNeighbours(position + direction) != 0)
            {
                valid &= ~(1 << i);
            }
        }
        return valid;
    }

    void createDoobleRoom(Vector2 position, int valid = -1, GameObject room = null)
    {
        if (room == null)
            room = doubleRoomPrefab;
        if (valid == -1)
                valid = placeForDoubleRoom(position);
        if (valid == 0)
            return;
        int direction = 0;
        while ((valid & (1 << direction)) == 0)
        {
            direction = Random.Range(0, 4);
        }
        createRoom(position, room, direction, true, position + intToVector2(direction));
    }

    Vector2 getRandomDirection()
    {
        int i = Random.Range(0, 4);
        return new Vector2(Mathf.Cos(i * Mathf.PI / 2), Mathf.Sin(i * Mathf.PI / 2));
    }

    Vector2 intToVector2(int i)
    {
        return new Vector2(Mathf.Cos(i * Mathf.PI / 2), Mathf.Sin(i * Mathf.PI / 2));
    }

    Quaternion intToQuaternion(int i)
    {
        Vector2 dir = intToVector2(i);
        return Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y));
    }


    int numberOfNeighbours(Vector2 position)
    {
        int count = 0;
        foreach (Room room in rooms)
        {
            if (room.position == position || room.secondPosition == position)
            {
                count++;
            }
        }
        return count;
    }

    int numberOfNeighbours(int valid)
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if ((valid & (1 << i)) == 0)
            {
                count++;
            }
        }
        return count;
    }

    string QuaternionToDir(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;
        Vector2 dir = new Vector2(Mathf.Cos(euler.y * Mathf.PI / 180), Mathf.Sin(euler.y * Mathf.PI / 180));
        if (dir.x > 0.5)
            return "rig";
        if (dir.x < -0.5)
            return "lef";
        if (dir.y < -0.5)
            return "top";
        return "bot";
    }

    void placeDoor(Transform doors, Vector2 position, Quaternion roomRotation)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 direction = intToVector2(i);
            Quaternion dir = intToQuaternion(i);
            dir = roomRotation * dir;
            string doorDir = QuaternionToDir(dir);
            if (doors == null)
            {
                continue;
            }
            GameObject door = doors.Find("wall-" + doorDir)?.gameObject;
            if (door == null)
            {
                continue;
            }
            if (isAvailable(position + direction))
            {
                door.SetActive(true);
            }
            else
            {
                door.SetActive(false);
            }
        }
    }
    Vector2 FindFarthestPosition()
    {
        Vector2 farthestPos = Vector2.zero;
        float maxDistance = 0f;

        foreach (Room room in rooms) {
            float distance = Vector2.Distance(room.position, Vector2.zero);
            if (distance > maxDistance) {
                maxDistance = distance;
                farthestPos = room.position;
            }
            if (room.isDoubleRoom) {
                float secondDistance = Vector2.Distance(room.secondPosition, Vector2.zero);
                if (secondDistance > maxDistance) {
                    maxDistance = secondDistance;
                    farthestPos = room.secondPosition;
                }
            }
        }
        return farthestPos;
    }

    void disableDoors(Transform doors, Vector2 position, Quaternion roomRotation)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 direction = intToVector2(i);
            Quaternion dir = intToQuaternion(i);
            dir = roomRotation * dir;
            string doorDir = QuaternionToDir(dir);
            if (doors == null)
            {
                continue;
            }
            GameObject door = doors.Find("door-" + doorDir)?.gameObject;
            if (door == null)
            {
                continue;
            }
            OpenDoor openDoor = door.GetComponent<OpenDoor>();
            if (isAvailable(position + direction)) {
                openDoor?.disableDoor();
            } else {
                openDoor?.Open();
            }
        }
    }

    void placeDoors()
    {
        foreach (Room room in rooms)
        {
            Vector2 position = room.position;
            Quaternion roomRotation = room.roomObject.transform.rotation;
            roomRotation = Quaternion.Euler(0, 360 - (roomRotation.eulerAngles.y + 180), 0);
            if (room.isDoubleRoom)
            {
                Transform doors = room.roomObject.transform.Find("WALLDOORS");
                Transform dis = room.roomObject.transform.Find("DOORS");
                placeDoor(doors, position, roomRotation);
                disableDoors(dis, position, roomRotation);
                Transform secondDoors = room.roomObject.transform.Find("WALLDOORS2");
                Transform secondDis = room.roomObject.transform.Find("DOORS2");
                placeDoor(secondDoors, room.secondPosition, roomRotation);
                disableDoors(secondDis, position, roomRotation);
            }
            else
            {
                Transform doors = room.roomObject.transform.Find("WALLDOORS");
                Transform dis = room.roomObject.transform.Find("DOORS");
                placeDoor(doors, position, roomRotation);
                disableDoors(dis, position, roomRotation);
            }
        }
    }

    public bool SpawnEnemies(GameObject roomObj)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || !enemySpawningEnabled) return false;
        if (roomObj == null)
        {
            Debug.LogWarning("Room object is null. Cannot spawn enemies.");
            return false;
        }
        // check if it's the first room
        if (roomObj.transform.position == Vector3.zero)
        {
            Debug.Log("Skipping enemy spawn in the start room.");
            return false;
        }

        int enemyCount = Random.Range(2, 4);
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            Vector3 randomPos = roomObj.transform.position + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));

            GameObject enemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
            enemy.transform.parent = roomObj.transform;
        }
        return true;
    }

    void SetSpawnersActive(GameObject room, bool state)
    {
        foreach (EnemySpawner spawner in room.GetComponentsInChildren<EnemySpawner>())
        {
            spawner.enabled = state;

            if (!state)
                spawner.StopSpawning();
        }
    }

    void Awake()
    {
        createRoom(Vector2.zero, startRoomPrefab);

        while (rooms.Count < numberOfRooms)
        {
            Room room = rooms[Random.Range(0, rooms.Count)];
            Vector2 direction = getRandomDirection();
            Vector2 position = (room.isDoubleRoom ? room.secondPosition : room.position) + direction;
            if (isAvailable(position))
            {
                int valid = placeForDoubleRoom(position);
                int neighbours = numberOfNeighbours(valid);
                if (neighbours != 1)
                    continue;
                if (Random.value < doubleRoomChance && valid != 0)
                {
                    createDoobleRoom(position, valid);
                }
                else
                {
                    createRoom(position, roomPrefab);
                }
            }
        }
        AddBossRoom();
        placeDoors();
        SpawnBoss();
    }


    private void AddBossRoom()
    {

        float maxDist = -1f;
        Room farthestRoom = null;
        foreach (Room room in rooms)
        {
            if (room.position == Vector2.zero) continue;
            float dist = Vector2.Distance(Vector2.zero, room.position);
            if (dist > maxDist)
            {
                maxDist = dist;
                farthestRoom = room;
            }
        }
        if (farthestRoom == null)
        {
            Debug.LogWarning("No valid room found for boss spawn.");
            return;
        }
        Destroy(farthestRoom.roomObject);
        Vector2 farthestPosition = farthestRoom.position;
        rooms.Remove(farthestRoom);
        

        int valid = placeForDoubleRoom(farthestPosition);
        createDoobleRoom(farthestPosition, valid, bossRoomPrefab);
    }

    void SpawnBoss()
    {
        float maxDist = -1f;
        Room farthestRoom = null;
        foreach (Room room in rooms)
        {
            if (room.position == Vector2.zero) continue;
            float dist = Vector2.Distance(Vector2.zero, room.position);
            if (dist > maxDist)
            {
                maxDist = dist;
                farthestRoom = room;
            }
        }

        if (farthestRoom == null)
        {
            Debug.LogWarning("No valid room found for boss spawn.");
            return;
        }

        if (farthestRoom != null && bossPrefab != null)
        {
            Room room = farthestRoom;
            Vector3 bossPos = room.roomObject.transform.position;
            Instantiate(bossPrefab, bossPos, Quaternion.identity, room.roomObject.transform).transform.localPosition = Vector3.zero;
        }
    }

    public GameObject GetRoomAtPosition(Vector2 position)
    {
        foreach (Room room in rooms)
        {
            if (room.position == position || room.secondPosition == position)
            {
                return room.roomObject;
            }
        }
        return null;
    }

    public GameObject[] GetNeighbourRooms(Vector2 position)
    {
        Room baseRoom = null;
        foreach (Room room in rooms)
        {
            if (room.position == position || room.secondPosition == position)
            {
                baseRoom = room;
                break;
            }
        }
        if (baseRoom == null)
        {
            Debug.LogWarning("No room found at position: " + position);
            return new GameObject[0];
        }
        List<GameObject> neighbours = new List<GameObject>();
        Vector2[] directions = new Vector2[]
        {
            new Vector2(1, 0),   // Right
            new Vector2(-1, 0),  // Left
            new Vector2(0, 1),   // Up
            new Vector2(0, -1)   // Down
        };
        foreach (Vector2 direction in directions)
        {
            Vector2 neighbourPosition = baseRoom.position + direction;
            GameObject neighbourRoom = GetRoomAtPosition(neighbourPosition);
            if (neighbourRoom != null)
            {
                neighbours.Add(neighbourRoom);
            }
            if (baseRoom.isDoubleRoom)
            {
                Vector2 secondNeighbourPosition = baseRoom.secondPosition + direction;
                GameObject secondNeighbourRoom = GetRoomAtPosition(secondNeighbourPosition);
                if (secondNeighbourRoom != null)
                {
                    neighbours.Add(secondNeighbourRoom);
                }
            }
        }
        // remove itself from the list if it exists
        neighbours.Remove(baseRoom.roomObject);
        return neighbours.ToArray();
    }
}
