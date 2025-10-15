using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BaseSoundPlayer : MonoBehaviour
{
    [System.Serializable]
    public class SoundClipData
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(-3f, 3f)] public float pitch = 1f;
    }

    [Header("Sound Settings")]
    public SoundClipData[] clips;

    [Tooltip("Time in seconds between sounds. Set to -1 to disable interval restriction.")]
    public float interval = 0.5f;

    [Tooltip("Should the sounds loop at the specified interval regardless of condition?")]
    public bool loop = false;

    protected AudioSource audioSource;
    protected float timer = 0f;
    private int lastClipIndex = -1;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        timer = 0f;
    }

    protected virtual void Update()
    {
        timer -= Time.deltaTime;

        if (ShouldPlaySound())
        {
            if (interval < 0f || timer <= 0f)
            {
                PlaySound();
                timer = interval;
            }
        }
    }

    protected virtual bool ShouldPlaySound()
    {
        return loop;
    }

    protected void PlaySound()
    {
        if (clips.Length == 0) return;

        int index;
        do
        {
            index = Random.Range(0, clips.Length);
        } while (index == lastClipIndex && clips.Length > 1);

        lastClipIndex = index;

        var data = clips[index];
        audioSource.pitch = data.pitch;
        audioSource.PlayOneShot(data.clip, data.volume);
    }
}
