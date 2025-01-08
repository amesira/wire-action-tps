using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FixedTurretAct : MonoBehaviour
{
    public Transform movingPart;
    public Transform gunPoint;
    public GameObject bulletPrefab;

    GameObject[] player;
    float areaRange = 100.0f;
    float shotSpeed = 15.0f;
    float shotSpawn = 1.5f;
    float rotStep = 15.0f;

    bool withinRange = false;
    float time = 0.0f;

    void Start()
    {
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
        if(withinRange) {
            time -= Time.deltaTime;
            RotToPlayer();
            if(time < 0.0f) {
                ShotBullet();
                time = shotSpawn;
            }
        }
	}

    void RotToPlayer() {
        /* ‰ñ“]•ûŒü */
        Quaternion lookRot = Quaternion.LookRotation(player[0].transform.position - movingPart.position);
        Quaternion nextRot = Quaternion.RotateTowards(movingPart.rotation, lookRot, rotStep);

        /* ‰ñ“]‚³‚¹‚é */
        movingPart.rotation = nextRot;
    }

    void ShotBullet() {
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);
        Vector3 direction = (gunPoint.position - movingPart.position).normalized;
        bullet.GetComponent<Rigidbody>().velocity = direction * shotSpeed;

        Destroy(bullet, 10.0f);
    }
}
