using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhaleHp : MonoBehaviour
{
    public Slider hpSlider;

    public Image fillImage;
    public Color halfColor;
    public Color lastColor;

    public float partsMax = 0.0f;
    public float partsCnt = 0.0f;

    float time = 0.2f;

    BreakableBoxObject[] bbo;

    void Start()
    {
        bbo = GetComponentsInChildren<BreakableBoxObject>();
        for(int i = 0; i < bbo.Length; i++) {
            partsCnt += bbo[i].partCnt;
        }
        partsMax = partsCnt;

        /* スライダー設定 */
        hpSlider.value = partsCnt / partsMax;
    }

    void Update()
    {
        time -= Time.deltaTime;

        if(time < 0.0f) {
            time = 0.2f;
            partsCnt = 0.0f;
            for(int i = 0; i < bbo.Length; i++) {
                partsCnt += bbo[i].partCnt;
            }

            /* スライダー設定 */
            hpSlider.value = partsCnt / partsMax;

            /* スライダーの色変更 */
            if(hpSlider.value < 0.1f) {
                fillImage.color = lastColor;
            }
            else if(hpSlider.value < 0.5f) {
                fillImage.color = halfColor;
            }
        }
    }
}
