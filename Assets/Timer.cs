using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public List<Text> timerTexts;
    public float upperDigit = 100.0f;

    Text[] texts;
    void Start()
    {
        for(int i = 0; i < timerTexts.Count; i++) {
            timerTexts[i].text = "0";
        }
        texts = GetComponentsInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTimerText(float _time) {
        float f = _time;
        float div = upperDigit;
        for(int i = 0; i < timerTexts.Count; i++) {
            int n = (int)(f / div);
            timerTexts[i].text = n.ToString();

            f %= div;
            div /= 10.0f;
        }
    }

    public void SetTimerColor(Color _color) {
        for(int i = 0; i < texts.Length; i++) {
            texts[i].color = _color;
        }
    }
}
