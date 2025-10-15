using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeteorShower : MonoBehaviour
{
    public float radius = 5f; // Radius of the meteor shower area

    public float interval = 1f;
    private float nextSpawnTime = 1f;
    public GameObject meteorPrefab; // Prefab of the meteor to spawn
    public float meteorSpeed = 10f; // Speed of the meteor
    public float meteorRotationSpeed = 100f; // Speed of the meteor rotation
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        nextSpawnTime -= Time.deltaTime;
        if (nextSpawnTime <= 0f)
        {
            SpawnMeteor();
            nextSpawnTime += interval;
        }
    }

    void SpawnMeteor()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomPosition = transform.position;
        randomPosition.x = transform.position.x + randomCircle.x;
        randomPosition.z = transform.position.z + randomCircle.y;

        GameObject meteor = Instantiate(meteorPrefab, randomPosition, meteorPrefab.transform.rotation);
        meteor.GetComponent<GoForward>().speed = meteorSpeed;
        meteor.GetComponentInChildren<RandomRotate>().speed = meteorRotationSpeed;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, radius);
    }
}
