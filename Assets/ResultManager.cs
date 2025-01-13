using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Image fadeImage;
    public Text loadText;

    public Text resultText;
    public Text resultSubText;

    public Text remainTime;
    public Text whaleHpRate;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOut());

        if(SendResult.instance.resultNum == 0) {
            resultText.text = "ゲームオーバー";
            resultText.color = Color.red;
        }
        else {
            resultText.text = "ゲームクリア！";
            resultText.color = Color.yellow;
        }
        resultSubText.text = SendResult.instance.subStr;
        remainTime.text = "残り時間：" + SendResult.instance.timerRemain.ToString("f2");
        whaleHpRate.text = "クジラ破壊率：" + ((1.0f - SendResult.instance.whaleHpRate) * 100f).ToString("f2") + "％";

        Destroy(SendResult.instance.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeOut() {
        Color color = fadeImage.color;
        color.a = 1.0f;
        fadeImage.color = color;

        color = loadText.color;
        color.a = 1.0f;
        loadText.color = color;

        fadeImage.gameObject.SetActive(true);
        /* フェードアウト */
        for(int i = 0; i < 1000; i++) {
            Color newColor = fadeImage.color;
            newColor.a -= Time.deltaTime;
            fadeImage.color = newColor;

            newColor = loadText.color;
            newColor.a -= Time.deltaTime;
            loadText.color = newColor;

            if(fadeImage.color.a < 0.0f) {
                break;
            }

            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
    public void BackTitle() {
        StartCoroutine(FadeIn());
    }
    IEnumerator FadeIn() {
        Color color = fadeImage.color;
        color.a = 0.0f;
        fadeImage.color = color;

        color = loadText.color;
        color.a = 0.0f;
        loadText.color = color;

        fadeImage.gameObject.SetActive(true);
        /* フェードアウト */
        for(int i = 0; i < 1000; i++) {
            Color newColor = fadeImage.color;
            newColor.a += Time.deltaTime;
            fadeImage.color = newColor;

            newColor = loadText.color;
            newColor.a += Time.deltaTime;
            loadText.color = newColor;

            if(fadeImage.color.a > 1.0f) {
                break;
            }

            yield return null;
        }

        SceneManager.LoadScene("TitleScene");
    }
}
