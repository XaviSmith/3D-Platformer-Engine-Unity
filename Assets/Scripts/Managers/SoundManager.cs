using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public SoundManager Instance;

    [SerializeField] AudioSource music;
    [SerializeField] AudioSource sfx;

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
}
