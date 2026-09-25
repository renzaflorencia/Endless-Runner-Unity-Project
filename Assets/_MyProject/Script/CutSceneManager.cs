using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage displayScreen;
    [SerializeField] private GameObject cutscenePanel;
    [SerializeField] private PlayerManager playerManager;

    [Header("Audio Settings")]
    [SerializeField] private float musicFadeDuration = 0.5f;

    private bool isCutscenePlaying = false;
    private RenderTexture renderTexture; // Variabel untuk menyimpan render texture

    private void Start()
    {
        if (videoPlayer != null && videoPlayer.clip != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            // Diperbaiki: Membuat RenderTexture sekali dan menyimpannya untuk mencegah memory leak
            renderTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 24);
            videoPlayer.targetTexture = renderTexture;

            videoPlayer.loopPointReached += OnVideoComplete;
            videoPlayer.started += OnVideoStarted;

            if (displayScreen != null)
            {
                displayScreen.texture = renderTexture;
            }
        }

        if (cutscenePanel != null)
        {
            cutscenePanel.SetActive(false);
        }
    }

    public void PlayCutscene()
    {
        if (videoPlayer != null && cutscenePanel != null && !isCutscenePlaying)
        {
            isCutscenePlaying = true;
            StartCoroutine(StartCutsceneSequence());
        }
    }

    private IEnumerator StartCutsceneSequence()
    {
        if(MusicManager.Instance != null) {
            MusicManager.Instance.FadeOutMusic(musicFadeDuration);
        }

        yield return new WaitForSeconds(musicFadeDuration);

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PauseMusic();
        }

        cutscenePanel.SetActive(true);
        if (displayScreen != null) displayScreen.gameObject.SetActive(true);
        videoPlayer.Play();
        Time.timeScale = 1f;
    }

    private void OnVideoStarted(VideoPlayer vp)
    {
        Debug.Log("Cutscene Started");
    }

    private void OnVideoComplete(VideoPlayer vp)
    {
        Debug.Log("Cutscene Complete");
        isCutscenePlaying = false;
        StartCoroutine(EndCutsceneSequence());
    }

    private IEnumerator EndCutsceneSequence()
    {
        // Diperbaiki: 'SetAktive' menjadi 'SetActive'
        cutscenePanel.SetActive(false);
        if (displayScreen != null) displayScreen.gameObject.SetActive(false);

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ResumeMusic();
            MusicManager.Instance.FadeInMusic(musicFadeDuration);
        }

        yield return new WaitForSeconds(musicFadeDuration);
        if (playerManager != null)
        {
            playerManager.showWinPanel();
        }

    }

    private void OnDisable()
    {
        // Diperbaiki: 'NULL' menjadi 'null'
        if (MusicManager.Instance != null && isCutscenePlaying)
        {
            MusicManager.Instance.ResumeMusic();
            MusicManager.Instance.FadeInMusic(musicFadeDuration);
        }
    }

    
}