using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : MonoBehaviour
{
    PlayerSoundControl psc;
    PlayerAnimeControl pac;

    public GameObject fallLine;
    public Transform startPos;

    [SerializeField] Text hpText;
    [SerializeField] int hpMax;
    public int hp;
    public HpHeart playerHeart;

    public Image respawnFadeImage;
    public Text respawnText;

    void Start()
    {
        psc = GetComponent<PlayerSoundControl>();
        pac = GetComponent<PlayerAnimeControl>();

        hp = hpMax;
        hpText.text = hp.ToString();
        playerHeart.SetHeartImage(hp);

        Color newColor = respawnFadeImage.color;
        newColor.a = 0.0f;
        respawnFadeImage.color = newColor;
        
        newColor = respawnText.color;
        newColor.a = 0.0f;
        respawnText.color = newColor;
    }

	private void OnTriggerEnter(Collider other) {
        if(other.gameObject == fallLine) {
            StartCoroutine(Respawn());
        }
	}

    IEnumerator Respawn() {
        //GameManager.instance.isPlaying = false;
        GetComponent<MovePlayer>().canMoving = false;

        for(int i = 0; i < 1000; i++) {
            Color newColor = respawnFadeImage.color;
            newColor.a += Time.deltaTime;
            respawnFadeImage.color = newColor;

            newColor = respawnText.color;
            newColor.a += Time.deltaTime;
            respawnText.color = newColor;

            if(newColor.a >= 1.0f) {
                break;
            }
            yield return null;
        }

        GetComponent<WireActionPlayer>().DivideWire();

        /* プレイヤーを初期位置へ */
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = startPos.position;
        transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(0.5f);

        for(int i = 0; i < 1000; i++) {
            Color newColor = respawnFadeImage.color;
            newColor.a -= Time.deltaTime;
            respawnFadeImage.color = newColor;

            newColor = respawnText.color;
            newColor.a -= Time.deltaTime;
            respawnText.color = newColor;

            if(newColor.a <= 0.0f) {
                break;
            }
            yield return null;
        }
        //GameManager.instance.isPlaying = true;
        GetComponent<MovePlayer>().canMoving = true;
    }

    public void DamagedPlayer() {
        if(GameManager.instance.isPlaying) {
            hp--;
            hpText.text = hp.ToString();
            playerHeart.SetHeartImage(hp);

            psc.PlayDamageSE();

            if(hp <= 0) {
                LosePlayer();
            }
        }
    }

    private void LosePlayer() {
        StartCoroutine(GameManager.instance.GameEnd(0));
    }

    private void WinPlayer() {
        StartCoroutine(GameManager.instance.GameEnd(1));
    }
}
