using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Separate class because we want the player sounds to be able to stop etc independently of main game sounds.
/// </summary>
public class PlayerSounds : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource runAudioSource; //Separate audio source since having it start/stop with the others causes problems 
    public AudioClip RunSound;
    public AudioClip JumpSound;
    public AudioClip BounceSound;
    public AudioClip LandSound;
    public AudioClip DiveSound;
    public AudioClip SlingshotJumpSound;
    public AudioClip SlideSound; 
    public AudioClip WallSlideSound;
    public AudioClip WallJumpSound;

    public void PlaySound(AudioClip clip, bool loop = false)
    {
        if(clip == RunSound)
        {
            runAudioSource.clip = clip;
            runAudioSource.loop = loop;

            runAudioSource.Play();
        } else
        {           
            audioSource.loop = loop;

            if(loop)
            {
                audioSource.clip = clip;
                audioSource.Play();
            } else
            {
                audioSource.PlayOneShot(clip);
            }
            
        }
        
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void StopRunSound()
    {
        runAudioSource.Stop();
    }

    public void PauseSound()
    {
        audioSource.Pause();
        runAudioSource.Pause();
    }

    public void UnPauseSound()
    {
        audioSource.UnPause();
        runAudioSource.UnPause();
    }

}
