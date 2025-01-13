using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WhaleHp : MonoBehaviour
{
    public static WhaleHp instance;

    public Slider hpSlider;
    public Text hpText;

    public Image fillImage;
    public Color halfColor;
    public Color lastColor;

    public float partsMax = 0.0f;
    public float partsCnt = 0.0f;

    public Canvas canvas;
    public GameObject breakUIPrefab;

    BreakableBoxObject[] bbo;

    float curValue;
    float sliderValue;
    bool updateValue = false;

    public float t = 0.0f;
    Outline outline;

	private void Awake() {
        if(instance == null) {
            instance = this;
        }
	}
	void Start()
	{
		if(canvas == null) {
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }

        bbo = GetComponentsInChildren<BreakableBoxObject>();
        for(int i = 0; i < bbo.Length; i++) {
            partsCnt += bbo[i].partCnt;
        }
        partsMax = partsCnt;

        outline = hpText.gameObject.GetComponent<Outline>();

        /* スライダー設定 */
        hpSlider.value = partsCnt / partsMax;
        sliderValue = hpSlider.value;

        /* テキスト設定 */
        int max = Mathf.FloorToInt(partsMax);
        int cnt = Mathf.FloorToInt(partsCnt);
        hpText.text = cnt.ToString("D4") + "/" + max.ToString("D4");

        outline.effectColor = fillImage.color;
    }

	private void Update() {
        if(updateValue) {
            t += Time.deltaTime * 5.0f * (1.0f - t);
            hpSlider.value = Mathf.Lerp(curValue, sliderValue, t);

            /* テキスト設定 */
            int max = Mathf.FloorToInt(partsMax);
            int cnt = Mathf.FloorToInt(partsMax * hpSlider.value);
            hpText.text = cnt.ToString("D4") + "/" + max.ToString("D4");

            /* スライダーの色変更 */
            if(hpSlider.value < 0.2f) {
                fillImage.color = lastColor;
            }
            else if(hpSlider.value < 0.5f) {
                fillImage.color = halfColor;
            }

            /* テキストの色にも適用 */
            outline.effectColor = fillImage.color;

            if(t >= 0.98f) {
                t = 0.0f;
                hpSlider.value = sliderValue;
                updateValue = false;
            }
        }
	}

	public void SetBreakData(int _breakCnt) {
        int curCnt = Mathf.FloorToInt(partsCnt);

        /* 残りパーツを数える */
        partsCnt = 0.0f;
        for(int i = 0; i < bbo.Length; i++) {
            partsCnt += bbo[i].partCnt;
        }

        /* スライダーの値を設定 */
        curValue = sliderValue;
        sliderValue = partsCnt / partsMax;

        StartCoroutine(WaitUpdateValue());
    }

    IEnumerator WaitUpdateValue() {
        yield return new WaitForSeconds(0.1f);
        updateValue = true;
    }
}
