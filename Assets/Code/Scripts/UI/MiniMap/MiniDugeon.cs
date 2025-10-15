using UnityEngine;

public class MiniDungeon : MonoBehaviour
{
    private GameObject mainCamera;
    private CameraFollow cameraFollow;
    private GameObject currentRoom;
    public Vector3 offest = new Vector3(50, 0, 50);

    private DungeonGeneration dungeonGeneration;
    public GameObject miniRoom;
    public GameObject miniDoubleRoom;

    private GameObject[] miniRooms;
    void Start()
    {
        mainCamera = Camera.main.gameObject;
        cameraFollow = mainCamera.GetComponent<CameraFollow>();
        dungeonGeneration = FindObjectOfType<DungeonGeneration>();
        if (dungeonGeneration == null)
        {
            Debug.LogError("DungeonGeneration script not found in the scene.");
            return;
        }

        if (cameraFollow.CurrentRoom != null)
        {
            currentRoom = cameraFollow.CurrentRoom;
        }
        transform.position = mainCamera.transform.position / 9 + offest;
        miniRooms = new GameObject[0];
    }


    private void GetNeighboursRooms(GameObject room)
    {
        if (room == null)
            return;
        GameObject[] neighbours = dungeonGeneration.GetNeighbourRooms(new Vector2(room.transform.position.x / 9, room.transform.position.z / 9));

        foreach (GameObject neighbourRoom in neighbours)
        {
            if (neighbourRoom != null && neighbourRoom != room)
            {
                createMiniRoom(neighbourRoom);
            }
        }
    }

    private bool createMiniRoom(GameObject room)
    {
        foreach (GameObject miniRoom in miniRooms)
        {
            if (miniRoom.name == room.name)
            {
                return false;
            }
        }
        GameObject roomPrefab = room.name.StartsWith("Double") ? miniDoubleRoom : miniRoom;
        GameObject miniRoomInstance = Instantiate(roomPrefab, transform);
        miniRoomInstance.transform.position = room.transform.position / 9 + offest;
        miniRoomInstance.transform.rotation = room.transform.rotation;
        miniRoomInstance.name = room.name;

        GameObject[] newMiniRooms = new GameObject[miniRooms.Length + 1];
        for (int i = 0; i < miniRooms.Length; i++)
        {
            newMiniRooms[i] = miniRooms[i];
        }
        newMiniRooms[newMiniRooms.Length - 1] = miniRoomInstance;
        miniRooms = newMiniRooms;
        return true;
    }

    void SetToVisited(GameObject room)
    {
        if (room == null)
            return;

        foreach (GameObject miniRoom in miniRooms)
        {
            if (miniRoom.name == room.name)
            {
                miniRoom.GetComponent<MiniRoom>().SetVisited();
                return;
            }
        }
    }

    void Update()
    {
        if (cameraFollow.CurrentRoom != null && cameraFollow.CurrentRoom != currentRoom)
        {
            currentRoom = cameraFollow.CurrentRoom;
            createMiniRoom(currentRoom);
            GetNeighboursRooms(currentRoom);
        }
    }
}
