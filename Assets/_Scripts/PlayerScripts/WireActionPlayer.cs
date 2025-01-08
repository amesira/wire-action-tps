using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WireActionPlayer : MonoBehaviour
{
    public RoapControl_PBD rope;    // ワイヤー用のロープ

    [Header("ワイヤーの部位")]
    public Transform gunPoint;      // 銃口
    public Transform anchorPoint;   // アンカー
    public Transform handPoint;

    [Header("ワイヤーのパラメータ")]
    public float anchorSpeed = 3.0f;    // アンカー射出スピード
    public float rewindSpeed = 50.0f;   // アンカー回収スピード
    public float moveDecay = 0.3f;  // ワイヤーアクション時の速度減衰
    public bool isLink;             // ワイヤーとリンクしているか

    [Header("アンカーターゲット")]
    public string targetTagName;    // アンカーターゲットのタグ
    public Transform anchorTarget;  // アンカーターゲット
    public float viewingAngle = 15.0f;
    public float viewingDistance;
    public RectTransform scopeImage;

    Transform anchorParent;
    GameObject[] targets;           // アンカーターゲットのリスト

    bool inputWireButton = false;

    Vector3 startPos;               // アンカーのスタート地点
    Transform targetWirePoint;     // アンカーの目標地点
    float lerpTime = 0.0f;

    PlayerSoundControl psc;


	private void Start() {
        targets = GameObject.FindGameObjectsWithTag(targetTagName);
        anchorParent = anchorPoint.parent;

        psc = GetComponent<PlayerSoundControl>();
	}

    //===================================================
    // アンカーターゲットを設定
    //===================================================
	public void SetAnchorTarget(Camera _camera) {
        anchorTarget = null;
        List<GameObject> viewTarget = new List<GameObject>();

        /* 視野角をラジアンに変換 */
        float viewingAngleCos = Mathf.Cos(viewingAngle * Mathf.Deg2Rad);

        /* 視界に入っているターゲットを取得 */
        foreach(GameObject o in targets) {
            /* ターゲットからカメラ方向へのベクトル */
            Vector3 targetToCamera_N = (_camera.transform.position - o.transform.position).normalized;

            /* 正規化したベクトルの内積が一定以下なら視界に入っている */
            if(Vector3.Dot(targetToCamera_N, _camera.transform.forward.normalized) < -viewingAngleCos &&
                Vector3.Magnitude(_camera.transform.position - o.transform.position) < viewingDistance) {
                viewTarget.Add(o);
            }
        }

        /* アンカーターゲットを設定 */
        float minDistance = viewingDistance;
        foreach(GameObject o in viewTarget) {
            /* ターゲットとカメラの距離を取得 */
            float distance = Vector3.Magnitude(_camera.transform.position - o.transform.position);

            /* 距離が最も短いポイントをアンカーターゲットに */
            if(distance < minDistance) {
                minDistance = distance;

                /* アンカーターゲットを設定 */
                anchorTarget = o.transform;
            }
        }

        /* アンカーターゲットの位置にスコープを表示 */
        if(anchorTarget != null) {
            scopeImage.gameObject.SetActive(true);

            /* スクリーン座標に変換したのち位置を設定 */
            Vector3 targetWorldPos = anchorTarget.position;
            Vector3 targetScreenPos = _camera.WorldToScreenPoint(targetWorldPos);
            scopeImage.position = targetScreenPos;
        }
        else {
            scopeImage.gameObject.SetActive(false);
        }
    }

    //===================================================
    // ワイヤーアクションのキー入力チェック
    //===================================================
    public void CheckInputWireButton() {
        /* マウスボタン入力を確認 */
        if(Input.GetMouseButtonDown(1)) {
            ShotWire();
        }
        else if(Input.GetMouseButtonUp(1)) {
            DivideWire();
        }
    }

    public void ShotWire() {
        if(anchorTarget != null) {
                inputWireButton = true;

                /* ロープ設定 */
                rope.roapType = RoapControl_PBD.ROPE_TYPE.ROPE_EXTEND;
                rope.isEndFixed = true;

                /* ロープを初期化 */
                rope.InitializeRope();

                /* アンカー射出の初期値を設定 */
                startPos = gunPoint.position;
                targetWirePoint = anchorTarget;
                lerpTime = 0.0f;

                /* アンカーポイントを独立させる */
                anchorPoint.parent = null;

                /* 効果音を鳴らす */
                psc.PlayWireInjection();
            }
    }

    public void DivideWire() {
        inputWireButton = false;

            /* プレイヤーとのリンクを切る */
            isLink = false;

            /* ロープ設定 */
            rope.roapType = RoapControl_PBD.ROPE_TYPE.ROPE_RETRACT;
            rope.isEndFixed = true;

            /* アンカー回収の初期値を設定 */
            startPos = anchorPoint.position;
            targetWirePoint = gunPoint;
            lerpTime = 0.0f;
    }

    //===================================================
    // ロープ終端と動きをリンクさせる
    //===================================================
    public Vector3 LinkRoap(Vector3 _vel) {
        /* 速度の減衰 */
        _vel.x *= moveDecay;
        _vel.z *= moveDecay;

        /* プレイヤーの力をロープに加える */
        rope.AddForceToPoint(_vel);

        /* ロープ終端地点へ動くための変数を返す */
        Vector3 forward = rope.GetEndPos() - gunPoint.position;
        return Vector3.Magnitude(rope.GetEndPointVel()) * forward;
    }

    //===================================================
    // アンカーを制御
    //===================================================
    public void WireAnchorControl() {
        if(inputWireButton) {
            if(lerpTime < 1.0f) {
                lerpTime += Time.deltaTime * anchorSpeed;

                /* 徐々に減速するようにアンカーを飛ばす */
                float easedLerp = 1.0f - (1.0f - lerpTime) * (1.0f - lerpTime);
                Vector3 pos = Vector3.Lerp(startPos, targetWirePoint.position, easedLerp);
                anchorPoint.position = pos;
            }
            else {
                /* アンカーの位置を固定 */
                anchorPoint.position = targetWirePoint.position;

                if(!isLink) {
                    /* プレイヤーとリンクさせる */
                    isLink = true;

                    /* ロープ設定 */
                    rope.roapType = RoapControl_PBD.ROPE_TYPE.ROPE_STATIC;
                    rope.isEndFixed = false;

                    /* 効果音を鳴らす */
                    psc.PlayWireShoot();
                }
            }
        }
        else {
            if(Vector3.Magnitude(gunPoint.position - anchorPoint.position) > 0.3f) {
                /* アンカーを回収 */
                Vector3 foward = gunPoint.position - anchorPoint.position;
                Vector3 pos = foward * Time.deltaTime * rewindSpeed;
                anchorPoint.position += pos;
            }
            else {
                /* アンカーポイントをプレイヤーの子オブジェクトに戻す */
                anchorPoint.position = gunPoint.position;
                anchorPoint.parent = anchorParent;
            }
        }

        handPoint.position = rope.GetEndPos();
    }
}
