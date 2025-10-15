using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundByAnimator : MonoBehaviour
{
    [Header("Sound Settings")]
    public bool useRandomClip = true;
    public bool loop = true;
    public float interval = 0.5f;

    public enum ConditionType
    {
        BoolTrue,
        BoolFalse,
        IntEquals,
        IntNotEqual,
        IntGreaterThan,
        IntLessThan
    }

    [System.Serializable]
    public class AnimatorCondition
    {
        public string parameterName;
        public ConditionType conditionType;
        public int intValue;
        [HideInInspector] public bool wasConditionMet;
    }

    [System.Serializable]
    public class ConditionalClip
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;

        public List<AnimatorCondition> conditions = new List<AnimatorCondition>();
    }

    [Header("Clips List")]
    public List<ConditionalClip> clips = new List<ConditionalClip>();

    [Header("Global Animator Conditions (used only when random is enabled)")]
    public List<AnimatorCondition> globalConditions = new List<AnimatorCondition>();

    private AudioSource audioSource;
    private Animator animator;
    private float timer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator == null || clips.Count == 0) return;

        if (useRandomClip)
        {
            bool allGlobalConditionsMet = AreAllConditionsMet(globalConditions);

            if (loop)
            {
                if (allGlobalConditionsMet)
                {
                    timer -= Time.deltaTime;
                    if (interval < 0 || timer <= 0f)
                    {
                        PlayRandomClip();
                        timer = interval;
                    }
                }
            }
            else
            {
                if (allGlobalConditionsMet && !WereConditionsPreviouslyMet(globalConditions))
                {
                    PlayRandomClip();
                }
            }

            UpdatePreviousStates(globalConditions);
        }
        else
        {
            foreach (var clip in clips)
            {
                bool allClipConditionsMet = AreAllConditionsMet(clip.conditions);

                if (loop)
                {
                    if (allClipConditionsMet)
                    {
                        timer -= Time.deltaTime;
                        if (interval < 0 || timer <= 0f)
                        {
                            PlayClip(clip);
                            timer = interval;
                        }
                    }
                }
                else
                {
                    if (allClipConditionsMet && !WereConditionsPreviouslyMet(clip.conditions))
                    {
                        PlayClip(clip);
                    }
                }

                UpdatePreviousStates(clip.conditions);
            }
        }
    }

    private void PlayRandomClip()
    {
        if (clips.Count == 0) return;
        var clip = clips[Random.Range(0, clips.Count)];
        PlayClip(clip);
    }

    private void PlayClip(ConditionalClip clip)
    {
        if (clip.clip == null || audioSource == null) return;
        audioSource.pitch = clip.pitch;
        audioSource.PlayOneShot(clip.clip, clip.volume);
    }

    private bool AreAllConditionsMet(List<AnimatorCondition> conditions)
    {
        foreach (var cond in conditions)
        {
            if (!EvaluateCondition(cond))
                return false;
        }
        return true;
    }

    private bool WereConditionsPreviouslyMet(List<AnimatorCondition> conditions)
    {
        foreach (var cond in conditions)
        {
            if (!cond.wasConditionMet)
                return false;
        }
        return true;
    }

    private void UpdatePreviousStates(List<AnimatorCondition> conditions)
    {
        foreach (var cond in conditions)
        {
            cond.wasConditionMet = EvaluateCondition(cond);
        }
    }

    private bool EvaluateCondition(AnimatorCondition condition)
    {
        if (animator == null || string.IsNullOrEmpty(condition.parameterName))
            return false;

        switch (condition.conditionType)
        {
            case ConditionType.BoolTrue:
                return animator.GetBool(condition.parameterName);
            case ConditionType.BoolFalse:
                return !animator.GetBool(condition.parameterName);
            case ConditionType.IntEquals:
                return animator.GetInteger(condition.parameterName) == condition.intValue;
            case ConditionType.IntNotEqual:
                return animator.GetInteger(condition.parameterName) != condition.intValue;
            case ConditionType.IntGreaterThan:
                return animator.GetInteger(condition.parameterName) > condition.intValue;
            case ConditionType.IntLessThan:
                return animator.GetInteger(condition.parameterName) < condition.intValue;
            default:
                return false;
        }
    }
}
