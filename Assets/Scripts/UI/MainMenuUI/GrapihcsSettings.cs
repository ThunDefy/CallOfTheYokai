using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GrapihcsSettings : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;
    public Slider musicVolumeSlider, soundVolumeSlider;
    float currentMusicVolume, currentSoundVolume;

    void Start()
    {
        LoadSettings();
    }

    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = volume;
    }
    public void SetSoundVolume(float volume)
    {
        currentSoundVolume = volume;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }


    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference",qualityDropdown.value);
        PlayerPrefs.SetFloat("MusicVolumePreference", currentMusicVolume);
        PlayerPrefs.SetFloat("SoundVolumePreference", currentSoundVolume);

    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        else
            qualityDropdown.value = 3;

        if (PlayerPrefs.HasKey("MusicVolumePreference"))
            musicVolumeSlider.value =PlayerPrefs.GetFloat("MusicVolumePreference");
        else
            musicVolumeSlider.value = 1;

        if (PlayerPrefs.HasKey("SoundVolumePreference"))
            soundVolumeSlider.value = PlayerPrefs.GetFloat("SoundVolumePreference");
        else
            soundVolumeSlider.value = 1;
    }
}
