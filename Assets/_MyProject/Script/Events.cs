using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Events : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject HowToPlay;
    [SerializeField] private GameObject HTPButton;
    [SerializeField] private PlayerManager PlayerManager;

    private bool isPaused = false;

    private void Update()
    {
        // Jika tombol Escape ditekan
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (PlayerManager.IsGameStarted)
        {
            HTPButton.SetActive(false);
        }
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Pause()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0;
            isPaused = true;
        }
    }

    public void Resume()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            isPaused = false;
        }
    }

    public void Tutorial()
    {
        if (!PlayerManager.IsGameStarted)
        {
            HowToPlay.SetActive(true);
            Time.timeScale = 1;
        }
    }
}