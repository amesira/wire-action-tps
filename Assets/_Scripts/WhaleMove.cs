using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WhaleMove : MonoBehaviour
{
    public float speed;
    public float rotStep;

    public int resolution;
    public List<Transform> routePoints;

    public Transform[] spines;

    List<Vector3> pathPoints;
    float pathDistance;

    int routeIndex = 0;

    void Start()
    {
        /* 移動経路の座標を保存 */
        List<Vector3> routePos = new List<Vector3>();
        for(int i = 0; i < routePoints.Count; i++) {
            routePos.Add(routePoints[i].position);
        }
        routePos.Add(routePoints[0].position);

        /* 曲線上のパスを計算 */
        pathPoints = GenerateCatmullRomPath(routePos, resolution);

        /* パス同士の平均距離 */
        float totalDistance = 0;
        for(int i = 0; i < pathPoints.Count - 1; i++) {
            totalDistance += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }
        pathDistance = totalDistance / (pathPoints.Count - 1);

        /* 初期位置を設定 */
        transform.position = pathPoints[0];

        /* デバッグ用：経路をCubeで表示 */
        //for(int i = 0; i < pathPoints.Count; i++) {
        //    GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    o.transform.position = pathPoints[i];
        //}
    }

    void FixedUpdate()
    {
        if(pathPoints != null) {
            /* 移動経路の番号を0に戻す */
            if(routeIndex < 0 || routeIndex >= pathPoints.Count) {
                routeIndex = 0;
            }

            /* 経路点に向かって移動 */
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[routeIndex], speed * Time.deltaTime);

            /* 移動方向に身体を回転 */
            //Quaternion lookRot = Quaternion.LookRotation(pathPoints[routeIndex] - transform.position);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotStep);

            int targetIndex = routeIndex;
            for(int i = 0; i < spines.Length - 1; i++) {
                /* 骨の距離からインデックスを算出 */
                float spineDistance = Vector3.Distance(spines[i].position, spines[i + 1].position);
                targetIndex -= Mathf.FloorToInt(spineDistance / pathDistance) + 3;
                if(targetIndex < 0) {
                    targetIndex += pathPoints.Count;
                }

                Quaternion spineLookRot = Quaternion.LookRotation(pathPoints[targetIndex] - spines[i].position);
                Quaternion adjustment = Quaternion.Euler(90, 0, 0);
                spineLookRot *= adjustment;

                // 親の逆回転を掛けてローカル回転に変換
                Quaternion localLookRot = Quaternion.Inverse(spines[i].parent.rotation) * spineLookRot;

                /* 初期回転はそのまま */
                localLookRot *= Quaternion.Euler(0, 90, 0);

                // ローカル回転を適用
                spines[i].localRotation = Quaternion.RotateTowards(spines[i].localRotation, localLookRot, rotStep);
            }

            /* 移動完了を判定 */
            if(Vector3.Distance(transform.position, pathPoints[routeIndex]) < 0.5f) {
                routeIndex++;
            }
        }
    }

    /* Catmull-Romスプラインの曲線を生成 */
    List<Vector3> GenerateCatmullRomPath(List<Vector3> _routePos, int _resolution) {
        List<Vector3> sampledPoints = new List<Vector3>();

        for(int i = 0; i < _routePos.Count - 1; i++) {
            Vector3 p0 = i > 0 ? _routePos[i - 1] : _routePos[i];     // 前の点
            Vector3 p1 = _routePos[i];                                // 現在の点
            Vector3 p2 = _routePos[i + 1];                            // 次の点
            Vector3 p3 = i + 2 < _routePos.Count ? _routePos[i + 2] : _routePos[i + 1]; // 次の次の点

            // セグメントの分割点を計算
            for(int j = 0; j < _resolution; j++) {
                float t = j / (float)_resolution; // 0～1に分割
                sampledPoints.Add(CalculateCatmullRomPoint(t, p0, p1, p2, p3));
            }
        }

        return sampledPoints;
    }

    /* Catmull-Romスプラインの点を計算 */
    Vector3 CalculateCatmullRomPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}
