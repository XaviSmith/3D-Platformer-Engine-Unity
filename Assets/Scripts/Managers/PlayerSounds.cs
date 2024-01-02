using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Separate class because we want the player sounds to be able to stop etc independently of main game sounds.
/// </summary>
public class PlayerSounds : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
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
        audioSource.clip = clip;
        audioSource.loop = loop;

        audioSource.Play();
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

}
