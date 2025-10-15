using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashItem : ItemBase
{
    public GameObject slashPrefab;

    public override void OnAttack(GameObject player)
    {
        Instantiate(slashPrefab, player.transform.position, player.transform.rotation);
    }
}
