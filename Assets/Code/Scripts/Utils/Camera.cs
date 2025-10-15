using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public GameObject CurrentRoom;
    public float smoothSpeed = 0.125f;
    public float diff = 4.25f;

    public float projection = 1.0f;

    public bool playerFocus = false;

    private Vector4 zoneOffset = new Vector4(0, 0, 0, 0);

    private Vector3 fogCenter = new Vector3(0, 0, 0);

    public Material materialToUpdate;

    public float shaderOffset = 0.5f;

    private void Start()
    {
        materialToUpdate.SetVector("_Zone", zoneOffset);
    }

    Vector3[] GetBoxColliderCorners(BoxCollider box)
    {
        Transform t = box.transform;
        Vector3 center = box.center;
        Vector3 size = box.size * 0.5f;

        Vector3[] localCorners = new Vector3[]
        {
            new Vector3(center.x - size.x, center.y - size.y, center.z - size.z),
            new Vector3(center.x + size.x, center.y - size.y, center.z - size.z),
            new Vector3(center.x - size.x, center.y - size.y, center.z + size.z),
            new Vector3(center.x + size.x, center.y - size.y, center.z + size.z)
        };

        for (int i = 0; i < localCorners.Length; i++)
        {
            localCorners[i] = t.TransformPoint(localCorners[i]);
        }

        return localCorners;
    }

    void followPlayer()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    void fogToPlayer(Vector3[] corners)
    {
        float znDiff = shaderOffset;
        Vector4 zoneTarget = new Vector4(corners[3].x - znDiff, corners[3].z - znDiff, corners[0].x + znDiff, corners[0].z + znDiff);
        Vector3 zoneCenterTarget = new Vector3((zoneTarget.x + zoneTarget.z) / 2, 0, (zoneTarget.y + zoneTarget.w) / 2);
        Vector4 targetOffset = new Vector4(zoneTarget.x - zoneCenterTarget.x, zoneTarget.y - zoneCenterTarget.z, zoneTarget.z - zoneCenterTarget.x, zoneTarget.w - zoneCenterTarget.z);
        if (playerFocus)
        {
            float plOffset = 0.3f;
            zoneOffset = Vector4.Lerp(zoneOffset, new Vector4(plOffset, plOffset, -plOffset, -plOffset), Time.deltaTime * 5);
            fogCenter = Vector3.Lerp(fogCenter, player.position, Time.deltaTime * 5);
        }
        else
        {
            zoneOffset = Vector4.Lerp(zoneOffset, targetOffset, Time.deltaTime * 2);
            fogCenter = Vector3.Lerp(fogCenter, zoneCenterTarget, Time.deltaTime * 2);
        }

        Vector4 zone = new Vector4(zoneOffset.x, zoneOffset.y, zoneOffset.z, zoneOffset.w);
        zone.x += fogCenter.x;
        zone.y += fogCenter.z;
        zone.z += fogCenter.x;
        zone.w += fogCenter.z;


        materialToUpdate.SetVector("_Zone", zone);
    }

    void followPlayerInRoom(Vector3[] corners)
    {
        if (player == null) return;

        Vector3 playerPosition = player.position;
        System.Array.Sort(corners, (a, b) => (a.x + a.z).CompareTo(b.x + b.z));
        if (corners[1].x > corners[2].x)
        {
            Vector3 temp = corners[1];
            corners[1] = corners[2];
            corners[2] = temp;
        }
        float minX = corners[0].x + diff;
        float maxX = corners[3].x - diff;
        float minZ = corners[0].z + diff;
        float maxZ = corners[3].z - diff;

        float x = Mathf.Clamp(playerPosition.x, minX, maxX);
        float z = Mathf.Clamp(playerPosition.z, minZ, maxZ);
        Vector3 desiredPosition = new Vector3(x, playerPosition.y, z) + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    void LateUpdate()
    {
        if (CurrentRoom == null)
        {
            followPlayer();
            fogToPlayer(new Vector3[4] {
                new Vector3(-1000, 0, -1000),
                new Vector3(1000, 0, -1000),
                new Vector3(-1000, 0, 1000),
                new Vector3(1000, 0, 1000)
            });
            return;
        }
        BoxCollider box = CurrentRoom.GetComponent<BoxCollider>();

        if (box != null)
        {
            Vector3[] corners = GetBoxColliderCorners(box);
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i].y += 4.5f;
            }
            followPlayerInRoom(corners);
            fogToPlayer(corners);
        }
    }

    public void ZoomTo(float size)
    {
        StartCoroutine(ZoomToCoroutine(size, 0.75f));
    }

    private IEnumerator ZoomToCoroutine(float targetSize, float speed)
    {
        // get the current orthographic size
        float startSize = Camera.main.orthographicSize;

        while (targetSize != Camera.main.orthographicSize)
        {
            // interpolate the orthographic size
            Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetSize, speed * Time.deltaTime);
            yield return null;
        }
    }
}
