using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;

    private float originalVolume;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            originalVolume = musicSource.volume;
        }
    }

    public void PlayMusic(string trackName, float fadeDuration = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossfade(musicLibrary.GetClipFromName(trackName), fadeDuration));
    }

    public void PlayImportantSound(AudioClip clip, Vector3 pos)
    {
        StartCoroutine(DuckAndPlay(clip, pos));
    }

    private IEnumerator DuckAndPlay(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            float duckVolume = 0.2f;
            float fadeOutDuration = 0.5f;
            float fadeInDuration = 0.5f;

            // Fade out music volume
            yield return StartCoroutine(FadeVolume(duckVolume, fadeOutDuration));

            // Play the important sound
            AudioSource.PlayClipAtPoint(clip, pos);
            yield return new WaitForSeconds(clip.length);

            // Fade in music volume
            yield return StartCoroutine(FadeVolume(originalVolume, fadeInDuration));
        }
    }

    private IEnumerator FadeVolume(float targetVolume, float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    private IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float fadeDuration = 0.5f)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(1f, 0, percent);
            yield return null;
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

       percent = 0;
        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, 1f, percent);
            yield return null;
        }
    }

    public void FadeOutMusic(float duration = 0.5f)
    {
        StartCoroutine(FadeVolume(0f, duration));
    }

    public void FadeInMusic(float duration = 0.5f)
    {
        StartCoroutine(FadeVolume(originalVolume, duration));
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }
}
