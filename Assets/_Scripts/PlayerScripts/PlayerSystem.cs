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

    void Start()
    {
        psc = GetComponent<PlayerSoundControl>();
        pac = GetComponent<PlayerAnimeControl>();

        hp = hpMax;
        hpText.text = hp.ToString();
        playerHeart.SetHeartImage(hp);
    }

	private void OnTriggerEnter(Collider other) {
        if(other.gameObject == fallLine) {
            transform.position = startPos.position;
        }
	}

    public void DamagedPlayer() {
        hp--;
        hpText.text = hp.ToString();
        playerHeart.SetHeartImage(hp);

        psc.PlayDamageSE();

        if(hp <= 0) {
            LosePlayer();
        }
    }

    private void LosePlayer() {
        StartCoroutine(pac.PlayEndAnim(0));
    }

    private void WinPlayer() {

    }
}
