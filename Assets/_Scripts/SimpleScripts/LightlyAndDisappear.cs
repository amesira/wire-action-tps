using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightlyAndDisappear : MonoBehaviour
{
    MeshRenderer meshRender;
    // Start is called before the first frame update
    void Start()
    {
        meshRender = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Color newColor = meshRender.material.color;
        newColor.a -= Time.deltaTime;
        meshRender.material.color = newColor;

        if(meshRender.material.color.a < 0.01f) {
            Destroy(gameObject);
        }
    }
}
