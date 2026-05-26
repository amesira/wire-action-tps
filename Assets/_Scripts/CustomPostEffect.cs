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
        // 他のエフェクトタイプを追加可能
    }
    // ============== Inspector Variables ==============
    [SerializeField] Material radialBlurMaterial;

    // 現在のエフェクトタイプ
    [SerializeField] private EffectType currentEffect = EffectType.RadialBlur;

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

    // エフェクトのタイプ・強度を変更するメソッド
    public void ChangePostEffect(EffectType effectType, float intensity)
    {
        currentEffect = effectType;
        string intensityParamName = GetShaderIntensityName(effectType);
        Material effectMaterial = GetEffectMaterial(effectType);
        if(effectMaterial != null && !string.IsNullOrEmpty(intensityParamName))
        {
            effectMaterial.SetFloat(intensityParamName, intensity);
        }
    }

    // エフェクトのタイプ・強度を一定時間変更するメソッド
    public void ChangePostEffectTemporarily(EffectType effectType, float intensity, float changeDuration, float holdDuration)
    {
        if (playingEffectCoroutine != null)
        {
            StopCoroutine(playingEffectCoroutine);
        }
        playingEffectCoroutine = StartCoroutine(ChangePostEffectTemporarilyCoroutine(effectType, intensity, changeDuration, holdDuration));
    }

    private IEnumerator ChangePostEffectTemporarilyCoroutine(EffectType effectType, float intensity, float changeDuration, float holdDuration)
    {
        string intensityParamName = GetShaderIntensityName(effectType);
        Material effectMaterial = GetEffectMaterial(effectType);

        // エフェクトを変更
        ChangePostEffect(effectType, 0.0f);

        float elapsedTime = 0f;
        while (elapsedTime < changeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentIntensity = Mathf.Lerp(0f, intensity, elapsedTime / changeDuration);
            if(effectMaterial != null && !string.IsNullOrEmpty(intensityParamName))
            {
                effectMaterial.SetFloat(intensityParamName, currentIntensity);
            }
            yield return null; // 次のフレームまで待機
        }

        // エフェクトを保持
        yield return new WaitForSeconds(holdDuration);

        elapsedTime = 0f;
        while (elapsedTime < changeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentIntensity = Mathf.Lerp(intensity, 0f, elapsedTime / changeDuration);
            if(effectMaterial != null && !string.IsNullOrEmpty(intensityParamName))
            {
                effectMaterial.SetFloat(intensityParamName, currentIntensity);
            }
            yield return null; // 次のフレームまで待機
        }

        // エフェクトを元に戻す
        ChangePostEffect(EffectType.None, 0f);

        playingEffectCoroutine = null;
    }

    // ============== private helper methods ==============
    // エフェクトの強度のパラメータ名を取得するヘルパーメソッド
    private string GetShaderIntensityName(EffectType effectType)
    {
        switch (effectType)
        {
            case EffectType.RadialBlur:
                return "_BlurStrength";
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
            // 他のエフェクトタイプに対するマテリアルを追加可能
            default:
                return null;
        }
    }

}
