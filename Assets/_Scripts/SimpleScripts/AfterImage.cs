using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public float generateSpawn = 0.1f;
    public Color afterImageColor;

    public bool isAfterImage = false;

    Mesh afterImageMesh;
    float time = 0.0f;

    Material source;

    void Start() {
        time = generateSpawn;

        /* 子オブジェクトも含めて全てのMeshFilterを取得 */
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        /* 各メッシュをCombineInstanceに入れる */
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for(int i = 0; i < meshFilters.Length; i++) {
            combine[i].mesh = meshFilters[i].mesh;
            combine[i].transform = transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
        }

        /* 統合したメッシュを作成 */
        afterImageMesh = new Mesh();
        afterImageMesh.name = gameObject.name + "(AfterImageMesh)";
        afterImageMesh.CombineMeshes(combine);

        afterImageMesh.RecalculateBounds();

        /* 残像用のマテリアルを取得 */
        source = Resources.Load<Material>("Materials/AfterImageMat");
    }

    void FixedUpdate()
    {
        if(isAfterImage) {
            time -= Time.deltaTime;

            if(time < 0.0f) {
                time = generateSpawn;

                /* 残像オブジェクトを作成 */
                GameObject afterImage = new GameObject(gameObject.name + "(AgterImage)");
                afterImage.transform.position = transform.position;
                afterImage.transform.rotation = transform.rotation;
                afterImage.transform.localScale = transform.lossyScale;

                /* メッシュを設定 */
                MeshFilter newFilter = afterImage.AddComponent<MeshFilter>();
                newFilter.mesh = afterImageMesh;

                /* マテリアルと色を設定 */
                MeshRenderer newRenderer = afterImage.AddComponent<MeshRenderer>();
                newRenderer.material = new Material(source);
                newRenderer.material.color = afterImageColor;

                /* 色を徐々に透明にして削除するコンポーネント */
                afterImage.AddComponent<LightlyAndDisappear>();
            }
        }
    }
}
