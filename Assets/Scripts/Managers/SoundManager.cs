using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] AudioSource music;
    [SerializeField] AudioSource sfx;
    float tempMusicVol = -1f;
    Coroutine currCoroutine = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        music.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, bool loop = false)
    {
        sfx.loop = loop;

        if(loop)
        {
            sfx.clip = clip;
            sfx.Play();
        } else
        {
            sfx.PlayOneShot(clip);
        }
        
    }

    public void StopSFX()
    {
        sfx.Stop();
    }

    //Lower music volume for things like stars
    public void LowerMusicVolume(float multiplier, float duration = -1f)
    {
        if(tempMusicVol < 0)
        {
            tempMusicVol = music.volume;
        }
        
        music.volume = tempMusicVol * multiplier;

        if(duration > 0f && currCoroutine == null)
        {
            currCoroutine = StartCoroutine(RestoreMusicVolumeCoroutine(duration));
        } else if(duration < 0f)
        {
            if(currCoroutine != null)
            {
                StopCoroutine(currCoroutine);
            }
        }
    }

    public void RestoreMusicVolume()
    {
        if(tempMusicVol > 0f)
        {
            music.volume = tempMusicVol;
            tempMusicVol = -1f;
        }
    }

    IEnumerator RestoreMusicVolumeCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        RestoreMusicVolume();
        currCoroutine = null;
    }
}
