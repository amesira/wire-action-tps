using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public string gameSceneName = "GameScene";
    public GameObject player;

    [Header("Fade Settings")]
    public Image fadeImage;
    public Text loadText;
    public float fadeSpeed = 1.5f;

    [SerializeField] private LoadingUiManager loadingUiManager;

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
            // ロープを発射してプレイヤーが出動
            if(wap.isLink) {
                rb.velocity = wap.LinkRoap(Vector3.zero);
            }
            wap.WireAnchorControl();

            // fadeImage.gameObject.SetActive(true);
            // Color newColor = fadeImage.color;
            // newColor.a += Time.deltaTime * fadeSpeed;
            // fadeImage.color = newColor;

            // newColor = loadText.color;
            // newColor.a += Time.deltaTime * fadeSpeed;
            // loadText.color = newColor;

            // if(fadeImage.color.a > 1.0f) {
            //     SceneManager.LoadScene(gameSceneName);
            // }
        }
	}

	public void OnClickPlay() {
        wap.ShotWire();
        isStart = true;
        loadingUiManager.Open();

        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene() {
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(gameSceneName);
    }

    public void GameEnd() {
#if UNITY_EDITOR    // エディタ上で実行している場合は、再生モードを停止
        UnityEditor.EditorApplication.isPlaying = false;
#else               // 本番ビルドの場合は、アプリケーションを終了
        Application.Quit();
#endif
    }
}
