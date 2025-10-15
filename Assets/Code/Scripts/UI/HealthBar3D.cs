using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar3D : MonoBehaviour
{
    [Header("Position Settings")]
    public Vector3 offset;

    [Header("Image Overlay")]
    public Image fillImage;

    [Header("Default Health Bar Color")]
    public Color barColor;
    public Color barFlashColor;

    [Header("Default Damage Settings")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    private Vector3 position;
    private float health;
    private float maxHealth;
    private float lastHealth;

    private float initialWidth;

    private float currentHealthPercent = 1f;
    public float transitionSpeed = 5f;

    public void SetTarget(Vector3 targetPos, float targetHealth, float targetMaxHealth)
    {
        position = targetPos;
        health = targetHealth;
        maxHealth = targetMaxHealth;
    }

    public void SetHealthBarOffset(Vector3 targetOffset)
    {
        offset = targetOffset;
    }

    public void SetHealth(float targetHealth)
    {
        health = targetHealth;
    }

    public void SetBarColor(Color targetBarColor)
    {
        barColor = targetBarColor;
    }

    public void SetBarDamageColor(Color targetDamageColor)
    {
        barFlashColor = targetDamageColor;
    }

    public void SetShakeDuration(float duration)
    {
        shakeDuration = duration;
    }

    public void SetShakeMagnitude(float magnitude)
    {
        shakeMagnitude = magnitude;
    }

    void Start()
    {
        if (fillImage != null)
        {
            initialWidth = fillImage.rectTransform.sizeDelta.x;
            currentHealthPercent = 1f;
            fillImage.color = barColor;
        }
        lastHealth = maxHealth;
    }

    public void FadeAndDestroy(float duration)
    {
        StartCoroutine(FadeOutAndDestroyRoutine(duration));
    }

    private void UpdatePosition()
    {
        transform.position = position + offset;
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    private IEnumerator FadeOutAndDestroyRoutine(float duration)
    {
        float elapsed = 0f;
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        float startAlpha = canvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator ShakeAndFlash()
    {
        float elapsed = 0f;

        fillImage.color = barFlashColor;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = transform.position + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }

        fillImage.color = barColor;
    }

    void Update()
    {
        if (health != lastHealth)
        {
            StartCoroutine(ShakeAndFlash());
            lastHealth = health;
        }
        else
        {
            fillImage.color = barColor;
        }
        UpdatePosition();

        float targetHealthPercent = Mathf.Clamp01(health / maxHealth);

        currentHealthPercent = Mathf.Lerp(currentHealthPercent, targetHealthPercent, Time.deltaTime * transitionSpeed);

        if (fillImage != null)
        {
            RectTransform rt = fillImage.rectTransform;
            rt.sizeDelta = new Vector2(initialWidth * currentHealthPercent, rt.sizeDelta.y);
        }
    }
}
