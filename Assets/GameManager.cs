using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        /* カーソルを非表示に */
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        /* Escを押したら */
        if(Input.GetKeyDown(KeyCode.Escape)) {
            /* カーソルを表示 */
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        /* マウスを押したら */
        if(Input.GetMouseButtonDown(0)) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
