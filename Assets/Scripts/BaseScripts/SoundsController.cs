using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsController : MonoBehaviour
{
    public static SoundsController instance; // Делаем класс синглтон

    public float currentSoundVolume;
    public float currentMusicVolume;
    public void LoadCurrentVolumes()
    {
        if (PlayerPrefs.HasKey("SoundVolumePreference"))
            currentSoundVolume = PlayerPrefs.GetFloat("SoundVolumePreference");
        else currentSoundVolume = 1f;

        if(PlayerPrefs.HasKey("MusicVolumePreference"))
            currentMusicVolume = PlayerPrefs.GetFloat("MusicVolumePreference");
        else currentMusicVolume = 1f;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        LoadCurrentVolumes();
    }
}
