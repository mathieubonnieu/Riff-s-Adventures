using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniRoom : MonoBehaviour
{
    private GameObject Ground;
    public Material UnvisitedGroundMaterial;
    public Material VisitedGroundMaterial;

    public Material OutlineMaterial;

    private Camera mainCamera;

    bool wasCurrenRoom = false;
    void Start()
    {
        Ground = transform.GetChild(0).Find("ground").gameObject;
        if (Ground == null)
        {
            Debug.LogError("Ground object not found in MiniRoom.");
            return;
        }
        SetUnvisited();
        mainCamera = Camera.main;
    }

    public void SetVisited(bool visited = true)
    {
        if (Ground == null)
        {
            Debug.LogError("Ground object is null in MiniRoom.");
            return;
        }

        Ground.GetComponent<Renderer>().material = visited ? VisitedGroundMaterial : UnvisitedGroundMaterial;
    }

    public void SetUnvisited()
    {
        SetVisited(false);
    }

    private bool isCurrentRoom()
    {
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow == null || cameraFollow.CurrentRoom == null)
        {
            return false;
        }

        return cameraFollow.CurrentRoom.name == gameObject.name;
    }

    private void SetOutlineToWalls(bool on = true)
    {
        Transform wallsParent = transform.GetChild(0);
        foreach (Transform child in wallsParent)
        {
            if (child.name.StartsWith("Wall"))
            {
                Renderer wallRenderer = child.GetComponent<Renderer>();
                if (wallRenderer != null)
                {
                    // set secondary material to outline
                    Material[] materials = wallRenderer.materials;
                    if (materials.Length > 1)
                    {
                        materials[0] = on ? OutlineMaterial : materials[1];
                        wallRenderer.materials = materials;
                    }
                    else
                    {
                        Debug.LogWarning("Wall renderer does not have a secondary material slot.");
                    }
                }
            }
        }
    }

    void Update()
    {
        if (isCurrentRoom() && !wasCurrenRoom)
        {
            wasCurrenRoom = true;
            SetVisited(true);
            SetOutlineToWalls();
        }
        else if (!isCurrentRoom() && wasCurrenRoom)
        {
            wasCurrenRoom = false;
            SetOutlineToWalls(false);
        }
    }
}
