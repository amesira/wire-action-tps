using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimeControl : MonoBehaviour
{
    Animator anim;

    public float blinkSpawn;

    float time;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        time = blinkSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if(time < 0.0f) {
            anim.SetTrigger("onBlink");
            time = blinkSpawn;
        }
    }
    public void IsRunning(bool _flag) {
        anim.SetBool("isRunning", _flag);
    }
}
