using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public float fadeSpeed;

    public Image fadeImage;
    public Text loadText;

    public Text resultText;
    public Text resultSubText;

    public Text remainTime;
    public Text whaleHpRate;

    [SerializeField] private LoadingUiManager loadingUiManager;

    // Start is called before the first frame update
    void Start()
    {
       // StartCoroutine(FadeOut());
       loadingUiManager.Close();

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

    public void BackTitle() {
        loadingUiManager.Open();
        StartCoroutine(LoadTitle(2.0f));
    }

    // n秒後にタイトルへ
    IEnumerator LoadTitle(float delay) {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("TitleScene");
    }

}
