using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// モード変更
/// </summary>
public class ModeChange : MonoBehaviour
{
    MainGameController mainGameController;
    ObjectController objectController;
    BGCrossFade crossFade;

    int modeNum = 0;
    int lastModeNum = 0;
    int loopNum = 0; //3種のモードを何回ループしたか
    int lastLoopNum = 0;

    [SerializeField, Header("各モードの時間")] float modeInterval;
    float nowTimeMode = 0; //経過時間

    void Start()
    {
        if (GetComponent<MainGameController>() is var mgc) mainGameController = mgc;
        if (GetComponent<ObjectController>() is var oc) objectController = oc;
        if (GetComponent<BGCrossFade>() is var cf) crossFade = cf;
    }

    void Update()
    {
        if(mainGameController.state == MainGameController.STATE.PLAY)
        {
            Change();
        }
    }

    /// <summary>
    /// モード変更
    /// </summary>
    void Change()
    {
        nowTimeMode += Time.deltaTime;

        if (nowTimeMode >= modeInterval)
        {
            nowTimeMode = 0;

            if (modeNum < MainGameController.MODE.GetNames(typeof(MainGameController.MODE)).Length - 1)
            {
                modeNum++;
            }
            else
            {
                modeNum = 0;
                loopNum++; //ループ回数追加
            }

            mainGameController.mode = (MainGameController.MODE)modeNum;
            
            crossFade.CrossFade(lastModeNum, modeNum);
            lastModeNum = modeNum;
        }

        //モード1周したら
        if (lastLoopNum != loopNum)
        {
            objectController.SpeedUp();

            lastLoopNum = loopNum;
        }
    }
}
