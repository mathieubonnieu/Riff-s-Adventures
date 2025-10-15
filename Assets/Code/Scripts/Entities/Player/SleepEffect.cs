using UnityEngine;

public class PlayerSleepEffect : MonoBehaviour
{
    private bool isSleeping = false;
    private float sleepTimer = 0f;

    public void TriggerSleep(float duration)
    {
        if (!isSleeping)
        {
            isSleeping = true;
            sleepTimer = duration;
            // Tu peux désactiver le mouvement ici si besoin
            GetComponent<PlayerMovement>().enabled = false;
        }
    }

    void Update()
    {
        if (isSleeping)
        {
            sleepTimer -= Time.deltaTime;
            if (sleepTimer <= 0f)
            {
                isSleeping = false;
                GetComponent<PlayerMovement>().enabled = true;
            }
        }
    }
}
