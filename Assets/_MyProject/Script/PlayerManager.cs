using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static bool GameOver;
    public GameObject GameOverPanel; 
    public GameObject WinPanel; // Panel untuk kondisi menang
    public GameObject HTPButton;
    public GameObject HUD;
    public GameObject startingText;
    public ScoreSystem scoreSystem;
    public HealthManager HealthManager; // Referensi ke HealthManager

    // Variabel untuk StartGame
    public static bool IsGameStarted;
    private bool hasWon = false;

    public PlayerControl playerControl;
    public Animator playerAnimator;

    // Start is called before the first frame update

    [SerializeField] private CutsceneManager cutscenemanager;

    void Start()
    {
        GameOver = false;
        Time.timeScale = 1; // Untuk replay
        IsGameStarted = false;
    }

    private void Update()
    {
        if (scoreSystem != null && scoreSystem.HasPlayerWon() && !hasWon)
        {
            hasWon = true;
            StartCoroutine(WinSequence());
        }

        if (GameOver && !hasWon)
        {
            StartCoroutine(GameOverSequence());
        }

        if (Input.GetKeyDown(KeyCode.Return) && !IsGameStarted)
        {
            playerControl.enabled = true;
            IsGameStarted = true;
            Destroy(startingText);

        }
    }

    // Metode publik untuk memulai game over dari skrip lain
    public void StartGameOverSequence()
    {
        if (GameOver || hasWon) return; // Jangan mulai jika sudah game over atau menang

        GameOver = true;
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        playerAnimator.SetTrigger("Die");
        yield return new WaitForSecondsRealtime(2f); // Menunggu animasi selesai

        Time.timeScale = 0;
        HUD.SetActive(false);
        GameOverPanel.SetActive(true);

        if (scoreSystem != null)
        {
            scoreSystem.DisplayGameOverScore();
        }
    }

    // Menggabungkan logika cutscene dan panel menang
    IEnumerator WinSequence()
    {
        if (cutscenemanager != null)
        {
            cutscenemanager.PlayCutscene();
        }
        yield return null;
    }

    internal void showWinPanel()
    {
        Time.timeScale = 0;

        if (HUD != null)
        {
            HUD.SetActive(false);
        }
        if (WinPanel != null)
        {
            WinPanel.SetActive(true);
        }
        if (scoreSystem != null)
        {
            scoreSystem.DisplayGameOverScore();
        }

        Debug.Log("pemain menang");
    }
}