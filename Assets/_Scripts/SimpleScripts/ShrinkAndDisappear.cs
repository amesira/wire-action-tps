using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAndDisappear : MonoBehaviour
{
    bool shrinking = false;

    float waitTime = 0.0f;
    float lerp = 0.0f;
    Vector3 startScale;
    float shrinkSpeed = 1.0f;

    void FixedUpdate()
    {
        if(shrinking) {
            waitTime -= Time.deltaTime;

            /* ˆê’èŠÔŒo‰ß‚µ‚½‚çûkˆ—‚ğn‚ß‚é */
            if(waitTime < 0.0f) {
                lerp += Time.deltaTime * shrinkSpeed;
                transform.localScale = Vector3.Lerp(startScale, new Vector3(0.01f, 0.01f, 0.01f), lerp);
                if(lerp >= 1.0f) {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public void StartShrinking(float _wait,float _shrinkSpeed) {
        shrinking = true;
        waitTime = _wait;
        lerp = 0.0f;
        shrinkSpeed = _shrinkSpeed;
        startScale = this.transform.localScale;
    }
}
