using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using Utils;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioClip LaunchSound;
    public AudioClip PopSound;
    public AudioClip FallSound;
    
    private static AudioSource _audioSource;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayLaunch()
    {
        _audioSource.PlayOneShot(LaunchSound);
    }

    public void PlayPop()
    {
        _audioSource.PlayOneShot(PopSound);
    }

    public void PlayFall()
    {
        _audioSource.PlayOneShot(FallSound);
    }
}
