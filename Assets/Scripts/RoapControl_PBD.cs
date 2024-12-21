//===================================================
// RoapControl_PBD.cs
// 
// 作成者：北村美羽  作成日：2024/12/20
//===================================================
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

//===================================================
// 構造体宣言
//===================================================
struct MassPoint {  // 質点構造体
    public Vector3 pos;    // 位置
    public Vector3 vel;    // 速度
    public Vector3 estPos; // 推定位置

    public float mass; // 質量
    public bool isFixed; // 固定
}

//===================================================
// クラス宣言
//===================================================
/// <summary>
/// 拘束クラス
/// </summary>
public class Constraint {           // 拘束クラス
    public int massPoint1;          // 質点A
    public int massPoint2;          // 質点B
    public float defStretch;        // 通常状態の伸び

    public Constraint(int _massPoint1,int _massPoint2,float _defStretch) {
        massPoint1 = _massPoint1;
        massPoint2 = _massPoint2;
        defStretch = _defStretch;
    }
}

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
    public int pointNum;            // 質点の個数

    [Header("ロープの両端の位置")]
    public Transform startPoint;    // ロープの開始点
    public GameObject hangObj; // ロープにつり下がる物体

    [Header("ロープの計算パラメータ")]
    public float stiffness;         // ばねの強さ
    public float gravity;           // ロープにかかる重力
    public float pointMass;         // 質点の質量
    public float kDamping = 0.03f;

    [Header("ロープの描画パラメータ")]
    public float lineWidth;
    public Color lineColor;

    MassPoint[] massPoints = null;  // 質点
    Constraint[] constraints;       // 質点間の拘束

    Vector3 moveForce;              // ロープの終端に加わる力

    LineRenderer roapLine;

    GameObject[] pointObj;          // 質点に表示するオブジェクト（デバッグ用）

	private void Awake() {
        /* 質点の初期化 */
        massPoints = new MassPoint[pointNum];

        for (int i = 0; i < pointNum; i++) {
            /* ロープの始点からの相対的な位置割合 */
            float relaPos = (float)i / (pointNum - 1);

            /* 質点のパラメータを初期化 */
            massPoints[i].pos = Vector3.Lerp(startPoint.position,hangObj.transform.position, relaPos);
            massPoints[i].vel = Vector3.zero;
            massPoints[i].mass = pointMass;
        }

        /* Constraintの初期化 */
        constraints = new Constraint[pointNum - 1]; // 質点間の拘束を表す

        for (int i = 0; i < pointNum - 1; i++) {
            /* 通常の伸びを計算（Vector3.Magnitude：ベクトルの長さを表す） */
            float defStretch = Vector3.Magnitude(massPoints[i].pos - massPoints[i + 1].pos);

            /* 隣り合う質点を接続 */
            constraints[i] = new Constraint(i, i + 1, defStretch);
        }

        /* isFixedの初期化 */
        for (int i = 0; i < pointNum; i++) {
            massPoints[i].isFixed = false;
        }
        massPoints[0].isFixed = true;

        /* moveForceの初期化 */
        moveForce = Vector3.zero;

        /* LineRendererの初期化 */
        roapLine = GetComponent<LineRenderer>();
        roapLine.positionCount = pointNum;
        roapLine.startWidth = roapLine.endWidth = lineWidth;
        roapLine.startColor = roapLine.endColor = lineColor;

        /* MassPointに設置するSphereを生成（デバッグ用） */
        pointObj = new GameObject[pointNum];
        for (int i = 0; i < pointNum; i++) {
            pointObj[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pointObj[i].name = "MassPointObject" + (i + 1);

            pointObj[i].transform.localScale = new Vector3(0.05f, 0.3f, 0.05f);
            Destroy(pointObj[i].GetComponent<SphereCollider>());
        }
	}

    void FixedUpdate()
    {
        /* 外力による速度変化 */
        for(int i = 0; i < pointNum; i++) {
            massPoints[i].vel += new Vector3(0.0f, gravity, 0.0f) * Time.deltaTime;

            if(massPoints[i].isFixed) {
                massPoints[i].vel = Vector3.zero;
            }
        }

        /* moveForceによる最終質点の移動 */
        massPoints[pointNum - 1].vel += moveForce;
        moveForce = Vector3.zero;

        /* hangObjとの差が一定距離以上開いた場合 */
        Vector3 hangDiff = hangObj.transform.position - massPoints[pointNum - 1].pos;
        if(Vector3.Magnitude(hangDiff) > 3.0f) {
            massPoints[pointNum - 1].vel += hangDiff;
        }

        /* VelocityDamping */
        VelocityDamping(pointNum, massPoints, kDamping);

        /* 位置の更新 */
        for (int i = 0; i < pointNum; i++) {
            /* 現在位置と速度から、推定位置を算出 */
            massPoints[i].estPos = massPoints[i].pos + massPoints[i].vel * Time.deltaTime;

            /* 位置を更新 */
            massPoints[i].pos = massPoints[i].estPos;
        }

        /* 開始点の位置設定 */
        massPoints[0].pos = startPoint.position;

        /* 速度の更新 */
        for (int i = 0; i < pointNum - 1; i++) {
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
            massPoints[c.massPoint1].vel += de1 / Time.deltaTime;
            massPoints[c.massPoint2].vel += de2 / Time.deltaTime;
        }

        /* LineRendererへ位置を反映 */
        for(int i = 0; i < pointNum; i++) {
            roapLine.SetPosition(i, massPoints[i].pos);
        }

        /* MassPointにSphereを表示（デバッグ用） */
        for (int i = 0; i < pointNum; i++) {
            pointObj[i].transform.position = massPoints[i].pos;
        }
    }

    //===================================================
    // 速度減衰関数
    //===================================================
    private void VelocityDamping(int _num, MassPoint[] _massPoints, float _k) {
        Vector3 cmPos = Vector3.zero;   // 質量中心
        Vector3 cmVel = Vector3.zero;   // 質量中心の速度
        float totalMass = 0.0f;         // 質量

        /* 質量中心の位置、速度を算出 */
        for (int i = 0; i < _num; i++) {
            cmPos += _massPoints[i].pos;
            cmVel += _massPoints[i].vel;
            totalMass += _massPoints[i].mass;
        }
        cmPos /= totalMass;
        cmVel /= totalMass;

        Vector3 L = Vector3.zero;           // 角運動量（回転する力の量）
        Matrix3x3 I = new Matrix3x3();      // 慣性テンソル（回転しやすさ）
        Vector3[] rs = new Vector3[_num];   // 角速度（実際の回転速度）

        for (int i = 0; i < _num; i++) {
            Vector3 r = _massPoints[i].pos - cmPos;
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
            L += Vector3.Cross(r, _massPoints[i].mass * _massPoints[i].vel);
            I.Add(R.MultiplyMatrix(R.Transpose()).MultiplyFloat(_massPoints[i].mass));
        }

        Vector3 omega = I.Inverse().MultiplyVector3(L);

        for (int i = 0; i < _num; i++) {
            Vector3 deltaV = cmVel + Vector3.Cross(omega, rs[i]) - _massPoints[i].vel;
            _massPoints[i].vel += _k * deltaV;
        }
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
