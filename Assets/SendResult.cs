using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendResult : MonoBehaviour
{
    public static SendResult instance;

    public int resultNum = 0;
    public string subStr;

    public float timerRemain;
    public float whaleHpRate;

	private void Awake() {
        if(instance == null) {
            instance = this;
        }
        DontDestroyOnLoad(this);
	}
	void Start()
    {
        
    }

    void Update()
    {
        
    }
}
