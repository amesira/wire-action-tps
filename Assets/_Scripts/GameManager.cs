using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool loadStart;
    public bool isPlaying;

    [Header("フェード処理")]
    public Image fadeImage;
    public Text loadText;
    public float fadeSpeed = 1.5f;

    [Header("UIグループ")]
    public GameObject startUI;
    public GameObject gameUI;

    [Header("タイマー")]
    public float timeRimit = 180.0f;
    public Timer timer;
    public Text timerText;
    
    float timerCnt = 0.0f;

    bool isFadeout;

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
        timerText.text = timerCnt.ToString();

        startUI.SetActive(true);
        gameUI.SetActive(false);
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
                isFadeout = false;
            }
        }
    }

	void Update()
    {
        if(isPlaying) {
            timerCnt -= Time.deltaTime;
            timer.SetTimerText(timerCnt);

            //string timerStr = timerCnt.ToString("f3");
            //timerText.text = timerStr;

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
    }
}
