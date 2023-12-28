using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Handles volume settings. Built for a small project so saving settings with PlayerPrefs instead of the usual Serialization system.
/// </summary>
public class VolumeManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    void Start() //Awake and OnEnable trigger too early
    {
        LoadVolume();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("musicVol", Mathf.Log10(volume)*20); //db scales logarithmically
        PlayerPrefs.SetFloat("musicVol", volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("sfxVol", Mathf.Log10(volume) * 20); //db scales logarithmically
        PlayerPrefs.SetFloat("sfxVol", volume);
    }


    void LoadVolume()
    {
        if(PlayerPrefs.HasKey("musicVol"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVol");
        }
        SetMusicVolume();

        if (PlayerPrefs.HasKey("sfxVol"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVol");
        }
        SetSFXVolume();
    }
}
