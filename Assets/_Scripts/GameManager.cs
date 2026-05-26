using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using unityroom.Api;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    public AudioSource audio;
    public AudioClip winBGM;
    public AudioClip loseBGM;

    public bool loadStart;
    public bool isPlaying;


    [Header("フェード処理")]
    public Image fadeImage;
    public Text loadText;
    public float fadeSpeed = 1.5f;

    [Header("UIグループ")]
    public GameObject startUI;
    public GameObject gameUI;
    public GameObject endUI;
    public Text resultText;
    public Text resultSubText;

    [Header("タイマー")]
    public float timeRimit = 180.0f;
    public Timer timer;
    public Slider timeSlider;

    [Header("ゲームスタート")]
    public RectTransform startPanel;
    public RectTransform startCutinText;
    
    float timerCnt = 0.0f;

    bool isFadeout;
    bool timeup = false;

	private void Awake() {
        if(instance == null) {
            instance = this;
        }
        loadStart = true;
        isPlaying = false;
	}

	void Start()
    {
        if(loadStart) {
            fadeImage.gameObject.SetActive(true);

            /* フェードアウト処理UIの初期化 */
            Color newColor = fadeImage.color;
            newColor.a = 1.0f;
            fadeImage.color = newColor;

            newColor = loadText.color;
            newColor.a = 1.0f;
            loadText.color = newColor;

            isFadeout = true;
        }

        timerCnt = timeRimit;
        //timerText.text = timerCnt.ToString();
        timer.SetTimerText(timerCnt);
        timeup = false;

        timeSlider.value = 1.0f;

        startUI.SetActive(true);
        gameUI.SetActive(false);
        endUI.SetActive(false);

        // スタートカットイン
        startPanel.gameObject.SetActive(false);
        startCutinText.gameObject.SetActive(false);
    }

    private void FixedUpdate() {
        if(isFadeout) {
            /* フェードアウト */
            Color newColor = fadeImage.color;
            newColor.a -= Time.deltaTime * fadeSpeed;
            fadeImage.color = newColor;

            newColor = loadText.color;
            newColor.a -= Time.deltaTime * fadeSpeed;
            loadText.color = newColor;

            if(fadeImage.color.a < 0.0f) {
                fadeImage.gameObject.SetActive(false);
                isFadeout = false;
            }
        }
    }

	void Update()
    {
        if(isPlaying) {
            // タイマー設定
            timerCnt -= Time.deltaTime;
            timer.SetTimerText(timerCnt);

            if(timerCnt < 20.0f) {
                timer.SetTimerColor(Color.red);
                audio.pitch = 2.0f;
            }
            else if(timerCnt < 40.0f) {
                timer.SetTimerColor(Color.yellow);
                audio.pitch = 1.5f;
            }

            if(timerCnt < 0.0f) {
                timer.SetTimerText(0.0f);
                timeup = true;
                StartCoroutine(GameEnd(0));
            }

            timeSlider.value = timerCnt / timeRimit;

            /* Escを押したら */
            if(Input.GetKeyDown(KeyCode.Escape)) {
                /* カーソルを表示 */
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            /* マウスを押したら */
            if(Input.GetMouseButtonDown(0)) {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    public void GameStart() {
        isPlaying = true;
        startUI.SetActive(false);
        gameUI.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(StartCutin(0.6f));
    }

    public IEnumerator StartCutin(float speed) {
        float t = 0.0f;
        float value = 0.0f;
        Vector2 panelSize = startPanel.sizeDelta;

        startPanel.gameObject.SetActive(true);
        startCutinText.gameObject.SetActive(true);

        bool wait = false;

        while(t < 1.0f) {
            t += Time.deltaTime * speed;

            if(t < 0.5f) {
                // EaseOut部分（前半）
                value = 0.5f * Mathf.Pow((t * 2), 1) * (2 - (t * 2));
                panelSize.y = Mathf.Lerp(0.0f, 150.0f, value * 2);
            }
            else {
                // EaseIn部分（後半）
                float nt = (t - 0.5f) * 2;
                value = 0.5f + 0.5f * Mathf.Pow(nt, 2);
                panelSize.y = Mathf.Lerp(150.0f, 0.0f, (value - 0.5f) * 2);
            }

            startPanel.sizeDelta = panelSize;
            startCutinText.anchoredPosition = Vector3.Lerp(new Vector3(720f, 0, 0), new Vector3(-720f, 0, 0), value);

            yield return null;

            if(!wait && t >= 0.5f) {
                wait = true;
                yield return new WaitForSeconds(1.0f);
            }
        }

         startPanel.gameObject.SetActive(false);
        startCutinText.gameObject.SetActive(false);
    }

    public IEnumerator GameEnd(int _result) {
        player.GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(player.GetComponent<PlayerAnimeControl>().PlayEndAnim(_result));
        isPlaying = false;

        startUI.SetActive(false);
        gameUI.SetActive(false);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        audio.Stop();
        audio.pitch = 1.0f;
        yield return new WaitForSeconds(1.5f);

        endUI.SetActive(true);

        if(_result == 0) { // 負けた場合
            resultText.text = "ゲームオーバー";
            resultText.color = Color.red;
            if(timeup) {
                resultSubText.text = "制限時間内にクジラを倒せなかった…";
                SendResult.instance.subStr = "制限時間内にクジラを倒せなかった…";
            }
            else {
                resultSubText.text = "ロボットが壊れてしまった…";
                SendResult.instance.subStr = "ロボットが壊れてしまった…";
            }

            audio.clip = loseBGM;
            audio.Play();
        }
        else { // 勝った場合
            resultText.text = "ゲームクリア！";
            resultText.color = Color.yellow;
            resultSubText.text = "見事クジラを倒した！";
            SendResult.instance.subStr = "見事クジラを倒した！";

            audio.clip = winBGM;
            audio.Play();

            // スコア送信
            UnityroomApiClient.Instance.SendScore(1, timerCnt, ScoreboardWriteMode.HighScoreDesc);
        }
        SendResult.instance.resultNum = _result;
    }

    public void LoadResult() {
        /* フェードアウト処理UIの初期化 */
        Color newColor = fadeImage.color;
        newColor.a = 0.0f;
        fadeImage.color = newColor;

        newColor = loadText.color;
        newColor.a = 0.0f;
        loadText.color = newColor;

        /* データの受け渡し */
        SendResult.instance.timerRemain = timerCnt;
        SendResult.instance.whaleHpRate = WhaleHp.instance.partsCnt / WhaleHp.instance.partsMax;

        StartCoroutine(GoResult());
    }

    IEnumerator GoResult() {
        fadeImage.gameObject.SetActive(true);
        for(int i = 0; i < 1000; i++) {
            /* フェードアウト */
            Color newColor = fadeImage.color;
            newColor.a += Time.deltaTime * fadeSpeed;
            fadeImage.color = newColor;

            newColor = loadText.color;
            newColor.a += Time.deltaTime * fadeSpeed;
            loadText.color = newColor;

            if(fadeImage.color.a > 1.0f) {
                break;
            }

            yield return null;
        }
        SceneManager.LoadScene("ResultScene");
    }
}
