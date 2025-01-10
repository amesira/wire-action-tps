using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BreakableBoxObject : MonoBehaviour
{
    AudioSource audioSource;

    public float splitSpawn = 1.0f;
    public float forceMagnitude = 100.0f;

    public int partCnt;

    public AudioClip breakSE;

    List<GameObject> childParts = new List<GameObject>();

    void Awake()
    {
        InitSplit();
        partCnt = childParts.Count;
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
                    /* 分割パーツを生成 */
                    GameObject part = new GameObject(gameObject.name + "Part" + "(" + i + "," + j + "," + r + ")");

                    /* トランスフォーム設定 */
                    Vector3 pos = bounds.min + new Vector3((partSize.x * i) + (partSize.x / 2),
                                                           (partSize.y * j) + (partSize.y / 2),
                                                           (partSize.z * r) + (partSize.z / 2));
                    //pos -= bounds.center;
                    part.transform.position = pos;
                    //part.transform.localPosition = pos;
                    part.transform.rotation = transform.rotation;
                    part.transform.localScale = partSize;

                    /* 分割パーツの詳細設定 */
                    part.AddComponent<MeshFilter>().mesh = mesh;
                    part.AddComponent<MeshRenderer>().material = mat;
                    part.AddComponent<BoxCollider>();
                    part.AddComponent<ShrinkAndDisappear>();

                    /* レイヤーを設定 */
                    part.layer = gameObject.layer;

                    /* 分割パーツをリストに保存 */
                    childParts.Add(part);

                    /* 分割パーツを子オブジェクトにする */
                    part.transform.parent = this.transform;
                }
            }
        }

        Destroy(GetComponent<BoxCollider>());
        GetComponent<MeshRenderer>().enabled = false;
    }

	private void Start() {
        audioSource = GetComponent<AudioSource>();
	}

	private void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag == "PlayerAttack") {
            BeBreaken(collision.contacts[0].point);    // プレイヤーの攻撃を受けたら壊れる
            Debug.Log("Be Breaken");
        }
	}

    private void BeBreaken(Vector3 _contactPos) {
        int breakCnt = 0;

        /* 効果音を鳴らす */
        audioSource.PlayOneShot(breakSE);

        /* 壊れる処理 */
        int partsNum = childParts.Count;
        for(int i = 0; i < partsNum; i++) {
            int index = partsNum - (i + 1);

            /* 衝突位置から半径r以内のオブジェクトのみ処理を行う */
            if(Vector3.Distance(childParts[index].transform.position, _contactPos) < splitSpawn * 5.0f) {
                BreakenPart(index, _contactPos);
                breakCnt++;
            }
        }

        /* 残りのパーツの数が少なかったらそのまま壊れる */
        if(childParts.Count <= 10) {
            int remainNum = childParts.Count;
            for(int i = 0; i < remainNum; i++) {
                int remIndex = remainNum - (i + 1);
                BreakenPart(remIndex, childParts[remIndex].transform.position);
                breakCnt++;
            }
        }

        /* BreakUIを設定 */
        WhaleHp.instance.SetBreakData(breakCnt);
    }

    void BreakenPart(int _index,Vector3 _contactPos) {

        /* ペアレントを切り離す */
        childParts[_index].transform.parent = null;

        /* レイヤーを変更 */
        childParts[_index].layer = gameObject.layer + 1;

        /* 力を加える */
        Vector3 forceDirection = (_contactPos - childParts[_index].transform.position).normalized;
        float force = Mathf.Clamp
                        (1f / (Vector3.Distance(childParts[_index].transform.position, _contactPos) + 0.1f), 0, 10f);
        force *= forceMagnitude;
        childParts[_index].AddComponent<Rigidbody>().AddForce(forceDirection * force, ForceMode.Impulse);

        /* 収縮したのち削除する処理 */
        childParts[_index].GetComponent<ShrinkAndDisappear>().StartShrinking(1.5f, 2.0f);

        /* リストから削除 */
        childParts.RemoveAt(_index);
        partCnt--;
    }
}
