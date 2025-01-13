using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpHeart : MonoBehaviour
{
    public Color fullColor;
    public Color damageColor;

    public Image[] heartImages;

    void Start()
    {
        for(int i = 0; i < heartImages.Length; i++) {
            heartImages[i].color = fullColor;
        }
    }

    public void SetHeartImage(int _heartCnt) {
        for(int i = 0; i < heartImages.Length; i++) {
            if(i < _heartCnt) {
                heartImages[i].color = fullColor;
            }
            else {
                heartImages[i].color = damageColor;
            }
        }
    }
}
