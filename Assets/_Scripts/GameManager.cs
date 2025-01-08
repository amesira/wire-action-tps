using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool loadStart;

    [Header("フェード処理")]
    public Image fadeImage;
    public Text loadText;
    public float fadeSpeed = 1.5f;

    bool isFadeout;

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
