using UnityEngine;
using UnityEngine.Video;

public class CustomVideoLooper : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public double loopStartTime = 3.7;
    private bool hasLoopedOnce = false;
    private bool isSeeking = false;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnVideoEnded;
        videoPlayer.seekCompleted += OnSeekCompleted;

        videoPlayer.Play();
    }

    private void OnVideoEnded(VideoPlayer vp)
    {
        if (!hasLoopedOnce)
        {
            hasLoopedOnce = true;
        }

        isSeeking = true;
        vp.time = loopStartTime;
        vp.Prepare(); // Needed to trigger seekCompleted reliably
    }

    private void OnSeekCompleted(VideoPlayer vp)
    {
        if (isSeeking)
        {
            isSeeking = false;
            vp.Play();
        }
    }
}
