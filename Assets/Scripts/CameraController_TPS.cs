using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_TPS : MonoBehaviour
{
    Rigidbody rb;

    public GameObject player;

    [Header("カメラ追従パラメータ")]
    public float cameraSpeed;
    public float rotateSpeed;

    Camera tpsCamera;
    float verticalRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        tpsCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveForward = player.transform.position - this.transform.position;
        rb.velocity = moveForward * cameraSpeed;

        // マウスの移動量
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

        // targetの位置のY軸を中心に、回転（公転）する
        transform.Rotate(Vector3.up, mouseX);

        // 縦回転の制御（ピッチ）
        verticalRotation -= mouseY; // マウスのY軸移動で角度を減少させる
        verticalRotation = Mathf.Clamp(verticalRotation, -45.0f, 45.0f);

        // 縦回転を適用（ローカル回転を制御）
        transform.localRotation = Quaternion.Euler(verticalRotation, this.transform.localRotation.eulerAngles.y, 0);
    }
}
