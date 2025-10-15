using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniPlayer : MonoBehaviour
{
    private GameObject player;
    public Vector3 offest = new Vector3(50, 0, 50);
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Vector3 miniPlayerPosition = player.transform.position / 9 + offest;
        transform.position = miniPlayerPosition;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Keep the Y position flat
        transform.rotation = player.transform.rotation;
    }
}
