using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSet : MonoBehaviour
{
    [SerializeField] GameObject controllers;

    void Start()
    {
        controllers.SetActive(false);

        // アンドロイドの場合はコントローラーを表示する
        #if UNITY_ANDROID
            controllers.SetActive(true);
        #endif
    }
}
