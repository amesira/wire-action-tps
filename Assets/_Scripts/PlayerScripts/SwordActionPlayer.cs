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

    float swingTime;

    List<GameObject> traceLine = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<PlayerAnimeControl>();

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

        swingTime -= Time.deltaTime;
        if(swingTime < 0.0f && swordCol.enabled) {
            swordCol.enabled = false;
        }
    }

    public void SwingSword() {
        swordCol.enabled = true;
        swingTime = 0.5f;
    }
}
