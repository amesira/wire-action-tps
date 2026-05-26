using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // メインカメラ
    [SerializeField] private bool isActive = true; // カメラコントローラーが有効かどうか

    [Header("カメラコントロールオブジェクト設定")]
    [SerializeField] private Transform pivotX;    // カメラの水平回転の中心
    [SerializeField] private Transform pivotY;    // カメラの垂直回転の中心
    [SerializeField] private Transform cameraBox; // カメラの位置を制御するオブジェクト

    [Header("TPSカメラ設定")]
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f); // ターゲットからのオフセット
    [SerializeField] private Vector3 cameraBoxOffset = new Vector3(0f, 0f, 0f); // カメラボックスのオフセット（Distanceと加算して使用）
    [SerializeField] private GameObject target; // カメラが追従するターゲット
    [SerializeField] private float distance = 5.0f; // ターゲットからの距離

    [Space(10)]
    [SerializeField] private float mouseSensitivity = 3.0f; // マウス感度
    [SerializeField] private float followTime = 20.0f; // カメラの追従速度
    [SerializeField] private float rotationSmoothTime = 0.1f; // カメラの回転のスムーズ時間
    [SerializeField] private float positionSmoothTime = 0.1f; // カメラの位置のスムーズ時間

    [Space(10)]
    [SerializeField] private float maxPitch = 80f; // カメラの垂直回転の最大角度
    [SerializeField] private float minPitch = -80f; // カメラの垂直回転の最小角度

    private float originalFOV; // カメラの元のFOVを保存する変数
    private Vector3 originalOffset; // カメラの元のオフセットを保存する変数

    // 回転角度のターゲット値
    private float targetYaw = 0f; // ターゲットの水平回転角度
    private float targetPitch = 0f; // ターゲットの垂直回転角度
    // 回転角度の初期値
    private float initialYaw = 0f; // 初期の水平回転角度
    private float initialPitch = 0f; // 初期の垂直回転角度
    // カメラシェイク用の変数
    private bool isShaking = false; // シェイク中かどうかのフラグ

    // SmoothDamp用の変数
    private Vector3 cameraVelocity = Vector3.zero; // カメラの移動速度
    private float yawVelocity = 0f; // カメラの水平回転速度
    private float pitchVelocity = 0f; // カメラの垂直回転速度
    private Vector3 cameraBoxVelocity = Vector3.zero; // カメラボックスの移動速度

    // 再生中のコルーチンを管理するための変数
    private Coroutine shakeCoroutine;
    private Coroutine fovCoroutine;
    

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        originalFOV = mainCamera.fieldOfView; // 元のFOVを保存

        // カメラの初期位置を設定する
        cameraBox.localPosition = new Vector3(0f, 0f, -distance);

        originalOffset = targetOffset; // 元のオフセットを保存

        // ターゲットの初期回転角度を取得する
        initialYaw = pivotX.localEulerAngles.y;
        initialPitch = pivotY.localEulerAngles.x;
    }

    void Update()
    {
        if (!isActive) return;

        float deltaTime = Time.unscaledDeltaTime;

        UpdateThirdPersonCamera(deltaTime);

        // カメラを回転させる
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        float currentYaw = pivotX.localEulerAngles.y;
        float currentPitch = pivotY.localEulerAngles.x;

        pivotX.localRotation = Quaternion.Euler(0f, Mathf.SmoothDampAngle(currentYaw, targetYaw, ref yawVelocity, rotationSmoothTime, Mathf.Infinity, deltaTime), 0f);
        pivotY.localRotation = Quaternion.Euler(Mathf.SmoothDampAngle(currentPitch, targetPitch, ref pitchVelocity, rotationSmoothTime, Mathf.Infinity, deltaTime), 0f, 0f);

        if (!isShaking)
        {
            // カメラボックス位置を更新する
            cameraBox.localPosition = Vector3.SmoothDamp(cameraBox.localPosition, new Vector3(0f, 0f, -distance) + cameraBoxOffset, ref cameraBoxVelocity, positionSmoothTime, Mathf.Infinity, deltaTime);
        }

        // カメラの位置を更新する
        mainCamera.transform.position = cameraBox.position;
        mainCamera.transform.rotation = cameraBox.rotation;
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    // カメラシェイクを開始する関数
    public void StartShake(float duration, float magnitude)
    {
        if (!isShaking)
        {
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine); // 既存のシェイクコルーチンを停止
            isShaking = true;
            shakeCoroutine = StartCoroutine(Shake(duration, magnitude));
        }
    }

    // カメラFOVを変更する関数
    public void ChangeFOV(float targetFOV, float duration)
    {
        if (fovCoroutine != null) StopCoroutine(fovCoroutine); // 既存のFOV変更コルーチンを停止
        fovCoroutine = StartCoroutine(ChangeFOVCoroutine(targetFOV, duration));
    }

    // カメラFOVを元に戻す関数
    public void ResetFOV(float duration)
    {
        if (fovCoroutine != null) StopCoroutine(fovCoroutine); // 既存のFOV変更コルーチンを停止
        fovCoroutine = StartCoroutine(ChangeFOVCoroutine(originalFOV, duration));
    }

    // カメラFOVを一定時間変更してから元に戻す関数
    public void ChangeFOVTemporarily(float targetFOV, float changeDuration, float holdDuration)
    {
        if (fovCoroutine != null) StopCoroutine(fovCoroutine); // 既存のFOV変更コルーチンを停止
        fovCoroutine = StartCoroutine(ChangeFOVTemporarilyCoroutine(targetFOV, changeDuration, holdDuration));
    }

    // カメラオフセットを変更する関数
    public void ChangeCameraOffset(Vector3 newOffset)
    {
        targetOffset = newOffset;
    }

    // カメラオフセットを元に戻す関数
    public void ResetCameraOffset()
    {
        targetOffset = originalOffset;
    }

    // カメラボックスオフセットを変更する関数
    public void ChangeCameraBoxOffset(Vector3 newOffset)
    {
        cameraBoxOffset = newOffset;
    }

    // カメラボックスオフセットを元に戻す関数
    public void ResetCameraBoxOffset()
    {
        cameraBoxOffset = Vector3.zero;
    }
    // カメラの回転角度をリセットする関数
    public void ResetCameraRotation()
    {
        targetYaw = initialYaw;
        targetPitch = initialPitch;
        pivotX.localRotation = Quaternion.Euler(0f, initialYaw, 0f);
        pivotY.localRotation = Quaternion.Euler(initialPitch, 0f, 0f);
    }

    // ------------------------------- private

    // TPSカメラの更新処理
    private void UpdateThirdPersonCamera(float deltaTime)
    {
        Vector3 desiredPosition = Vector3.zero;
        desiredPosition = target.transform.position + targetOffset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref cameraVelocity, followTime, Mathf.Infinity, deltaTime);

        // マウス入力を取得してカメラの回転角度を更新する
        Vector2 input = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        if (input.sqrMagnitude < 0.01f) return;

        targetYaw += input.x * mouseSensitivity * deltaTime;
        targetPitch -= input.y * mouseSensitivity * deltaTime;
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration * 0.5f;
            if (t > 1f) t = 1f;
            magnitude = Mathf.Lerp(magnitude, 0f, t * t); // シェイクの強さを徐々に減少させる

            // Perlinノイズを使用してシェイクのオフセットを生成
            Vector3 shakeOffset = Vector3.zero;
            shakeOffset.x = Perlin1D(elapsed * 10f) * magnitude;
            shakeOffset.y = Perlin1D((elapsed + 100f) * 10f) * magnitude;

            // カメラの位置にシェイクオフセットを加算
            cameraBox.localPosition = new Vector3(0f, 0f, -distance) + shakeOffset;

            yield return null; // 次のフレームまで待機
        }
        isShaking = false; // シェイク終了
    }

    private IEnumerator ChangeFOVCoroutine(float targetFOV, float duration)
    {
        float startFOV = mainCamera.fieldOfView;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / duration);
            yield return null; // 次のフレームまで待機
        }
        mainCamera.fieldOfView = targetFOV; // 最終的なFOVを確実に設定
    }

    private IEnumerator ChangeFOVTemporarilyCoroutine(float targetFOV, float changeDuration, float holdDuration)
    {
        // FOVを変更する
        yield return StartCoroutine(ChangeFOVCoroutine(targetFOV, changeDuration));

        // 変更したFOVを一定時間保持する
        yield return new WaitForSecondsRealtime(holdDuration);

        // FOVを元に戻す
        yield return StartCoroutine(ChangeFOVCoroutine(originalFOV, changeDuration));
    }

    // 補間
    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    // ランダムなグラデーションを生成する関数
    private float RandomGradient(int x)
    {
        x = (x << 13) ^ x;
        return 1.0f - ((x * (x * x * 15731 + 789221) + 1376312589)
            & 0x7fffffff) / 1073741824.0f;
    }

    // 1D Perlinノイズを生成する関数
    private float Perlin1D(float x)
    {
        int x0 = Mathf.FloorToInt(x);
        int x1 = x0 + 1;

        float t = x - x0;

        float g0 = RandomGradient(x0);
        float g1 = RandomGradient(x1);

        float v0 = g0 * (x - x0);
        float v1 = g1 * (x - x1);

        return Mathf.Lerp(v0, v1, Fade(t));
    }
}