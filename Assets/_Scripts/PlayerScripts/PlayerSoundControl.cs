using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundControl : MonoBehaviour
{
    AudioSource audioSource;

    public AudioClip runClip;
    public AudioClip jumpClip;
    public AudioClip wireInjectionClip;
    public AudioClip wireShootClip;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayWireInjection() {
        audioSource.PlayOneShot(wireInjectionClip);
    }

    public void PlayWireShoot() {
        audioSource.PlayOneShot(wireShootClip);
    }

    public void PlayRunClip() {
        audioSource.PlayOneShot(runClip);
    }

    public void PlayJumpClip() {
        audioSource.PlayOneShot(jumpClip);
    }
}
