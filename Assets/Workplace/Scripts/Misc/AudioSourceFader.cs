using System.Collections;
using UnityEngine;

/// <summary>
/// AudioSource wrapper that provides fade-in/fade-out functionality for Play, PlayOneShot, Stop, Pause, and UnPause operations.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioSourceFader : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private AudioSource audioSource;
    private float targetVolume = 1f;
    private Coroutine currentFadeCoroutine;
    private bool isPlayed = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        targetVolume = audioSource.volume;
    }

    #region Play Methods

    /// <summary>
    /// Plays the audio source with fade-in effect.
    /// </summary>
    public void Play()
    {
        StopCurrentFade();
        audioSource.volume = 0f;
        audioSource.Play();
        isPlayed = true;
        isPlayed = true;
        currentFadeCoroutine = StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Plays the audio source with fade-in effect at a specified time.
    /// </summary>
    public void Play(ulong delay)
    {
        StopCurrentFade();
        audioSource.volume = 0f;
        audioSource.Play(delay);
        isPlayed = true;
        currentFadeCoroutine = StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Plays the AudioClip at a specific time on the absolute time-line with fade-in effect.
    /// </summary>
    public void PlayScheduled(double time)
    {
        StopCurrentFade();
        audioSource.volume = 0f;
        audioSource.PlayScheduled(time);
        isPlayed = true;
        currentFadeCoroutine = StartCoroutine(FadeIn());
    }

    public void PlayOrUnPause()
    {
        if (isPlayed)
        {
            UnPause();
        }
        else
        {
            Play();
        }
    }

    /// <summary>
    /// Plays an AudioClip, and scales the AudioSource volume by volumeScale with fade-in effect.
    /// </summary>
    public void PlayOneShot(AudioClip clip)
    {
        PlayOneShot(clip, 1f);
    }

    /// <summary>
    /// Plays an AudioClip, and scales the AudioSource volume by volumeScale with fade-in effect.
    /// </summary>
    public void PlayOneShot(AudioClip clip, float volumeScale)
    {
        if (clip == null)
            return;

        StopCurrentFade();

        // For PlayOneShot, we'll fade in the overall volume but play the one-shot at the target volume scale
        float startVolume = audioSource.volume;
        audioSource.PlayOneShot(clip, volumeScale);

        if (startVolume < targetVolume)
        {
            currentFadeCoroutine = StartCoroutine(FadeIn());
        }
    }

    /// <summary>
    /// Plays the audio source delayed by the specified number of seconds with fade-in effect.
    /// </summary>
    public void PlayDelayed(float delay)
    {
        StopCurrentFade();
        audioSource.volume = 0f;
        audioSource.PlayDelayed(delay);
        StartCoroutine(DelayedFadeIn(delay));
    }

    #endregion

    #region Stop Methods

    /// <summary>
    /// Stops playing the clip with fade-out effect.
    /// </summary>
    public void Stop()
    {
        StopCurrentFade();
        currentFadeCoroutine = StartCoroutine(FadeOutAndStop());
    }

    /// <summary>
    /// Changes the time at which a sound that has already been scheduled to play will end with fade-out effect.
    /// </summary>
    public void SetScheduledEndTime(double time)
    {
        audioSource.SetScheduledEndTime(time);
        // Calculate remaining time and fade out accordingly
        double currentDspTime = AudioSettings.dspTime;
        double remainingTime = time - currentDspTime;

        if (remainingTime > fadeOutDuration)
        {
            StartCoroutine(DelayedFadeOut((float)(remainingTime - fadeOutDuration)));
        }
        else
        {
            StopCurrentFade();
            currentFadeCoroutine = StartCoroutine(FadeOut());
        }
    }

    #endregion

    #region Pause Methods

    /// <summary>
    /// Pauses playing the clip with fade-out effect.
    /// </summary>
    public void Pause()
    {
        StopCurrentFade();
        currentFadeCoroutine = StartCoroutine(FadeOutAndPause());
    }

    /// <summary>
    /// Unpauses the paused playback of this AudioSource with fade-in effect.
    /// </summary>
    public void UnPause()
    {
        StopCurrentFade();
        audioSource.volume = 0f;
        audioSource.UnPause();
        currentFadeCoroutine = StartCoroutine(FadeIn());
    }

    #endregion

    #region Fade Coroutines

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        float startVolume = audioSource.volume;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeInDuration);
            float curveValue = fadeInCurve.Evaluate(t);
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, curveValue);
            yield return null;
        }

        audioSource.volume = targetVolume;
        currentFadeCoroutine = null;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float startVolume = audioSource.volume;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeOutDuration);
            float curveValue = fadeOutCurve.Evaluate(t);
            audioSource.volume = Mathf.Lerp(startVolume, 0f, curveValue);
            yield return null;
        }

        audioSource.volume = 0f;
        currentFadeCoroutine = null;
    }

    private IEnumerator FadeOutAndStop()
    {
        yield return StartCoroutine(FadeOut());
        audioSource.Stop();
    }

    private IEnumerator FadeOutAndPause()
    {
        yield return StartCoroutine(FadeOut());
        audioSource.Pause();
    }

    private IEnumerator DelayedFadeIn(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentFadeCoroutine = StartCoroutine(FadeIn());
    }

    private IEnumerator DelayedFadeOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentFadeCoroutine = StartCoroutine(FadeOut());
    }

    #endregion

    #region Helper Methods

    private void StopCurrentFade()
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
        }
    }

    /// <summary>
    /// Updates the target volume. This is the volume that will be faded to.
    /// </summary>
    public void SetTargetVolume(float volume)
    {
        targetVolume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Gets the target volume.
    /// </summary>
    public float GetTargetVolume()
    {
        return targetVolume;
    }

    /// <summary>
    /// Sets the fade-in duration.
    /// </summary>
    public void SetFadeInDuration(float duration)
    {
        fadeInDuration = Mathf.Max(0f, duration);
    }

    /// <summary>
    /// Sets the fade-out duration.
    /// </summary>
    public void SetFadeOutDuration(float duration)
    {
        fadeOutDuration = Mathf.Max(0f, duration);
    }

    /// <summary>
    /// Sets the fade-in curve.
    /// </summary>
    public void SetFadeInCurve(AnimationCurve curve)
    {
        fadeInCurve = curve;
    }

    /// <summary>
    /// Sets the fade-out curve.
    /// </summary>
    public void SetFadeOutCurve(AnimationCurve curve)
    {
        fadeOutCurve = curve;
    }

    /// <summary>
    /// Gets the underlying AudioSource component.
    /// </summary>
    public AudioSource GetAudioSource()
    {
        return audioSource;
    }

    #endregion

    #region AudioSource Property Passthrough

    // Expose commonly used AudioSource properties for convenience
    public AudioClip clip
    {
        get => audioSource.clip;
        set => audioSource.clip = value;
    }

    public float volume
    {
        get => targetVolume;
        set
        {
            targetVolume = Mathf.Clamp01(value);
            audioSource.volume = targetVolume;
        }
    }

    public float pitch
    {
        get => audioSource.pitch;
        set => audioSource.pitch = value;
    }

    public bool loop
    {
        get => audioSource.loop;
        set => audioSource.loop = value;
    }

    public bool isPlaying => audioSource.isPlaying;

    public float time
    {
        get => audioSource.time;
        set => audioSource.time = value;
    }

    public int timeSamples
    {
        get => audioSource.timeSamples;
        set => audioSource.timeSamples = value;
    }

    public bool mute
    {
        get => audioSource.mute;
        set => audioSource.mute = value;
    }

    #endregion
}
