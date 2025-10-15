using UnityEngine;

public class VideoUISliderAndZoom : MonoBehaviour
{
    public RectTransform videoUI;
    public float slideDistance = 100f;
    public float slideDuration = 2f;

    public float zoomDuration = 1f;      // Time to complete a full zoom-in and out cycle
    public float zoomScaleMin = 1f;      // Normal scale
    public float zoomScaleMax = 1.1f;    // Zoomed-in scale

    private Vector2 startPos;
    private Vector2 targetPos;
    private float slideElapsed = 0f;
    private bool isSliding = true;

    private float zoomTimer = 0f;
    private bool zoomingIn = true;

    void Start()
    {
        if (videoUI == null)
            videoUI = GetComponent<RectTransform>();

        startPos = videoUI.anchoredPosition;
        targetPos = startPos + new Vector2(slideDistance, 0f);
    }

    void Update()
    {
        if (isSliding)
        {
            slideElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(slideElapsed / slideDuration);
            videoUI.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            if (t >= 1f)
                isSliding = false;
        }
        else
        {
            AnimateZoom();
        }
    }

    void AnimateZoom()
    {
        zoomTimer += Time.deltaTime;
        float halfCycle = zoomDuration / 2f;

        float t = (zoomTimer % halfCycle) / halfCycle;
        float scale;

        if ((int)(zoomTimer / halfCycle) % 2 == 0)
        {
            // Zooming in
            scale = Mathf.Lerp(zoomScaleMin, zoomScaleMax, t);
        }
        else
        {
            // Zooming out
            scale = Mathf.Lerp(zoomScaleMax, zoomScaleMin, t);
        }

        videoUI.localScale = new Vector3(scale, scale, 1f);
    }
}
