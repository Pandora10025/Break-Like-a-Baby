using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource AmbienceSource;
    [SerializeField] AudioSource SoundEffectsSource;
    [SerializeField] AudioSource FootStepsource;

    [Space]

    public AudioClip backgroundSFX;
    public AudioClip chaseSFX;
    public AudioClip weatherSFX;
    public AudioClip winSFX;
    public AudioClip interactSFX;
    public AudioClip taskSFX;
    public AudioClip breakSFX;
    public AudioClip gameOverSFX;
    public AudioClip hitSFX;
    public AudioClip startGameSFX;

    // Low-pass filter reference
    private AudioLowPassFilter lowPassFilter;

    public void Awake()
    {
        instance = this;

        // Setup music
        musicSource.clip = backgroundSFX;
        musicSource.Play();

        // Setup ambience
        AmbienceSource.clip = weatherSFX;
        AmbienceSource.Play();

        // Ensure the low-pass filter is on the musicSource
        lowPassFilter = FindAnyObjectByType<AudioLowPassFilter>();
    }

    public void PlaySFX(AudioClip clip)
    {
        SoundEffectsSource.PlayOneShot(clip);
    }

    public void PauseMusic()
    {
        musicSource.Stop();
    }
    public void SwitchMusic(AudioClip newClip)
    {
        if (musicSource != null && musicSource.clip != newClip)
        {
            StartCoroutine(FadeOutAndSwitch(newClip));
        }
    }

    private IEnumerator FadeOutAndSwitch(AudioClip newClip)
    {
        // Fade out current track
        float fadeDuration = 1f;  // Set your fade duration
        float startVolume = 0.7f;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = 0;
        musicSource.Stop();  // Stop the current music

        // Switch to new track
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in new track
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = startVolume;  // Reset to original volume
    }
    // Function to apply or remove low-pass filter effect
    public void ApplyLowPassFilter(bool applyFilter, float cutoffFrequency = 500f)
    {
        if (lowPassFilter != null)
        {
            lowPassFilter.enabled = applyFilter;  // Enable/disable the filter

            // Set the cutoff frequency when applying the filter
            if (applyFilter)
            {
                lowPassFilter.cutoffFrequency = cutoffFrequency;
            }
            else
            {
                lowPassFilter.cutoffFrequency = 22000f; // Reset to maximum frequency when the filter is off
            }
        }
    }
}