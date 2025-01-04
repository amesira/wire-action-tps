using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SwordActionPlayer : MonoBehaviour
{
    public enum PLAYER_ACTION {
        SWORD_ACTION,
        THROW_ACTION,
    }

    PlayerAnimeControl anim;

    public PLAYER_ACTION playerAct;

    [Header("Œ•ƒAƒNƒVƒ‡ƒ“")]
    public BoxCollider swordCol;
    public GameObject traceObject;
    public bool isTrace;
    public float traceSpawn = 0.1f;

    [Header("“Š‚°‚Â‚¯ƒAƒNƒVƒ‡ƒ“")]
    public GameObject throwObject;

    float traceTime;

    float swingTime;

    List<GameObject> traceLine = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<PlayerAnimeControl>();

        playerAct = PLAYER_ACTION.SWORD_ACTION;

        traceObject.SetActive(false);
        swordCol.enabled = false;
    }

    public void CheckInputActionButton() {
        if(Input.GetMouseButtonDown(0)) {
            anim.SetSword();
        }
    }

    public void UpdateSword() {
        if(isTrace) {
            traceTime -= Time.deltaTime;
            if(traceTime < 0.0f) {
                traceTime = traceSpawn;
                /* ‹OÕ‚ð¶¬ */
                GameObject traceCopy = Instantiate(traceObject);

                /* ‹OÕ‚ÌÝ’è */
                traceCopy.SetActive(true);
                traceCopy.transform.position = traceObject.transform.position;
                traceCopy.transform.rotation = traceObject.transform.rotation;
                traceCopy.transform.localScale = traceObject.transform.lossyScale;

                /* ”•bŒã‚É‹OÕ‚ðíœ */
                Destroy(traceCopy, 0.15f);
            }
        }

        swingTime -= Time.deltaTime;
        if(swingTime < 0.0f && swordCol.enabled) {
            swordCol.enabled = false;
        }
    }

    public void SwingSword() {
        swordCol.enabled = true;
        swingTime = 0.5f;
    }

	private void OnTriggerEnter(Collider other) {
        if(other.tag == "ThrowObj") {
            throwObject = other.gameObject;
            playerAct = PLAYER_ACTION.THROW_ACTION;
        }
	}
}
