//===================================================
// RoapControl_PBD.cs
// 
// 作成者：北村美羽  作成日：2024/12/20
//===================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

/// <summary>
/// 3x3行列クラス
/// </summary>
public class Matrix3x3 {
    public float m00, m01, m02;
    public float m10, m11, m12;
    public float m20, m21, m22;

    public Matrix3x3 MultiplyMatrix(Matrix3x3 _mat) {   // 行列×行列
        Matrix3x3 matrix = new Matrix3x3();

        matrix.m00 = this.m00 * _mat.m00 + this.m01 * _mat.m10 + this.m02 * _mat.m20;
        matrix.m01 = this.m00 * _mat.m01 + this.m01 * _mat.m11 + this.m02 * _mat.m21;
        matrix.m02 = this.m00 * _mat.m02 + this.m01 * _mat.m12 + this.m02 * _mat.m22;

        matrix.m10 = this.m10 * _mat.m00 + this.m11 * _mat.m10 + this.m12 * _mat.m20;
        matrix.m11 = this.m10 * _mat.m01 + this.m11 * _mat.m11 + this.m12 * _mat.m21;
        matrix.m12 = this.m10 * _mat.m02 + this.m11 * _mat.m12 + this.m12 * _mat.m22;

        matrix.m20 = this.m20 * _mat.m00 + this.m21 * _mat.m10 + this.m22 * _mat.m20;
        matrix.m21 = this.m20 * _mat.m01 + this.m21 * _mat.m11 + this.m22 * _mat.m21;
        matrix.m22 = this.m20 * _mat.m02 + this.m21 * _mat.m12 + this.m22 * _mat.m22;

        return matrix;
    }

    public Matrix3x3 MultiplyFloat(float _f) {      // 行列×小数型
        Matrix3x3 matrix = new Matrix3x3();

        matrix.m00 = this.m00 * _f;
        matrix.m01 = this.m01 * _f;
        matrix.m02 = this.m02 * _f;

        matrix.m10 = this.m10 * _f;
        matrix.m11 = this.m11 * _f;
        matrix.m12 = this.m12 * _f;

        matrix.m20 = this.m20 * _f;
        matrix.m21 = this.m21 * _f;
        matrix.m22 = this.m22 * _f;

        return matrix;
    }

    public Vector3 MultiplyVector3(Vector3 _vec) {
        Vector3 vector = Vector3.zero;

        vector.x = this.m00 * _vec.x + this.m01 * _vec.y + this.m02 * _vec.z;
        vector.y = this.m10 * _vec.x + this.m11 * _vec.y + this.m12 * _vec.z;
        vector.z = this.m20 * _vec.x + this.m21 * _vec.y + this.m22 * _vec.z;

        return vector;
    }

    public Matrix3x3 Add(Matrix3x3 _mat) {
        Matrix3x3 matrix = new Matrix3x3();

        matrix.m00 = this.m00 + _mat.m00;
        matrix.m01 = this.m01 + _mat.m01;
        matrix.m02 = this.m02 + _mat.m02;

        matrix.m10 = this.m10 + _mat.m10;
        matrix.m11 = this.m11 + _mat.m11;
        matrix.m12 = this.m12 + _mat.m12;

        matrix.m20 = this.m20 + _mat.m20;
        matrix.m21 = this.m21 + _mat.m21;
        matrix.m22 = this.m22 + _mat.m22;

        return matrix;
    }

    public Matrix3x3 Transpose() {  // 転置行列
        Matrix3x3 matrix = new Matrix3x3();

        matrix.m00 = this.m00;
        matrix.m10 = this.m01;
        matrix.m20 = this.m02;

        matrix.m01 = this.m10;
        matrix.m11 = this.m11;
        matrix.m21 = this.m12;

        matrix.m02 = this.m20;
        matrix.m12 = this.m21;
        matrix.m22 = this.m22;

        return matrix;
    }

    public Matrix3x3 Inverse() {    // 逆行列
        Matrix3x3 matrix = new Matrix3x3();

        matrix.m00 = this.m11 * this.m22 - this.m12 * this.m21;
        matrix.m01 = -(this.m01 * this.m22 - this.m02 * this.m21);
        matrix.m02 = this.m01 * this.m12 - this.m02 * this.m11;

        matrix.m10 = -(this.m10 * this.m22 - this.m12 * this.m20);
        matrix.m11 = this.m00 * this.m22 - this.m02 * this.m20;
        matrix.m12 = -(this.m00 * this.m12 - this.m02 * this.m10);

        matrix.m20 = this.m10 * this.m21 - this.m11 * this.m20;
        matrix.m21 = -(this.m00 * this.m21 - this.m01 * this.m20);
        matrix.m22 = this.m00 * this.m11 - this.m01 * this.m10;

        return matrix;
    }
}

public class RoapControl_PBD : MonoBehaviour
{
    public enum ROPE_TYPE {
        ROPE_STATIC,    // 静的なロープ（質点の数は動かない）
        ROPE_EXTEND,    // 伸びるロープ
        ROPE_RETRACT,   // 縮むロープ
    }
    struct MassPoint {         // 質点構造体
        public Vector3 pos;    // 位置
        public Vector3 vel;    // 速度
        public Vector3 estPos; // 推定位置

        public float mass;     // 質量
        public bool isFixed;   // 固定
    }
    public class Constraint {           // 拘束クラス
        public int massPoint1;          // 質点A
        public int massPoint2;          // 質点B
        public float defStretch;        // 通常状態の伸び

        public Constraint(int _massPoint1, int _massPoint2, float _defStretch) {
            massPoint1 = _massPoint1;
            massPoint2 = _massPoint2;
            defStretch = _defStretch;
        }
    }

    const int MIN_MASS_POINT = 2;

    public ROPE_TYPE roapType;

    [Header("ロープの両端の位置")]
    public Transform startPoint;    // ロープの開始点
    public Transform endPoint;      // ロープに終点
    public bool isEndFixed;

    [Header("ロープの計算パラメータ")]
    public float pointSpawn = 1.5f; // 質点の配置間隔
    public float spawnRange = 0.5f; // 伸びの範囲
    public float pointMass = 1.0f;  // 質点の質量

    [Space]
    public float stiffness = 1.0f;  // ばねの強さ
    public float gravity = -9.8f;   // ロープにかかる重力
    public float kDamping = 0.03f;
    public float maxPointVel = 50.0f;

    [Space]
    public float endPointSpwn = 0.1f;
    public float endPointGetPower = 5.0f;

    [Header("ロープの描画パラメータ")]
    public float lineWidth;
    public Color lineColor;

    int pointNum;

    List<MassPoint> massPoints = null;  // 質点
    List<Constraint> constraints;       // 質点間の拘束

    Vector3 moveForce;                  // ロープの終端に加わる力

    LineRenderer roapLine;

    List<GameObject> pointObj;          // 質点に表示するオブジェクト（デバッグ用）

	private void Awake() {
        /* 質点の個数を設定 */
        float roapLen = Vector3.Magnitude(startPoint.position - endPoint.position);
        pointNum = Mathf.FloorToInt(roapLen / pointSpawn);
        if(pointNum < MIN_MASS_POINT) {
            pointNum = MIN_MASS_POINT;
        }

        /* 質点の初期化 */
        massPoints = new List<MassPoint>();
        for(int i = 0; i < pointNum; i++) {
            massPoints.Add(new MassPoint());
        }

        for(int i = 0; i < pointNum; i++) {
            /* ロープの始点からの相対的な位置割合 */
            float relaPos = (float)i / (pointNum - 1);

            /* 質点のパラメータを初期化 */
            MassPoint tmp = massPoints[i];
            tmp.pos = Vector3.Lerp(startPoint.position, endPoint.position, relaPos);
            tmp.vel = Vector3.zero;
            tmp.mass = pointMass;
            massPoints[i] = tmp;
        }

        /* Constraintの初期化 */
        constraints = new List<Constraint>(); // 質点間の拘束を表す
        for(int i = 0; i < pointNum - 1; i++) {
            /* 通常の伸びを計算（Vector3.Magnitude：ベクトルの長さを表す） */
            float defStretch = pointSpawn;

            /* 隣り合う質点を接続 */
            constraints.Add(new Constraint(i, i + 1, defStretch));
        }

        /* isFixedの初期化 */
        for(int i = 0; i < pointNum; i++) {
            MassPoint init = massPoints[i];
            if((i > 0 && i < pointNum - 1) || (i == pointNum - 1 && !isEndFixed)) {
                init.isFixed = false;
            }
            else {
                init.isFixed = true;
            }
            massPoints[i] = init;
        }

        /* moveForceの初期化 */
        moveForce = Vector3.zero;

        /* LineRendererの初期化 */
        roapLine = GetComponent<LineRenderer>();
        roapLine.positionCount = pointNum;
        roapLine.startWidth = roapLine.endWidth = lineWidth;
        roapLine.startColor = roapLine.endColor = lineColor;

        /* MassPointに設置するSphereを生成（デバッグ用） */
        pointObj = new List<GameObject>();
        for(int i = 0; i < pointNum; i++) {
            pointObj.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
            pointObj[i].name = "MassPointObject" + (i + 1);

            pointObj[i].transform.localScale = new Vector3(0.05f, 0.3f, 0.05f);
            Destroy(pointObj[i].GetComponent<SphereCollider>());
        }
	}

    void FixedUpdate() {
        /* 最終点のisFixedを更新 */
        if(massPoints[pointNum - 1].isFixed != isEndFixed) {
            MassPoint tmp = massPoints[pointNum - 1];
            tmp.isFixed = isEndFixed;
            massPoints[pointNum - 1] = tmp;
        }

        /* 動的なロープ処理 */
        switch(roapType) {
            case ROPE_TYPE.ROPE_EXTEND: // 質点を追加
                if(Vector3.Magnitude(massPoints[pointNum - 1].pos - massPoints[pointNum - 2].pos) > pointSpawn + spawnRange) {
                    AddMassPoint();
                }
                break;
            case ROPE_TYPE.ROPE_RETRACT: // 質点を削除
                if(Vector3.Magnitude(massPoints[pointNum - 1].pos - massPoints[pointNum - 2].pos) < pointSpawn + spawnRange && pointNum > 2) {
                    RemoveMassPoint();
                }
                break;
        }

		/* 外力による速度変化 */
        for(int i = 0; i < pointNum; i++) {
            MassPoint tmp = massPoints[i];
            tmp.vel += new Vector3(0.0f, gravity, 0.0f) * Time.deltaTime;

            if(massPoints[i].isFixed) {
                tmp.vel = Vector3.zero;
            }
            massPoints[i] = tmp;
        }

        /* オブジェクトによる速度変化 */
        if(!massPoints[pointNum - 1].isFixed) {
            MassPoint tmp = massPoints[pointNum - 1];

            /* moveForceによる最終質点の移動 */
            tmp.vel += moveForce;
            moveForce = Vector3.zero;

            /* endPointとの差が一定距離以上開いた場合 */
            Vector3 hangDiff = endPoint.position - massPoints[pointNum - 1].pos;
            if(Vector3.Magnitude(hangDiff) > endPointSpwn) {
                tmp.vel += hangDiff * Time.deltaTime * endPointGetPower;
            }

            massPoints[pointNum - 1] = tmp;
        }

        /* VelocityDamping */
        VelocityDamping();

        /* 位置の更新 */
        for(int i = 0; i < pointNum; i++) {
            MassPoint tmp = massPoints[i];
            /* 現在位置と速度から、推定位置を算出 */
            tmp.estPos = massPoints[i].pos + massPoints[i].vel * Time.deltaTime;

            /* 位置を更新 */
            tmp.pos = tmp.estPos;
            massPoints[i] = tmp;
        }

        /* 開始点と終了点の位置設定 */
        if(massPoints[0].isFixed) {
            MassPoint tmp = massPoints[0];
            tmp.pos = startPoint.position;
            massPoints[0] = tmp;
        }
        if(massPoints[pointNum - 1].isFixed) {
            MassPoint tmp = massPoints[pointNum - 1];
            tmp.pos = endPoint.position;
            massPoints[pointNum - 1] = tmp;
        }

        /* 速度の更新 */
        for(int i = 0; i < pointNum - 1; i++) {
            /* 使用する変数を一時変数に保持 */
            Constraint c = constraints[i];
            Vector3 e1 = massPoints[c.massPoint1].estPos;
            Vector3 e2 = massPoints[c.massPoint2].estPos;

            /* 質量の逆数 */
            float w1 = 1f / massPoints[c.massPoint1].mass;
            float w2 = 1f / massPoints[c.massPoint2].mass;

            /* 推定位置から伸びを計算 */
            float diff = Vector3.Magnitude(e1 - e2);

            /* ロープの弾性力を計算 */
            Vector3 de1 = -stiffness * w1 / (w1 + w2) * (diff - c.defStretch) * Vector3.Normalize(e1 - e2);
            Vector3 de2 = stiffness * w2 / (w1 + w2) * (diff - c.defStretch) * Vector3.Normalize(e1 - e2);

            /* 速度を更新 */
            MassPoint tmp1 = massPoints[c.massPoint1];
            MassPoint tmp2 = massPoints[c.massPoint2];

            tmp1.vel += de1 / Time.deltaTime;
            tmp2.vel += de2 / Time.deltaTime;

            /* 最大速度を制限 */
            tmp1.vel = Vector3.ClampMagnitude(tmp1.vel, maxPointVel);
            tmp2.vel = Vector3.ClampMagnitude(tmp2.vel, maxPointVel);

            massPoints[c.massPoint1] = tmp1;
            massPoints[c.massPoint2] = tmp2;
        }

        /* LineRendererへ位置を反映 */
        for(int i = 0; i < pointNum; i++) {
            roapLine.SetPosition(i, massPoints[i].pos);
        }

        /* MassPointにSphereを表示（デバッグ用） */
        for(int i = 0; i < pointNum; i++) {
            pointObj[i].transform.position = massPoints[i].pos;
        }
    }

    //===================================================
    // 速度減衰関数
    //===================================================
    private void VelocityDamping() {
        Vector3 cmPos = Vector3.zero;   // 質量中心
        Vector3 cmVel = Vector3.zero;   // 質量中心の速度
        float totalMass = 0.0f;         // 質量

        /* 質量中心の位置、速度を算出 */
        for (int i = 0; i < pointNum; i++) {
            cmPos += massPoints[i].pos;
            cmVel += massPoints[i].vel;
            totalMass += massPoints[i].mass;
        }
        cmPos /= totalMass;
        cmVel /= totalMass;

        Vector3 L = Vector3.zero;           // 角運動量（回転する力の量）
        Matrix3x3 I = new Matrix3x3();      // 慣性テンソル（回転しやすさ）
        Vector3[] rs = new Vector3[pointNum];   // 角速度（実際の回転速度）

        for (int i = 0; i < pointNum; i++) {
            Vector3 r = massPoints[i].pos - cmPos;
            rs[i] = r;

            /* スキュー対称行列を作成 */
            Matrix3x3 R = new Matrix3x3();
            R.m01 = r[2];
            R.m02 = -r[1];
            R.m10 = r[2];
            R.m12 = -r[0];
            R.m20 = -r[1];
            R.m21 = r[0];

            /* 剛体の回転運動の計算 */
            L += Vector3.Cross(r, massPoints[i].mass * massPoints[i].vel);
            I.Add(R.MultiplyMatrix(R.Transpose()).MultiplyFloat(massPoints[i].mass));
        }

        Vector3 omega = I.Inverse().MultiplyVector3(L);

        for (int i = 0; i < pointNum; i++) {
            MassPoint tmp = massPoints[i];
            Vector3 deltaV = cmVel + Vector3.Cross(omega, rs[i]) - massPoints[i].vel;
            tmp.vel += kDamping * deltaV;
            massPoints[i] = tmp;
        }
    }

    //===================================================
    // 質点追加関数
    //===================================================
    private void AddMassPoint() {
        MassPoint addPoint = new MassPoint();

        /* 追加する質点の初期化 */
        addPoint.pos = endPoint.position;
        addPoint.vel = Vector3.zero;
        addPoint.mass = pointMass;

        /* 新しい質点をリストに追加 */
        massPoints.Add(addPoint);
        pointNum++;

        /* 拘束クラスを追加 */
        Constraint addConstraint = new Constraint(pointNum - 2, pointNum - 1, pointSpawn);
        constraints.Add(addConstraint);

        /* isFixedの初期化 */
        for(int i = 0; i < pointNum; i++) {
            MassPoint init = massPoints[i];
            if((i > 0 && i < pointNum - 1) || (i == pointNum - 1 && !isEndFixed)) {
                init.isFixed = false;
            }
            else {
                init.isFixed = true;
            }
            massPoints[i] = init;
        }

        /* LineRendererの更新 */
        roapLine.positionCount = pointNum;

        /* デバッグ用Sphereの追加 */
        pointObj.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
        pointObj[pointNum - 1].name = "MassPointObject" + (pointNum - 1 + 1);

        pointObj[pointNum - 1].transform.localScale = new Vector3(0.05f, 0.3f, 0.05f);
        Destroy(pointObj[pointNum - 1].GetComponent<SphereCollider>());
    }

    //===================================================
    // 質点削除関数
    //===================================================
    private void RemoveMassPoint() {
        massPoints.RemoveAt(pointNum - 1);
        constraints.RemoveAt(pointNum - 2);

        /* デバッグ用Sphereの削除 */
        Destroy(pointObj[pointNum - 1]);
        pointObj.RemoveAt(pointNum - 1);

        pointNum--;

        /* isFixedの初期化 */
        for(int i = 0; i < pointNum; i++) {
            MassPoint init = massPoints[i];
            if((i > 0 && i < pointNum - 1) || (i == pointNum - 1 && !isEndFixed)) {
                init.isFixed = false;
            }
            else {
                init.isFixed = true;
            }
            massPoints[i] = init;
        }

        /* LineRendererの更新 */
        roapLine.positionCount = pointNum;
    }

    //===================================================
    // 終端質点の位置を取得
    //===================================================
    public Vector3 GetEndPos() {
        return massPoints[pointNum - 1].pos;
    }

    //===================================================
    // 終端質点の速度を取得
    //===================================================
    public Vector3 GetEndPointVel() {
        return massPoints[pointNum - 1].vel;
    }

    //===================================================
    // 終端質点に力を加える
    //===================================================
    public void AddForceToPoint(Vector3 _force) {
        moveForce = _force;
    }
}
