using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pose : MonoBehaviour
{
    [SerializeField] MainGameController mainGameController;
    [SerializeField] GameObject window;

    void Start()
    {
        window.SetActive(false);
    }

    void Update()
    {
        //スペースでポーズ
        if(Input.GetKeyDown(KeyCode.Space) && !window.activeSelf)
        {
            window.SetActive(true);
            mainGameController.state = MainGameController.STATE.WAIT;
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// ポーズ解除
    /// </summary>
    public void PoseEnd()
    {
        if(window.activeSelf)
        {
            window.SetActive(false);
            mainGameController.state = MainGameController.STATE.PLAY;
            Time.timeScale = 1;
        }
    }
}
