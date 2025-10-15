using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public PlayerStats.stats statsModifier;

    public virtual void UpdateItem(GameObject player)
    {
    }

    public virtual void OnItemPickup(GameObject player)
    {
    }

    public virtual void OnHit(GameObject enemy)
    {
    }

    public virtual void OnAttack(GameObject player)
    {
    }

    public virtual void OnGetHit(GameObject player)
    {
    }

    public virtual void OnDodge(GameObject player)
    {
    }
}
