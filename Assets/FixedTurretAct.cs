using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FixedTurretAct : MonoBehaviour
{
    AudioSource audioSource;

    public Transform movingPart;
    public Transform gunPoint;
    public GameObject bulletPrefab;
    public AudioClip shotSE;

    GameObject[] player;
    float areaRange = 50.0f;
    float shotSpeed = 20.0f;
    float shotSpawn = 3.0f;
    float rotStep = 15.0f;

    bool withinRange = false;
    float time = 0.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        withinRange = false;
        for(int i = 0; i < player.Length; i++) {
            if(Vector3.Distance(transform.position, player[i].transform.position) < areaRange) {
                withinRange = true;
                break;
            }
        }
    }

	private void FixedUpdate() {
        if(withinRange && GameManager.instance.isPlaying) {
            time -= Time.deltaTime;
            RotToPlayer();
            if(time < 0.0f) {
                ShotBullet();
                time = shotSpawn;
            }
        }
	}

    void RotToPlayer() {
        // プレイヤーの位置を向く
        Quaternion lookRot = Quaternion.LookRotation((player[0].transform.position + new Vector3(0.0f, 1.0f, 0.0f)) - movingPart.position);
        Quaternion nextRot = Quaternion.RotateTowards(movingPart.rotation, lookRot, rotStep);

        // 回転を適用
        movingPart.rotation = nextRot;
    }

    void ShotBullet() {
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);

        /* 発射方向の設定 */
        Vector3 direction = (gunPoint.position - movingPart.position).normalized;
        bullet.GetComponent<Rigidbody>().velocity = direction * shotSpeed;
        bullet.GetComponent<Bullet>().myTurret = gameObject;

        /* 爆発エフェクトの表示 */
        EventManager.instance.SetExplosion(gunPoint.position, Color.red);

        // SEの再生
        audioSource.PlayOneShot(shotSE);
    }

    public void TakeDamage()
    {
        movingPart.eulerAngles = new Vector3(-35.0f, movingPart.eulerAngles.y, movingPart.eulerAngles.z);
    }
}
