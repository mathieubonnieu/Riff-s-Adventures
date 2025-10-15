using UnityEngine;
using UnityEngine.UI;

public class HeartIcon : MonoBehaviour
{
    public PlayerLife playerLife;
    public Image HeartIconFull;
    public Image HeartIconEmpty;

    private Vector3 originalScale;
    private float pulseSpeed = 2f;
    private float pulseAmount = 0.1f;
    private RectTransform heartFullRect;

    void Start()
    {
        if (HeartIconFull == null || HeartIconEmpty == null)
        {
            Debug.LogError("UI: Heart images not assigned properly.");
            return;
        }

        heartFullRect = HeartIconFull.GetComponent<RectTransform>();
        originalScale = transform.localScale;
        HeartIconFull.type = Image.Type.Filled;
        HeartIconFull.fillMethod = Image.FillMethod.Vertical;
        HeartIconFull.fillOrigin = (int)Image.OriginVertical.Bottom;
        heartFullRect.pivot = new Vector2(0.5f, 0f); // Set pivot to bottom
    }

    void Update()
    {
        if (playerLife == null) return;

        float currentHealth = playerLife.GetHealth();
        float maxHealth = playerLife.GetMaxHealth();
        float healthPercent = currentHealth / maxHealth;

        SetHeartFill(healthPercent);
        if (healthPercent <= 0.3f)
        {
            float scale = 1 + Mathf.PingPong(Time.time * pulseSpeed, pulseAmount);
            HeartIconFull.transform.localScale = originalScale * scale;
            HeartIconEmpty.transform.localScale = originalScale * scale;
        }
        else
        {
            HeartIconFull.transform.localScale = originalScale;
            HeartIconEmpty.transform.localScale = originalScale;
        }
    }

    void SetHeartFill(float healthPercent)
    {
        healthPercent = Mathf.Clamp01(healthPercent);

        HeartIconFull.fillAmount = healthPercent;
    }
}
