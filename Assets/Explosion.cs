using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Explosion : MonoBehaviour
{
    Color effectColor;       // エフェクトの色
    public Vector3 effectSize;      // エフェクトの大きさ
    public int effectNum;           // エフェクトの数
    public float power;             // 拡散力

    public GameObject effectPrefab;

    Material source;

	void Start()
	{
        //GenerateEffectCube();
    }

    public void StartEffect(Color color) {
        effectColor = color;
        GenerateEffectCube();
    }
    // エフェクトを生成
    void GenerateEffectCube() {
        source = Resources.Load<Material>("Materials/EffectMat");
        float phi = (1 + Mathf.Sqrt(5)) / 2; // 黄金比
        for(int i = 0; i < effectNum; i++) {
            GameObject effectObj = Instantiate(effectPrefab);

            /* Transform設定 */
            effectObj.transform.position = transform.position;
            effectObj.transform.localScale = effectSize;

            /* マテリアルと色を設定 */
            MeshRenderer effectRenderer = effectObj.GetComponent<MeshRenderer>();
            effectRenderer.material = new Material(source);
            effectRenderer.material.color = effectColor;

            /* 力の方向を算出 */
            float y = 1 - (i / (float)(effectNum - 1)) * 2; // -1から1までの範囲で均等に
            float radius = Mathf.Sqrt(1 - y * y); // 半径を計算
            float theta = 2 * Mathf.PI * i / phi; // 黄金比による角度
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 direction = new Vector3(x, y, z).normalized;

            /* RigidBody設定 */
            Rigidbody effectRb = effectObj.GetComponent<Rigidbody>();
            effectRb.AddForce(direction * power, ForceMode.Impulse);

            /* 小さくなって消えるスクリプトを追加 */
            effectObj.AddComponent<ShrinkAndDisappear>().StartShrinking(0.3f, 1.5f);
        }
    }
}
