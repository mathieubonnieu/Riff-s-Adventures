

using UnityEngine;

class HealItem : ItemBase
{
    public override void OnItemPickup(GameObject player)
    {
        PlayerLife playerLife = player.GetComponent<PlayerLife>();
        if (playerLife != null)
        {
            playerLife.Heal(25);
        }
    }
}