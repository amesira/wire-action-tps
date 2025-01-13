using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimeControl : MonoBehaviour
{
    Animator anim;
    Animation animation;

    public float blinkSpawn;
    public float stateChangeY;

    public AnimationClip loseAnim;
    public AnimationClip winAnim;

    float time;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        animation = GetComponent<Animation>();

        time = blinkSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if(time < 0.0f) {
            anim.SetTrigger("onBlink");
            time = blinkSpawn;
        }
    }
    public void SetRunning(bool _flag) {
        anim.SetBool("isRunning", _flag);
    }

    public void SetScaleVertical(float _yVel) {
        if(_yVel > stateChangeY) {
            anim.SetFloat("scaleVertical", 1.0f);
        }
        else if(_yVel < -stateChangeY) {
            anim.SetFloat("scaleVertical", -1.0f);
        }
        else {
            anim.SetFloat("scaleVertical", 0.0f);
        }
    }

    public void SetSword() {
        anim.SetTrigger("swingSword");
    }

    public IEnumerator PlayEndAnim(int a) {
        anim.enabled = false;
        yield return null;

        if(a == 0) {
            animation.clip = loseAnim;
            yield return null;

            animation.Play();
        }
        else {
            animation.clip = winAnim;
            yield return null;

            animation.Play();
        }
    }
}
