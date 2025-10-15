using System.Collections;
using System.Collections.Generic;
using RPGCharacterAnims.Extensions;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLife : MonoBehaviour
{
    public int health = 200;

    private int maxHealth; // Defined from the health variable at the start of the game
    private bool isDead = false;
    private TriggerActions triggerActions;

    private PlayerStats playerStatsComponent;

    private float timer = 0f;

    public float invincibilityDuration = 1f; // Duration of invincibility after taking damage

    private FloatingTextSpawner floatingTextSpawner;

    public GameObject healParticle;

    PlayerActions playerActions;


    void Start()
    {
        triggerActions = GetComponent<TriggerActions>();
        floatingTextSpawner = GetComponent<FloatingTextSpawner>();
        maxHealth = health;
        playerStatsComponent = GetComponent<PlayerStats>();
        playerActions = GetComponent<PlayerActions>();
        if (playerStatsComponent != null)
        {
            PlayerStats.stats playerStats = playerStatsComponent.GetModifiedStats();
            maxHealth = (int)playerStats.life;
            health = maxHealth;
        }
    }

    void Update()
    {
        maxHealth = (int)playerStatsComponent.GetModifiedStats().life;
        if (isDead) return;

        timer += Time.deltaTime;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Prevent damage if already dead
        if (playerActions.isDodging) return; // Prevent damage while dodging
        if (timer < invincibilityDuration) return; // Prevent damage during invincibility
        health -= damage;
        floatingTextSpawner?.ShowFloatingText(damage, true);
        timer = 0f; // Reset timer when taking damage
        triggerActions.CancelCurrentTriggerAndActivate(8, 1);
        if (health <= 0)
        {
            CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
            cameraFollow.playerFocus = true;
            cameraFollow.CurrentRoom = null;
            cameraFollow.ZoomTo(1f);
            health = 0;

            Die();
        }
        else
        {
            GetComponent<Playsound>()?.PlaySound("hit" + Random.Range(1, 5));
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return; // Prevent healing if dead
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        if (healParticle != null)
        {
            GameObject healParticleInstance = Instantiate(healParticle, transform.position, Quaternion.identity);
            healParticleInstance.transform.SetParent(transform);
            Destroy(healParticleInstance, 1f);
        }
    }

    public void Die()
    {
        GetComponent<Playsound>()?.PlaySoundDetached("death");
        isDead = true;
        triggerActions.CancelCurrentTriggerAndActivate(6);
    }

    public void Revive()
    {
        isDead = false;
        health = 100;
        triggerActions.CancelCurrentTriggerAndActivate(7);
    }
}
