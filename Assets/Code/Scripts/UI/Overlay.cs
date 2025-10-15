using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    private Image overlay;
    public bool isAnimating = false;
    public Color start = Color.black;
    public Color end = new Color(0, 0, 0, 0);
    public float duration = 2f;
    public AudioSource music;

    private Animator animator;
    private Coroutine currentFade;

    void Start()
    {
        overlay = GetComponent<Image>();
        overlay.color = start;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isAnimating && currentFade == null)
        {
            currentFade = StartCoroutine(FadeOut());
        }
    }

    private void disableAnimator()
    {
        animator.enabled = false;
        isAnimating = true;
    }

    private IEnumerator FadeOut()
    {
        float t = 0.0f;
        float initialVolume = music != null ? music.volume : 1f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            overlay.color = Color.Lerp(start, end, t);

            if (music != null)
            {
                music.volume = Mathf.Lerp(initialVolume, 0f, t);
            }

            yield return null;
        }

        overlay.color = end;
        if (music != null)
        {
            music.volume = 0f;
        }

        isAnimating = false;
        currentFade = null;
    }


    public bool isComplete()
    {
        return overlay.color == end;
    }

    public void startFade()
    {
        disableAnimator();
        isAnimating = true;
    }
}
