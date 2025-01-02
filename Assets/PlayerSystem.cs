using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public GameObject fallLine;
    public Transform startPos;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other) {
        if(other.gameObject == fallLine) {
            transform.position = startPos.position;
        }
	}
}
