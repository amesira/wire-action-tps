using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class HitStop : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ヒットストップを開始するメソッド
    public void StartHitStop(
        float duration, float intensity = 0.1f, 
        UnityAction onEnter = null, UnityAction onEntered = null, UnityAction onUpdate = null, UnityAction onExit = null)
    {
        StartCoroutine(HitStopSequence(duration, intensity, onEnter, onEntered, onUpdate, onExit));
    }

    // ------------- private
    // ヒットストップのコルーチン
    protected virtual IEnumerator HitStopSequence(float duration, float intensity, UnityAction onEnter, UnityAction onEntered, UnityAction onUpdate, UnityAction onExit)
    {
        // 停止前のイベントを呼び出す
        if (onEnter != null) onEnter.Invoke();
        transform.DOShakePosition(duration, intensity).SetUpdate(true);
        yield return null;

        // 停止開始直後のイベントを呼び出す
        if (onEntered != null) onEntered.Invoke();

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            // 停止中のイベントを呼び出す
            if (onUpdate != null) onUpdate.Invoke();
            
            yield return null; // 次のフレームまで待つ
        }

        // 停止終了のイベントを呼び出す
        if (onExit != null) onExit.Invoke();
    }
}
