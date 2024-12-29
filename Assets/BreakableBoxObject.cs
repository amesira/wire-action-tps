using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BreakableBoxObject : MonoBehaviour
{
    public float splitSpawn = 1.0f;
    public float forceMagnitude = 100.0f;

    List<GameObject> childParts = new List<GameObject>();

    void Start()
    {
        InitSplit();
    }

    private void InitSplit() {
        Bounds bounds = GetComponent<BoxCollider>().bounds;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Material mat = GetComponent<MeshRenderer>().material;

        /* 分割数を設定 */
        int splitX = (int)(bounds.size.x / splitSpawn) + 1;
        int splitY = (int)(bounds.size.y / splitSpawn) + 1;
        int splitZ = (int)(bounds.size.z / splitSpawn) + 1;

        /* 分割されたパーツの大きさを設定 */
        Vector3 partSize = new Vector3(bounds.size.x / splitX, bounds.size.y / splitY, bounds.size.z / splitZ);

        for(int i = 0; i < splitX; i++) {
            for(int j = 0; j < splitY; j++) {
                for(int r = 0; r < splitZ; r++) {
                    /* 生成位置を設定 */
                    Vector3 pos = bounds.min + new Vector3((partSize.x * i) + (partSize.x / 2),
                                                           (partSize.y * j) + (partSize.y / 2),
                                                           (partSize.z * r) + (partSize.z / 2));
                    /* 分割パーツを生成 */
                    GameObject part = new GameObject(gameObject.name + "Part" + "(" + i + "," + j + "," + r + ")");
                    part.AddComponent<MeshFilter>().mesh = mesh;
                    part.AddComponent<MeshRenderer>().material = mat;
                    //GameObject part = Instantiate(gameObject, pos, transform.rotation, this.transform);
                    part.transform.position = pos;
                    part.transform.rotation = transform.rotation;
                    part.transform.localScale = partSize;

                    /* 分割パーツの詳細設定 */
                    part.AddComponent<BoxCollider>();

                    /* 分割パーツをリストに保存 */
                    childParts.Add(part);
                }
            }
        }

        Destroy(GetComponent<BoxCollider>());
        GetComponent<MeshRenderer>().enabled = false;
    }

	private void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "PlayerAttack") {
            BeBreaken(collision.contacts[0].point);    // プレイヤーの攻撃を受けたら壊れる
        }
	}

    private void BeBreaken(Vector3 _contactPos) {
        Bounds bounds = GetComponent<BoxCollider>().bounds;

        /* 分割数を設定 */
        int splitX = (int)(bounds.size.x / splitSpawn) + 1;
        int splitY = (int)(bounds.size.y / splitSpawn) + 1;
        int splitZ = (int)(bounds.size.z / splitSpawn) + 1;

        Debug.Log("bounds.size : " + bounds.size);
        Debug.Log("splitX : " + splitX);
        Debug.Log("splitY : " + splitY);
        Debug.Log("splitZ : " + splitZ);

        /* 分割されたパーツの大きさを設定 */
        Vector3 partSize = new Vector3(bounds.size.x / splitX, bounds.size.y / splitY, bounds.size.z / splitZ);

        Debug.Log("partsize : " + partSize);

        List<GameObject> remainParts = new List<GameObject>();

        for(int i = 0; i < splitX; i++) {
            for(int j = 0; j < splitY; j++) {
                for(int r = 0; r < splitZ; r++) {
                    /* 生成位置を設定 */
                    Vector3 pos = bounds.min + new Vector3((partSize.x * i) + (partSize.x / 2),
                                                           (partSize.y * j) + (partSize.y / 2),
                                                           (partSize.z * r) + (partSize.z / 2));
                    /* 分割パーツを生成 */
                    GameObject part = Instantiate(gameObject, pos, transform.rotation);
                    part.transform.localScale = partSize;

                    /* 分割パーツの詳細設定 */
                    Destroy(part.GetComponent<BreakableBoxObject>());

                    if(Vector3.Distance(part.transform.position, _contactPos) < 3.0f) {
                        /* 分割パーツに力を加える */
                        Rigidbody rb = part.GetComponent<Rigidbody>();
                        if(rb) {
                            Vector3 forceDirection = (part.transform.position - _contactPos).normalized;
                            float force = Mathf.Clamp
                                (1f / (Vector3.Distance(part.transform.position, _contactPos) + 0.1f), 0, 10f);
                            force *= forceMagnitude;
                            rb.AddForce(forceDirection * force, ForceMode.Impulse);
                        }
                    }
                    else {
                        remainParts.Add(part);
                    }
                }
            }
        }

        GameObject margeObject = MargeMesh(remainParts, gameObject.name + "(breaken)", transform.position);
        margeObject.AddComponent<MeshCollider>().convex = true;
        margeObject.AddComponent<BoxCollider>().enabled = false;
        margeObject.AddComponent<Rigidbody>();
        margeObject.AddComponent<BreakableBoxObject>();
        margeObject.layer = gameObject.layer;

        Destroy(gameObject);
    }

    private GameObject MargeMesh(List<GameObject> _margeParts,string _objName,Vector3 _basePos) {
        CombineInstance[] combineInstanceAry = new CombineInstance[_margeParts.Count];
        Material partMat = _margeParts[0].GetComponent<MeshRenderer>().material;
        int partsCnt = _margeParts.Count;

        /* 結合先オブジェクトを生成 */
        GameObject margeObj = new GameObject(_objName);
        margeObj.transform.position = _basePos;

        /* メッシュ情報の設定 */
        for(int i = 0; i < partsCnt; i++) {
            /* メッシュを設定 */
            Mesh partMesh = _margeParts[i].GetComponent<MeshFilter>().mesh;
            combineInstanceAry[i].mesh = partMesh;

            /* ワールド座標系から結合後オブジェクトのローカル空間へ変換 */
            Matrix4x4 partMatrix = _margeParts[i].transform.localToWorldMatrix;
            Matrix4x4 newObjMatrix = margeObj.transform.worldToLocalMatrix;
            combineInstanceAry[i].transform = newObjMatrix * partMatrix;

            /* パーツオブジェクトを削除 */
            Destroy(_margeParts[i]);
        }

        /* メッシュ結合 */
        Mesh combinedMesh = new Mesh();
        combinedMesh.name = _objName + "Mesh";
        combinedMesh.CombineMeshes(combineInstanceAry);

        /* 結合したメッシュでオブジェクトを設定 */
        margeObj.AddComponent<MeshFilter>().mesh = combinedMesh;
        margeObj.AddComponent<MeshRenderer>().material = partMat;

        return margeObj;
    }
}
