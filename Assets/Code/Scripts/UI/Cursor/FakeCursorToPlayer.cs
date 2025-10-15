using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class SmartFakeCursor : MonoBehaviour
{

    public GameObject player;
    public float idleTimeThreshold = 0.5f; // Temps avant de commencer à diminuer l'opacité
    public float fadeOutDuration = 0.5f; // Durée de la transition d'opacité

    private RectTransform rectTransform;
    private Image cursorImage; // Référence à l'image du curseur
    private Vector2 lastMousePosition;
    private bool isCursorIdle = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player GameObject not found. Please assign it in the inspector or ensure it has the 'Player' tag.");
            }
        }
        rectTransform = GetComponent<RectTransform>();
        cursorImage = GetComponent<Image>(); // Assurez-vous que votre curseur a un composant Image
        if (rectTransform == null || cursorImage == null)
        {
            Debug.LogError("RectTransform or Image component not found on this GameObject. Please ensure it has both.");
        }
        // cursor invisibility
        Cursor.visible = false;
        lastMousePosition = Mouse.current.position.ReadValue();
    }

    Vector2 getPlayerPosition()
    {
        if (player == null)
        {
            Debug.LogError("Player GameObject is not assigned or found.");
            return Vector2.zero;
        }
        Cursor.visible = false;

        // Get the player's position in world space
        Vector3 playerPosition = player.transform.position;

        // Convert the player's world position to screen position
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(playerPosition);

        return screenPosition;
    }

    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (mousePosition != lastMousePosition)
        {
            lastMousePosition = mousePosition;
            isCursorIdle = false;
            cursorImage.color = new Color(cursorImage.color.r, cursorImage.color.g, cursorImage.color.b, 1f); // Réinitialiser l'opacité
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine); // Arrêtez la coroutine de fadeOut si elle est en cours
            }
            fadeCoroutine = StartCoroutine(CheckIdleTime()); // Démarrez une nouvelle coroutine
        }

        // Set the position of the RectTransform to the world position
        rectTransform.position = mousePosition;
        // rotate the cursor to face the player
        if (player == null)
        {
            Debug.LogError("Player GameObject is not assigned or found.");
            return;
        }
        Vector2 playerScreenPosition = getPlayerPosition();
        Vector2 direction = playerScreenPosition - mousePosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f; // Adjusting angle to match the cursor's orientation
        rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        // reduce the size of the cursor if it's close to the player
        float distanceToPlayer = Vector2.Distance(mousePosition, getPlayerPosition());
        distanceToPlayer /= 500f; // Normalize the distance to a range suitable for scaling
        float scaleFactor = Mathf.Clamp(distanceToPlayer, 0.5f, 1f); // Scale between 0.5 and 1 based on distance
        rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }

    IEnumerator CheckIdleTime()
    {
        yield return new WaitForSeconds(idleTimeThreshold);
        isCursorIdle = true;
        fadeCoroutine = StartCoroutine(FadeOutCursor());
    }

    IEnumerator FadeOutCursor()
    {
        float elapsedTime = 0;
        Color initialColor = cursorImage.color;
        while (elapsedTime < fadeOutDuration)
        {
            float t = elapsedTime / fadeOutDuration;
            cursorImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(1f, 0.2f, t));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cursorImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0.2f);
    }
}
