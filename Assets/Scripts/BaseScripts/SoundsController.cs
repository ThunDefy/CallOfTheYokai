using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsController : MonoBehaviour
{
    public static SoundsController instance; // Делаем класс синглтон

    public float currentSoundVolume;
    public float currentMusicVolume;

    private AudioSource audioSrc => GetComponent<AudioSource>();
    private Sounds sounds => GetComponent<Sounds>();
    public void LoadCurrentVolumes()
    {
        if (PlayerPrefs.HasKey("SoundVolumePreference"))
            currentSoundVolume = PlayerPrefs.GetFloat("SoundVolumePreference");
        else currentSoundVolume = 1f;

        if(PlayerPrefs.HasKey("MusicVolumePreference"))
            currentMusicVolume = PlayerPrefs.GetFloat("MusicVolumePreference");
        else currentMusicVolume = 1f;

        audioSrc.volume = currentMusicVolume;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (!sounds.isPlaying) sounds.PlaySound(0, volume: SoundsController.instance.currentMusicVolume,p1:1f,p2:1f);
    }

    private void Start()
    {
        LoadCurrentVolumes();
        sounds.PlaySound(0, volume: SoundsController.instance.currentMusicVolume, p1: 1f, p2: 1f);
    }
}
