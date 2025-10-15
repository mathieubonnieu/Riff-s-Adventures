using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Playsound : MonoBehaviour
{
    [System.Serializable]
    public class NamedClip
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(-3f, 3f)] public float pitch = 1f;
    }

    [Header("Sound Clip Settings")]
    public List<NamedClip> clips = new List<NamedClip>();

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Play a sound by name (exact match).
    /// </summary>
    public void PlaySound(string name)
    {
        if (audioSource == null || string.IsNullOrEmpty(name))
            return;

        var match = clips.Find(c => c.name == name);
        if (match != null && match.clip != null)
        {
            audioSource.pitch = match.pitch;
            audioSource.PlayOneShot(match.clip, match.volume);
        }
        else
        {
            Debug.LogWarning($"No sound named '{name}' found on {gameObject.name}");
        }
    }

    /// <summary>
    /// Play a sound by name (exact match) on a detached game object.
    /// </summary>
    public void PlaySoundDetached(string name)
    {
        var match = clips.Find(c => c.name == name);
        if (match == null || match.clip == null)
        {
            Debug.LogWarning($"No sound named '{name}' found on {gameObject.name}");
            return;
        }

        GameObject tempAudio = new GameObject("TempAudio_" + name);
        tempAudio.transform.position = transform.position;

        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = match.clip;
        tempSource.volume = match.volume;
        tempSource.pitch = match.pitch;
        tempSource.spatialBlend = 1f; // Optional: make it 3D
        tempSource.Play();

        Destroy(tempAudio, match.clip.length / match.pitch);
    }

    /// <summary>
    /// Play a sound by name (exact match) and loop it.
    /// </summary>
    public void PlayLoopingSound(string name)
    {
        var clipData = clips.Find(c => c.name == name);
        if (clipData != null && clipData.clip != null)
        {
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = clipData.clip;
            audioSource.volume = clipData.volume;
            audioSource.pitch = clipData.pitch;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Play a sound by name (exact match) detach and loop it.
    /// </summary>
    public GameObject PlayLoopingSoundDetached(string name)
    {
        var clipData = clips.Find(c => c.name == name);
        if (clipData == null || clipData.clip == null) return null;

        GameObject audioObject = new GameObject("LoopingSound_" + name);
        audioObject.transform.SetParent(transform); // Follow parent transform
        audioObject.transform.localPosition = Vector3.zero;

        var source = audioObject.AddComponent<AudioSource>();
        source.clip = clipData.clip;
        source.volume = clipData.volume;
        source.pitch = clipData.pitch;
        source.loop = true;
        source.spatialBlend = 1f; // 3D sound
        source.Play();

        return audioObject;
    }

    /// <summary>
    /// Stop the current playing sound and destroy it.
    /// </summary>
    public void StopLoopingSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource);
        }
    }
}
