using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireActionPlayer : MonoBehaviour
{
    public RoapControl_PBD roap;

    [Header("ワイヤーのパラメータ")]
    public float moveDecay;

    public Vector3 LinkRoap(Vector3 _vel) {
        /* 速度の減衰 */
        _vel.x *= moveDecay;
        _vel.z *= moveDecay;

        /* プレイヤーの力をロープに加える */
        roap.AddForceToPoint(_vel);

        /* ロープ終端地点へ動くための変数を返す */
        Vector3 forward = roap.GetEndPos() - this.transform.position;
        return Vector3.Magnitude(roap.GetEndPointVel()) * forward;
    }
}
