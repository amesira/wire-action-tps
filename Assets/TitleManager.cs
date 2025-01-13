using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public string gameSceneName = "GameScene";
    public GameObject player;

    [Header("フェード処理")]
    public Image fadeImage;
    public Text loadText;
    public float fadeSpeed = 1.5f;

    Rigidbody rb;
    WireActionPlayer wap;

    bool isStart = false;
    void Start() {
        rb = player.GetComponent<Rigidbody>();
        wap = player.GetComponent<WireActionPlayer>();

        Color newColor = fadeImage.color;
        newColor.a = 1.0f;
        fadeImage.color = newColor;

        newColor = loadText.color;
        newColor.a = 1.0f;
        loadText.color = newColor;

        StartCoroutine(Fade());
    }

    IEnumerator Fade() {
        fadeImage.gameObject.SetActive(true);

        for(int i = 0; i < 1000; i++) {
            Color newColor = fadeImage.color;
            newColor.a -= Time.deltaTime * fadeSpeed;
            fadeImage.color = newColor;

            newColor = loadText.color;
            newColor.a -= Time.deltaTime * fadeSpeed;
            loadText.color = newColor;

            if(newColor.a < 0.0f) {
                break;
            }
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }

	private void FixedUpdate() {
        if(isStart) {
            /* 速度設定 */
            if(wap.isLink) {    // ロープにつながっている場合
                rb.velocity = wap.LinkRoap(Vector3.zero);
            }

            /* ワイヤーコントロール */
            wap.WireAnchorControl();

            fadeImage.gameObject.SetActive(true);
            /* フェードイン */
            Color newColor = fadeImage.color;
            newColor.a += Time.deltaTime * fadeSpeed;
            fadeImage.color = newColor;

            newColor = loadText.color;
            newColor.a += Time.deltaTime * fadeSpeed;
            loadText.color = newColor;

            if(fadeImage.color.a > 1.0f) {
                SceneManager.LoadScene(gameSceneName);
            }
        }
	}

	public void OnClickPlay() {
        wap.ShotWire();
        isStart = true;
    }

    public void GameEnd() {
#if UNITY_EDITOR    // unity上で実行した場合
        UnityEditor.EditorApplication.isPlaying = false;
#else               // アプリケーションとして実行した場合
        Application.Quit();
#endif
    }
}
