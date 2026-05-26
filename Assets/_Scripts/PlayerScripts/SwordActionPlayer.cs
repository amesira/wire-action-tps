using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class SwordActionPlayer : MonoBehaviour
{
    public enum PLAYER_ACTION {
        SWORD_ACTION,
        THROW_ACTION,
    }

    PlayerAnimeControl anim;
    PlayerSoundControl psc;

    public PLAYER_ACTION playerAct;

    [Header("剣アクション")]
    public BoxCollider swordCol;
    public GameObject traceObject;
    public bool isTrace;
    public float traceSpawn = 0.1f;

    [Header("投げつけアクション")]
    public Transform throwBox;
    public GameObject throwObject;
    public float throwPower = 100.0f;

    float traceTime;
    float swingTime;
    List<GameObject> traceLine = new List<GameObject>();

    bool inputAction = false;

    void Start()
    {
        anim = GetComponent<PlayerAnimeControl>();
        psc = GetComponent<PlayerSoundControl>();

        playerAct = PLAYER_ACTION.SWORD_ACTION;

        traceObject.SetActive(false);
        swordCol.enabled = false;
    }

    public void CheckInputActionButton() {
        if(Input.GetMouseButtonDown(0)) {
            inputAction = true;
        }
    }

    public void PlayerActionUpdate(Camera _playerCam) {
        /* アクション入力 */
        if(inputAction) {
            inputAction = false;

            switch(playerAct) {
                case PLAYER_ACTION.SWORD_ACTION:
                    anim.SetSword();
                    break;
                case PLAYER_ACTION.THROW_ACTION:
                    ThrowAwayObject(_playerCam);
                    break;
                default:
                    break;
            }
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

        /* 剣を振った後処理 */
        swingTime -= Time.deltaTime;
        if(swingTime < 0.0f && swordCol.enabled) {
            swordCol.enabled = false;
        }
    }

    public void SwingSword() {
        psc.PlaySwingSE(0);
        swordCol.enabled = true;
        swingTime = 0.5f;
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

	private void OnTriggerEnter(Collider other) {
        if(playerAct == PLAYER_ACTION.SWORD_ACTION && swordCol.enabled) {
            if(other.tag == "ThrowObject") {
                psc.PlaySwingSE(1);

                throwObject = other.gameObject;

                /* オブジェクトを手に持つ */
                throwObject.transform.parent = throwBox;
                throwObject.transform.position = throwBox.position;

                /* コンポーネント設定 */
                throwObject.GetComponent<SphereCollider>().enabled = false;
                throwObject.GetComponent<BoxCollider>().enabled = false;
                throwObject.GetComponent<Rigidbody>().isKinematic = true;
                Destroy(throwObject.GetComponent<FixedTurretAct>());

                SetAction(PLAYER_ACTION.THROW_ACTION);
            }
        }
	}

    void SetAction(PLAYER_ACTION _setAct) {
        playerAct = _setAct;

        switch(_setAct) {
            case PLAYER_ACTION.SWORD_ACTION:
                swordCol.gameObject.SetActive(true);
                break;
            case PLAYER_ACTION.THROW_ACTION:
                swordCol.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}
