using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float lifetime = 2f;
    public Vector3 floatOffset = new Vector3(0, 2, 0);

    private float timer;
    private Transform cam;
    private TextMeshPro textMesh;
    private Color originalColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        cam = Camera.main.transform;
    }

    void Start()
    {
        transform.position += floatOffset;
        if (textMesh != null)
        {
            originalColor = textMesh.color;
        }
    }

    void Update()
    {
        if (textMesh == null) return;

        transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }
    }

    public void SetColor(Color color)
    {
        if (textMesh != null)
        {
            originalColor = color;
            textMesh.color = color;
        }
    }

    public void SetBold(bool bold)
    {
        if (textMesh != null)
        {
            textMesh.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        }
    }
}
