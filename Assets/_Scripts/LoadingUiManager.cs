using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUiManager : MonoBehaviour
{
    [System.Serializable]
    private class LoadingUiLine
    {
        public List<Animation> animations;
    }

    // ============== Inspector Variables ==============
    // ロードUIグループ
    [SerializeField] private List<LoadingUiLine> loadingUiLines;
    [SerializeField] private float animationDelay = 0.3f;

    private bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        //Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Open()
    {
        Initialize();
        for (int i = 0; i < loadingUiLines.Count; i++)
        {
            StartCoroutine(PlayAnimations(1f, loadingUiLines[i].animations));
        }
    }

    public void Close()
    {
        Initialize();
        for (int i = 0; i < loadingUiLines.Count; i++)
        {
            StartCoroutine(PlayAnimations(-1f, loadingUiLines[i].animations));
        }
    }

    // 初期化
    public void Initialize()
    {
        if (!initialized)
        {
            for (int i = 0; i < loadingUiLines.Count; i++)
            {
                for (int j = 0; j < loadingUiLines[i].animations.Count; j++)
                {
                    loadingUiLines[i].animations[j].gameObject.SetActive(true);

                    // アニメーションをサンプリングして、初期状態を設定
                    AnimationState state = loadingUiLines[i].animations[j][loadingUiLines[i].animations[j].clip.name];

                    state.enabled = true;
                    state.weight = 1.0f;
                    state.time = 0.0f;

                    loadingUiLines[i].animations[j].Sample();
                    state.enabled = false;
                }
            }
            initialized = true;
        }
    }

    // アニメーションを再生するコルーチン
    private IEnumerator PlayAnimations(float speed, List<Animation> animations)
    {

        foreach (var anim in animations)
        {
            // アニメーションの速度を設定
            anim[anim.clip.name].speed = speed;
            // アニメーションの再生位置を設定（先頭または末尾）
            anim[anim.clip.name].time = speed > 0f ? 0f : anim[anim.clip.name].length;

            // アニメーションをサンプリングして、初期状態を設定
            AnimationState state = anim[anim.clip.name];

            state.enabled = true;
            state.weight = 1.0f;
            state.time = speed > 0f ? 0f : anim[anim.clip.name].length;

            anim.Sample();
            state.enabled = false;
        }

        foreach (var anim in animations)
        {
            anim.Play();
            yield return new WaitForSeconds(animationDelay);
        }
    }
}
