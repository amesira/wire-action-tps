using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CustomPostEffect : MonoBehaviour
{
    [System.Serializable]
    public enum EffectType
    {
        None,
        RadialBlur,
        MonoMask,
        // 他のエフェクトタイプを追加可能
    }
    // ============== Inspector Variables ==============
    [SerializeField] Material radialBlurMaterial;
    [SerializeField] Material monochromeMaterial;

    // 現在のエフェクトタイプ
    [SerializeField] private EffectType currentEffect = EffectType.RadialBlur;
    // 現在のエフェクト優先度
    [SerializeField] private int currentEffectPriority = 0;

    private Coroutine playingEffectCoroutine;

    void Start()
    {
        currentEffect = EffectType.None;   
    }

    // RenderTextureを使用してエフェクトを適用する
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Material currentMaterial = GetEffectMaterial(currentEffect);
        if(currentMaterial == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        Graphics.Blit(src, dest, currentMaterial);
    }

    public void PlayPostEffect(EffectType postEffectType, float intensity, float duration, float holdDuration = 0f, int priority = 0) {
        if (priority < currentEffectPriority) {
            // 現在のエフェクトの方が優先度が高い場合は新しいエフェクトを無視
            return;
        }

        currentEffect = postEffectType;
        currentEffectPriority = priority;
        if (playingEffectCoroutine != null) {
            StopCoroutine(playingEffectCoroutine);
        }
        playingEffectCoroutine = StartCoroutine(PlayEffect(postEffectType, intensity, duration, holdDuration));
    }

    public void ResetPostEffect() {
        if (playingEffectCoroutine != null) {
            StopCoroutine(playingEffectCoroutine);
        }
        currentEffect = EffectType.None;
        currentEffectPriority = 0;
    }

    // ポストエフェクトを再生するためのメソッド
    private IEnumerator PlayEffect(
        EffectType postEffectType,
         float intensity, float duration, float holdDuration = 0f) 
    {
        Material effectMaterial = GetEffectMaterial(postEffectType);

        string intensityPropertyName = GetShaderIntensityName(postEffectType);
        if (effectMaterial == null || string.IsNullOrEmpty(intensityPropertyName)) {
            Debug.LogWarning("Invalid post effect type or missing material/intensity property.");
            yield break;
        }

        // エフェクトの強さを徐々に変化させるコルーチンを開始
        yield return StartCoroutine(ChangeMaterialIntensity(effectMaterial, intensityPropertyName, intensity, duration));

        // 強さが最大になった状態を一定時間保持する
        yield return new WaitForSecondsRealtime(holdDuration);

        // エフェクトの強さを徐々に0に戻すコルーチンを開始
        yield return StartCoroutine(ChangeMaterialIntensity(effectMaterial, intensityPropertyName, 0f, duration));

        // エフェクトを完全にオフにする
        currentEffect = EffectType.None;
        currentEffectPriority = 0;
    }

    // -------------------------- private

    private IEnumerator ChangeMaterialIntensity(Material mat, string propertyName, float targetIntensity, float duration) {
        if (mat == null) yield break;

        float initialIntensity = mat.GetFloat(propertyName);
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.unscaledDeltaTime; // 時間の流れに影響されないようにする
            float newIntensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsedTime / duration);
            mat.SetFloat(propertyName, newIntensity);
            yield return null;
        }

        mat.SetFloat(propertyName, targetIntensity); // 最終的に目標の強さを確実に設定
    }

    // ============== private helper methods ==============
    // エフェクトの強度のパラメータ名を取得するヘルパーメソッド
    private string GetShaderIntensityName(EffectType effectType)
    {
        switch (effectType)
        {
            case EffectType.RadialBlur:
                return "_BlurStrength";
            case EffectType.MonoMask:
                return "_MonoStrength";
            // 他のエフェクトタイプに対するパラメータ名を追加可能
            default:
                return "";
        }
    }

    // エフェクトのマテリアルを取得するヘルパーメソッド
    private Material GetEffectMaterial(EffectType effectType)
    {
        switch (effectType)
        {
            case EffectType.RadialBlur:
                return radialBlurMaterial;
            case EffectType.MonoMask:
                return monochromeMaterial;
            // 他のエフェクトタイプに対するマテリアルを追加可能
            default:
                return null;
        }
    }

}
