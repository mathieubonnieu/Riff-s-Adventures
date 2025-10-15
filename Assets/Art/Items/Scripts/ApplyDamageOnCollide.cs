using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyDamageOnCollide : MonoBehaviour
{
    public float damageAmount = 10f;
    void OnTriggerEnter(Collider other)
    {
        // if other inherit from EnemyBase
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            // Apply damage to the enemy
            enemy.TakeDamage(damageAmount, 1f);
        }
    }
}
