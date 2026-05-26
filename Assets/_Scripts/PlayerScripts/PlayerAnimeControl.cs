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
        // まばたきアニメーションの再生
        time -= Time.deltaTime;
        if(time < 0.0f) {
            anim.SetTrigger("onBlink");
            time = blinkSpawn;
        }
    }

    // 走るアニメーションの切り替え
    public void SetRunning(bool _flag) {
        anim.SetBool("isRunning", _flag);
    }

    // y軸の速度に応じてアニメーションを切り替える
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

    // 剣アクションのトリガー
    public void SetSword() {
        anim.SetTrigger("swingSword");
        Debug.Log("swing");
    }

    // 剣アクションのアニメーションをスキップして、指定した位置で停止する
    public void SkipAndStopSwordAnim(float normalizedTime) {
        anim.Play("PlayerSword_Swing", 2, normalizedTime);
        anim.Update(0.0f);
    }
    // 剣アクションのレイヤーを再開する
    public void ResumeSwordLayer() {
        anim.SetLayerWeight(2, 1.0f);
    }

    // ゲーム終了時のアニメーション再生
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

    // アニメーションの再生を停止する
    public void StopAnim() {
        anim.enabled = false;
    }

    // アニメーションの再生を再開する
    public void StartAnim() {
        anim.enabled = true;
    }
}
