using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [System.Serializable]
    public struct stats
    {
        public float damage;
        public float speed;
        public float life;
        public float knockback;
        public float attackSpeed;
    }
    public stats playerStats;
    private List<ItemBase> activeItems = new List<ItemBase>();

    void Start()
    {
    }

    public void AddItem(ItemBase item)
    {
        if (item != null)
        {
            activeItems.Add(item);
            item.OnItemPickup(gameObject);
        }
    }

    public stats GetModifiedStats()
    {
        stats modifiedStats = playerStats;

        foreach (ItemBase item in activeItems)
        {
            modifiedStats.damage += item.statsModifier.damage;
            modifiedStats.speed += item.statsModifier.speed;
            modifiedStats.life += item.statsModifier.life;
            modifiedStats.knockback += item.statsModifier.knockback;
            modifiedStats.attackSpeed += item.statsModifier.attackSpeed;
        }

        return modifiedStats;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemBase item = other.GetComponent<ItemBase>();
            if (item != null)
            {
                AddItem(item);
                Destroy(other.gameObject);
            }
        }
    }

    
    private void Update()
    {
        foreach (ItemBase item in activeItems)
        {
            item.UpdateItem(gameObject);
        }
    }

    public void OnItemPickup(GameObject player)
    {
        foreach (ItemBase item in activeItems)
        {
            item.OnItemPickup(player);
        }
    }
    
    public void OnHit(GameObject enemy)
    {
        foreach (ItemBase item in activeItems)
        {
            item.OnHit(enemy);
        }
    }

    public void OnAttack(GameObject player)
    {
        foreach (ItemBase item in activeItems)
        {
            item.OnAttack(player);
        }
    }

    public void OnGetHit(GameObject player)
    {
        foreach (ItemBase item in activeItems)
        {
            item.OnGetHit(player);
        }
    }

    public void OnDodge(GameObject player)
    {
        foreach (ItemBase item in activeItems)
        {
            item.OnDodge(player);
        }
    }
}
