using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioClip[] sounds;
    public SoundArrays[] randSound;

    private AudioSource audioSrc => GetComponent<AudioSource>();

    public bool isPlaying = false;

    private void Start()
    {
        if(!audioSrc) gameObject.AddComponent<AudioSource>();
    }
    public void PlaySound(int i, float volume = 1f, bool random = false, bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f)
    {
        if (!isPlaying)  // Check if the audio source is currently playing a sound
        {
            AudioClip clip = random ? randSound[i].soundArray[Random.Range(0, randSound[i].soundArray.Length)] : sounds[i];
            audioSrc.pitch = Random.Range(p1, p2);

            if (destroyed)
                AudioSource.PlayClipAtPoint(clip, transform.position, volume);
            else
                audioSrc.PlayOneShot(clip, volume);
            isPlaying = true;
            StartCoroutine(ResetIsPlaying(sounds[i].length));
        }
    }

    IEnumerator ResetIsPlaying(float duration)
    {
        yield return new WaitForSeconds(duration);
        isPlaying = false; // —брасываем флаг после окончани€ проигрывани€ звука
    }

    [System.Serializable]
    public class SoundArrays
    {
        public AudioClip[] soundArray;
    }
}
