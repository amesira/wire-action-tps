using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_TPS : MonoBehaviour
{
    Rigidbody rb;

    public Camera camera;
    public GameObject target;

    [Header("カメラ追従パラメータ")]
    public float cameraSpeed;

    [Header("カメラ操作パラメータ")]
    public float rotateSpeed;
    public float angleLimit = 45.0f;

    float verticalRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowTarget();
        CameraControl();
    }

    void FollowTarget() {
        /* ターゲット追従 */
        Vector3 moveForward = target.transform.position - this.transform.position;
        rb.velocity = moveForward * cameraSpeed;
    }

    void CameraControl() {
        /* マウスの移動量を取得 */
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

        /* 横回転の制御 */
        this.transform.Rotate(Vector3.up, mouseX);

        /* 縦方向の回転量 */
        verticalRotation -= mouseY; // マウスのY軸移動で角度を減少させる
        verticalRotation = Mathf.Clamp(verticalRotation, -angleLimit, angleLimit);

        /* 縦回転を適用（ローカル回転を制御） */
        this.transform.localRotation = Quaternion.Euler(verticalRotation, this.transform.localRotation.eulerAngles.y, 0);
    }
}
