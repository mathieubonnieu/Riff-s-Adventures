using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public GameObject explosionPrefab;
    public GameObject fireZonePrefab;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
            {
                // Instantiate(explosionPrefab, hit.point, Quaternion.identity);
                Instantiate(fireZonePrefab, hit.point, Quaternion.identity);
                Destroy(gameObject); // Destroy the meteor after it hits the ground
            }
        }  
    }
}
