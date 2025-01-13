using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundControl : MonoBehaviour
{
    AudioSource audioSource;

    public AudioClip runClip;
    public AudioClip jumpClip;

    public AudioClip swingClip;
    public AudioClip chopSwordClip;

    public AudioClip wireInjectionClip;
    public AudioClip wireShootClip;

    public AudioClip throwClip;

    public AudioClip damageClip;

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

    public void PlayThrowClip() {
        audioSource.PlayOneShot(throwClip);
    }

    public void PlaySwingSE(int n) {
        if(n == 0) {
            audioSource.PlayOneShot(swingClip);
        }
        else if(n == 1) {
            audioSource.PlayOneShot(chopSwordClip);
        }
    }

    public void PlayDamageSE() {
        audioSource.PlayOneShot(damageClip);
    }

}
