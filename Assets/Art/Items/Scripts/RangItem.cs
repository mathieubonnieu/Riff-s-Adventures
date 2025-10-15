

using UnityEngine;

class RangeItem : ItemBase
{
    GameObject[] weapons;
    Vector3[] originalScales;
    public float range = 1.5f;
    private Animator animator;

    private PlayerActions playerActions;

    public override void OnItemPickup(GameObject player)
    {
        animator = player.GetComponent<Animator>();
        weapons = GameObject.FindGameObjectsWithTag("Weapon");
        playerActions = player.GetComponent<PlayerActions>();
        originalScales = new Vector3[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            GameObject weapon = weapons[i];
            originalScales[i] = weapon.transform.localScale;
            Vector3 targetScale = new Vector3(range * originalScales[i].x, range * originalScales[i].y, range * originalScales[i].z);
            weapon.transform.localScale = targetScale;
        }
    }
    
    public override void UpdateItem(GameObject player)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            GameObject weapon = weapons[i];
            Vector3 scale = weapon.transform.localScale;
            Vector3 originalScale = originalScales[i];
            Vector3 targetScale = new Vector3(range * originalScale.x, range * originalScale.y, range * originalScale.z);
            if (playerActions.isAttacking && animator.GetCurrentAnimatorStateInfo(0).normalizedTime * 100 < 60)
            {
                scale = Vector3.Lerp(scale, targetScale, Time.deltaTime * 10);
            }
            else
            {
                scale = Vector3.Lerp(scale, originalScale, Time.deltaTime * 10);
            }
            weapon.transform.localScale = scale;
        }
    }
}