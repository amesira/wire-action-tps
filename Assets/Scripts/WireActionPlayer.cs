using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WireActionPlayer : MonoBehaviour
{
    public RoapControl_PBD roap;

    [Header("ワイヤーの部位")]
    public Transform gunPoint;
    public Transform anchorPoint;

    [Header("ワイヤーのパラメータ")]
    public float anchorSpeed;
    public Transform target;

    [Header("ワイヤーアクションのパラメータ")]
    public float moveDecay;
    public bool isLink;

    bool inputWireButton = false;

    Vector3 startPos;
    Vector3 targetWirePointPos;

    float lerpTime = 0.0f;

    //===================================================
    // ロープ終端と動きをリンクさせる
    //===================================================
    public Vector3 LinkRoap(Vector3 _vel) {
        /* 速度の減衰 */
        _vel.x *= moveDecay;
        _vel.z *= moveDecay;

        /* プレイヤーの力をロープに加える */
        roap.AddForceToPoint(_vel);

        /* ロープ終端地点へ動くための変数を返す */
        Vector3 forward = roap.GetEndPos() - gunPoint.position;
        return Vector3.Magnitude(roap.GetEndPointVel()) * forward;
    }

    //===================================================
    // ワイヤーアクションのキー入力チェック
    //===================================================
    public void CheckInputWireButton() {
        /* マウスボタン入力を確認 */
        if(Input.GetMouseButtonDown(1)) {
            inputWireButton = true;
            

            /* ロープパラメータの変更 */
            roap.SetRopeTypeDynamic();
            roap.isEndFixed = true;

            /* パラメータの初期化 */
            startPos = gunPoint.position;
            targetWirePointPos = target.position;
            lerpTime = 0.0f;

            /* アンカーポイントを独立させる */
            anchorPoint.parent = null;
        }
        else if(Input.GetMouseButtonUp(1)) {
            //inputWireButton = false;
            //isLink = false;
        }
    }

    //===================================================
    // ワイヤー
    //===================================================
    public void WireControl() {
        if(inputWireButton) {
            if(lerpTime < 1.0f) {
                lerpTime += Time.deltaTime * anchorSpeed;

                /* 徐々に減速するようにアンカーを飛ばす */
                float easedLerp = 1.0f - (1.0f - lerpTime) * (1.0f - lerpTime);
                Vector3 pos = Vector3.Lerp(startPos, targetWirePointPos, easedLerp);
                anchorPoint.position = pos;
            }
            else {
                isLink = true;
                roap.SetRopeTypeStatic();
                roap.isEndFixed = false;
            }
        }
    }
}
