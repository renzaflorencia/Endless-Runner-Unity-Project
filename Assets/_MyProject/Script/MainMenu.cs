using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer; 
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        LoadVolume();
        Time.timeScale = 1;

        // Memainkan musik
        MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GamePlay");
        MusicManager.Instance.PlayMusic("GamePlay");
    }

    public void UpdateMusicVolume(float volume)
    {   
        audioMixer.SetFloat("MusicVol", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFXVol", volume);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVol", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVol", musicVolume);

        audioMixer.GetFloat("SFXVol", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVol", sfxVolume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
    }

    public void Quit()
    {
        Application.Quit();
    }
}