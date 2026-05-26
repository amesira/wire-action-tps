using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXService : MonoBehaviour
{
    public static VFXService instance;
    void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    //============= Inspector Variables ==============
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CustomPostEffect customPostEffect;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // CameraControllerのラッパー

    // カメラFOVを変更する
    public void ChangeFOV(float targetFOV, float duration) {
        cameraController.ChangeFOV(targetFOV, duration);
    }
    // カメラFOVを一定時間変更する
    public void ChangeFOVTemporarily(float targetFOV, float changeDuration, float holdDuration) {
        cameraController.ChangeFOVTemporarily(targetFOV, changeDuration, holdDuration);
    }
    // カメラFOVを元に戻す
    public void ResetFOV(float duration) {
        cameraController.ResetFOV(duration);
    }

    // カメラシェイク
    public void StartCameraShake(float duration, float magnitude) {
        cameraController.StartShake(duration, magnitude);
    }
    // カメラオフセットを変更する
    public void ChangeCameraBoxOffset(Vector3 newOffset) {
        cameraController.ChangeCameraBoxOffset(newOffset);
    }
    // カメラオフセットを元に戻す
    public void ResetCameraBoxOffset() {
        cameraController.ResetCameraBoxOffset();
    }

    // CustomPostEffectsのラッパー
    public void PlayPostEffect(CustomPostEffect.EffectType postEffectType, float intensity, float duration, float holdDuration = 0f, int priority = 0) {
        customPostEffect.PlayPostEffect(postEffectType, intensity, duration, holdDuration, priority);
    }
    public void ResetPostEffect() {
        customPostEffect.ResetPostEffect();
    }

}
