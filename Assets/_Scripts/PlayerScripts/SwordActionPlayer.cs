using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordActionPlayer : MonoBehaviour
{
    // プレイヤーのアクション状態
    public enum PLAYER_ACTION {
        SWORD_ACTION,
        THROW_ACTION,
    }

    // ================================================
    Rigidbody rb;

    PlayerAnimeControl anim;
    PlayerSoundControl psc;

    public PLAYER_ACTION playerAct;

    [Header("剣アクション")]
    public GameObject traceObject;
    public bool isTrace;
    public float traceSpawn = 0.1f;

    [Header("投げつけアクション")]
    public Transform throwBox;
    public GameObject throwObject;
    public float throwPower = 100.0f;

    [Header("ヒットストップ")]
    [SerializeField] private HitStop hitStop;
    [SerializeField] private float hitStopDuration = 0.1f;
    [SerializeField] private GameObject hitStopEffect;

    float traceTime;
    List<GameObject> traceLine = new List<GameObject>();

    bool inputSwordAction = false;

    bool inputReadyThrowAction = false;
    bool inputThrowAction = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        anim = GetComponent<PlayerAnimeControl>();
        psc = GetComponent<PlayerSoundControl>();

        playerAct = PLAYER_ACTION.SWORD_ACTION;

        traceObject.SetActive(false);
    }

    public void CheckInputActionButton() {
        if(Input.GetMouseButtonDown(0)) {
            inputSwordAction = true;
        }
        if (Input.GetMouseButton(0)) {
            inputReadyThrowAction = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            inputThrowAction = true;
        }
    }

    public void PlayerActionUpdate(Camera _playerCam) 
    {
        switch(playerAct) {
            case PLAYER_ACTION.SWORD_ACTION:
                {
                    if(inputSwordAction) {
                        SwingSword();
                    }

                    /* 剣の軌跡 */
                    if(isTrace && playerAct == PLAYER_ACTION.SWORD_ACTION) {
                        traceTime -= Time.deltaTime;
                        if(traceTime < 0.0f) {
                            traceTime = traceSpawn;
                            /* 軌跡を生成 */
                            GameObject traceCopy = Instantiate(traceObject);

                            /* 軌跡の設定 */
                            traceCopy.SetActive(true);
                            traceCopy.transform.position = traceObject.transform.position;
                            traceCopy.transform.rotation = traceObject.transform.rotation;
                            traceCopy.transform.localScale = traceObject.transform.lossyScale;

                            /* 数秒後に軌跡を削除 */
                            Destroy(traceCopy, 0.15f);
                        }
                    }

                    break;
                }
            case PLAYER_ACTION.THROW_ACTION:
                {
                    // 投げる
                    if(inputThrowAction) {
                        ThrowAwayObject(_playerCam);

                        Time.timeScale = 1.0f;
                        VFXService.instance.ResetPostEffect();
                        VFXService.instance.PlayPostEffect(CustomPostEffect.EffectType.MonoMask, 0.0f, 0.1f, 0.1f, 1);
                        VFXService.instance.ResetFOV(0.1f);
                    }
                    // 投げる準備
                    else if(inputReadyThrowAction)
                    {
                        Time.timeScale = 0.6f;
                        VFXService.instance.PlayPostEffect(CustomPostEffect.EffectType.MonoMask, 1.0f, 0.1f, 0.1f, 1);
                        VFXService.instance.ChangeFOVTemporarily(60.0f, 0.1f, 1.0f);
                    }
                    
                    break;
                }
            default:
                break;
        }

        // 入力リセット
        inputSwordAction = false;
        inputReadyThrowAction = false;
        inputThrowAction = false;
    }

    public void SwingSword() 
    {

        // 攻撃対象検出
        GameObject hitObj = null;
        Collider[] hitColliders = Physics.OverlapBox(
            transform.position + transform.forward * 1.0f, 
            new Vector3(1.0f, 1.0f, 1.0f), 
            transform.rotation);
        foreach (Collider hitCollider in hitColliders) {
            if (hitCollider.gameObject.tag != "ThrowObject") continue;
            hitObj = hitCollider.gameObject;
            break;
        }

        // 少し前へ
        transform.position += transform.forward * 2.0f;

        // 攻撃対象がいる場合
        if (hitObj != null) {
            FixedTurretAct turret = hitObj.GetComponent<FixedTurretAct>();
            if (turret != null) {
                turret.TakeDamage();
                turret.enabled = false;
            }

            // ヒットストップ開始
            hitStop.StartHitStop(
                0.3f, 
                1.0f,
                onEnter: () => {
                    // ヒットストップ開始時の処理
                    anim.SkipAndStopSwordAnim(0.5f);
                    anim.StopAnim();

                    GameObject effect = Instantiate(hitStopEffect, hitObj.transform.position, Quaternion.identity);
                    Destroy(effect, 3.0f);

                    VFXService.instance.PlayPostEffect(CustomPostEffect.EffectType.MonoMask, 0.7f, 0.1f, 0.2f, 1);
                    VFXService.instance.ChangeFOVTemporarily(70.0f, 0.1f, 0.2f);
                    VFXService.instance.StartCameraShake(0.5f, 3.0f);
                },
                onEntered: () => {
                    // ヒットストップ中の処理（必要に応じて）
                },
                onUpdate: () => {
                    // ヒットストップ中の毎フレームの処理（必要に応じて）
                    rb.velocity = Vector3.zero; // プレイヤーの動きを完全に止める
                },
                onExit: () => {
                    // ヒットストップ終了時の処理
                    anim.StartAnim();
                    anim.ResumeSwordLayer();
                    VFXService.instance.PlayPostEffect(CustomPostEffect.EffectType.RadialBlur, 0f, 0.5f, 0.1f, 1);
                    VFXService.instance.ResetFOV(0.2f);

                    // 投げつけるオブジェクトを設定
                    throwObject = hitObj;
                    throwObject.transform.parent = throwBox;
                    throwObject.transform.localPosition = Vector3.zero;
                    SetAction(PLAYER_ACTION.THROW_ACTION);
                }
            );
        }
        // 攻撃対象がいない場合
        else
        {
            // 剣を振るだけ
            anim.SetSword();
        }

        

        // 効果音
        psc.PlaySwingSE(0);
    }

    void ThrowAwayObject(Camera _cam) {
        /* 効果音 */
        psc.PlayThrowClip();

        /* 投げつけベクトルを計算 */
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Vector3 force = ray.direction.normalized * throwPower;

        /* 投げつける */
        throwObject.tag = "PlayerAttack";
        throwObject.layer = 11;
        throwObject.transform.parent = null;

        throwObject.transform.position = transform.position + transform.up * 0.5f;

        throwObject.GetComponent<BoxCollider>().enabled = true;
        throwObject.GetComponent<BoxCollider>().size *= 2.0f;
        throwObject.GetComponent<Rigidbody>().isKinematic = false;
        throwObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        throwObject.GetComponent<AfterImage>().isAfterImage = true;

        Destroy(throwObject, 10.0f);

        SetAction(PLAYER_ACTION.SWORD_ACTION);
    }

    // アクション状態の設定
    void SetAction(PLAYER_ACTION _setAct) {
        playerAct = _setAct;
    }
}
