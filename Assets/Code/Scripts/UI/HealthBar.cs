using System.Collections;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public PlayerLife playerLife;
    private TextMeshProUGUI health;
    private int lastHealth;
    private Coroutine feedbackCoroutine;
    private Color originalColor;

    private int currentHealth;
    private int maxHealth;

    void Start()
    {
        health = GetComponent<TextMeshProUGUI>();
        originalColor = health.color;
        lastHealth = playerLife.GetHealth();
        currentHealth = playerLife.GetHealth();
        maxHealth = currentHealth;
        UpdateHealthText();
    }

    void Update()
    {
        if (playerLife == null) return;

        currentHealth = playerLife.GetHealth();

        if (currentHealth != lastHealth)
        {
            UpdateHealthText();

            // Trigger feedback animation
            if (feedbackCoroutine != null)
                StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = StartCoroutine(HealthChangeFeedback());

            lastHealth = currentHealth;
        }
    }

    void UpdateHealthText()
    {
        health.SetText(currentHealth + " / " + maxHealth);
    }

    IEnumerator HealthChangeFeedback()
    {
        health.color = Color.red;

        Vector3 originalPos = transform.localPosition;
        float shakeDuration = 0.3f;
        float shakeMagnitude = 5f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        if (currentHealth != 0)
            health.color = originalColor;
    }
}
