//===================================================
// MovePlayer.cs
// 
// 作成者：北村美羽  作成日：2024/12/19
//===================================================
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    Rigidbody rb;                       // プレイヤーのRigidBody
    WireActionPlayer wap;
    PlayerAnimeControl anim;
    SwordActionPlayer sap;
    
    public LayerMask groundLayer;       // 地面レイヤー
    public Camera playerCamera;

    [Header("プレイヤーのパラメータ")]
    public float moveSpeed;             // 移動速度
    public float jumpPower;             // ジャンプ力

    [Space]
    public float rotStep = 15.0f;       // 回転する大きさ
    
    [Header("プレイヤーの部位")]
    public BoxCollider groundCheckCol;  // 接地確認用コライダー

    float inputHorizontal;              // 左右キーの入力
    float inputVertical;                // 前後キーの入力

    bool inputJumpKey;                  // ジャンプキーが押された

    void Start()
    {
        /* コンポーネントの取得 */
        rb = GetComponent<Rigidbody>();
        wap = GetComponent<WireActionPlayer>();
        anim = GetComponent<PlayerAnimeControl>();
        sap = GetComponent<SwordActionPlayer>();

        /* 初期化処理 */
        inputJumpKey = false;
        groundCheckCol.enabled = false;
    }

    void Update()
    {
        /* 移動キー入力 */
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        /* ジャンプキー入力 */
        if(Input.GetKeyDown(KeyCode.Space)) {
            inputJumpKey = true;
        }

        /* ワイヤーアクション入力 */
        wap.CheckInputWireButton();

        /* 攻撃アクション入力 */
        sap.CheckInputActionButton();
    }

	private void FixedUpdate() {
        /* アンカーターゲットを設定 */
        wap.SetAnchorTarget(playerCamera);

		/* 移動方向を取得 */
        Vector3 cameraForward 
            = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight
            = Vector3.Scale(playerCamera.transform.right, new Vector3(1, 0, 1)).normalized;

        Vector3 moveForward
            = (cameraForward * inputVertical + cameraRight * inputHorizontal).normalized;

        /* 移動処理 */
        Vector3 moveVel = moveForward * moveSpeed + new Vector3(0, rb.velocity.y, 0);

        /* 接地判定用の一時コライダーを作成 */
        bool isGround = Physics.CheckBox(transform.position + groundCheckCol.center,
            groundCheckCol.size / 2, Quaternion.identity,groundLayer);

        /* ジャンプ */
        if(inputJumpKey) {
            inputJumpKey = false;

            if(isGround) {  // ジャンプ処理
                moveVel += new Vector3(0, jumpPower, 0);
            }
        }

        /* 速度設定 */
        if(wap.isLink) {    // ロープにつながっている場合
            moveVel.y = 0.0f;
            rb.velocity = wap.LinkRoap(moveVel);
        }
        else {
            rb.velocity = moveVel;
        }

        /* プレイヤーの向きを進行方向へ回転 */
        if(moveForward != Vector3.zero) {
            /* 最終的な回転方向 */
            Vector3 lookForward = moveForward;
            lookForward.y = 0.0f;
            Quaternion lookRot = Quaternion.LookRotation(lookForward);

            /* 実際の回転方向 */
            Quaternion nextRot = Quaternion.RotateTowards(transform.rotation, lookRot, rotStep);

            /* 回転させる */
            transform.rotation = nextRot;
        }

        /* ワイヤーコントロール */
        wap.WireAnchorControl();

        /* アニメーション */
        if(Vector3.Magnitude(moveVel) > 0.1f && !wap.isLink) {
            anim.SetRunning(true);
        }
        else {
            anim.SetRunning(false);
        }
        anim.SetScaleVertical(rb.velocity.y);

        /* 剣の軌跡 */
        sap.UpdateTrace();
	}
}
