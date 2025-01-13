using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    SphereCollider spCol;

    public GameObject myTurret;

    float delRemit = 20.0f;

    void Start()
    {
        spCol = GetComponent<SphereCollider>();
        spCol.enabled = false;

        /* 一定時間経過後に自身を削除 */
        Destroy(gameObject, delRemit);
    }

	private void FixedUpdate() {
        bool isHit = false;

        Collider[] hitCol = Physics.OverlapSphere(transform.position, spCol.radius);
        foreach(Collider col in hitCol) {
            /* 自身を発射した砲台ではない何かと衝突した場合 */
            if(col.gameObject != myTurret) {
                Debug.Log(col.gameObject.name + "と衝突！");
                if(col.gameObject.tag == "Player") {
                    /* Playerにダメージを与える */
                    col.gameObject.GetComponent<PlayerSystem>().DamagedPlayer();
                }
                isHit = true;
            }
        }

        if(isHit) {
            EventManager.instance.SetExplosion(transform.position, Color.red);
            Destroy(gameObject);
        }
	}
}
