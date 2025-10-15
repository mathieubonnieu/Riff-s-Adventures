using UnityEngine;
public class SleepWave : MonoBehaviour
{
    public float speed = 5f;
    public float duration = 5f;

    private Vector3 direction;
    private Rigidbody rb;
    private bool hasLaunched = false;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        hasLaunched = true;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // On désactive la gravité pour que l'onde reste droite
    }

    private void Start()
    {
        GetComponent<Playsound>()?.PlaySoundDetached("launch");
        GetComponent<Playsound>()?.PlayLoopingSoundDetached("living");
    }

    private void FixedUpdate()
    {
        if (hasLaunched)
        {
            rb.velocity = direction * speed;
        }

        duration -= Time.fixedDeltaTime;
        if (duration <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSleepEffect sleep = other.GetComponent<PlayerSleepEffect>();
            if (sleep != null)
            {
                sleep.TriggerSleep(1f);
            }
            GetComponent<Playsound>()?.PlaySoundDetached("impact");
            PlayerLife playerLife = other.GetComponent<PlayerLife>();
            if (playerLife != null)
            {
                playerLife.TakeDamage(10);
            }

            Destroy(gameObject);
        }
    }
}
