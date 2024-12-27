using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SwordActionPlayer : MonoBehaviour
{
    PlayerAnimeControl anim;

    public BoxCollider swordCol;

    [Header("åïÇÃãOê’")]
    public GameObject traceObject;
    public bool isTrace;
    public float traceSpawn = 0.1f;

    float traceTime;

    List<GameObject> traceLine = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<PlayerAnimeControl>();

        traceObject.SetActive(false);
    }

    public void CheckInputActionButton() {
        if(Input.GetMouseButtonDown(0)) {
            anim.SetSword();
        }
    }

    public void UpdateTrace() {
        if(isTrace) {
            traceTime -= Time.deltaTime;
            if(traceTime < 0.0f) {
                /* ãOê’Çê∂ê¨ */
                GameObject traceCopy = Instantiate(traceObject);

                /* ãOê’ÇÃê›íË */
                traceCopy.SetActive(true);
                traceCopy.transform.position = traceObject.transform.position;
                traceCopy.transform.rotation = traceObject.transform.rotation;
                traceCopy.transform.localScale = traceObject.transform.lossyScale;

                /* êîïbå„Ç…ãOê’ÇçÌèú */
                Destroy(traceCopy, 0.15f);
            }
        }
    }

    public void SwingSword() {
        swordCol.enabled = true;
        isTrace = true;
        traceTime = traceSpawn;
    }

    public void PutAwaySword() {
        swordCol.enabled = false;
        isTrace = false;
    }
}
