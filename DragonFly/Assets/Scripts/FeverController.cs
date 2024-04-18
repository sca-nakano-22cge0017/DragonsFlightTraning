using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverController : MonoBehaviour
{
    MainGameController mainGameController;
    UIDisp uiDisp;
    Object _object;
    [SerializeField, Header("生成場所")] Transform objectsParent;

    [Header("フィーバー")]
    [SerializeField] Image[] balls;
    int ball = 0; //獲得アイテムの数
    int lateBall;
    [SerializeField, Header("最大個数")] int maxBall = 7;
    public int Ball { get { return ball; } set { ball = value; } }

    [SerializeField] GameObject[] ballsImage;
    [SerializeField, Header("ゲージ")] GameObject guages;
    [SerializeField] Image guageInside;

    [SerializeField, Header("継続時間")] float feverTime;
    float nowTimeFever = 0f; // 経過時間

    [SerializeField, Header("フィーバー時の速度上昇倍率")] float feverRatio;
    float _ratio = 1;
    [SerializeField, Header("風エフェクト")] ParticleSystem windEffect;
    
    void Start()
    {
        if (GetComponent<MainGameController>() is var mgc) mainGameController = mgc;
        if (GetComponent<UIDisp>() is var ud) uiDisp = ud;
        if (GetComponent<Object>() is var obj) _object = obj;

        //フィーバーアイテム個数初期化
        ball = 0;
        lateBall = 0;
        uiDisp.UIDisplay(balls, balls.Length, false);

        //風エフェクト
        windEffect.Stop();
    }

    void Update()
    {
        if (mainGameController.state == MainGameController.STATE.PLAY)
        {
            FeverControll();
        }
    }

    /// <summary>
    /// フィーバーの管理
    /// </summary>
    void FeverControll()
    {
        if (ball >= maxBall) ball = maxBall;

        //前フレームから獲得数に変更があれば表示をし直す
        if (lateBall != ball)
        {
            //アイテムをすべて消す
            uiDisp.UIDisplay(balls, balls.Length, false);

            //現在のアイテム獲得数だけ再表示
            uiDisp.UIDisplay(balls, ball, true);

            lateBall = ball;
        }

        //7個獲得したらフィーバー突入
        if (ball >= maxBall)
        {
            ball = 0; //初期化
            mainGameController.IsFever = true;
            StartCoroutine(FeverTimeCheck());
        }

        //フィーバータイム
        if (mainGameController.IsFever)
        {
            _ratio = feverRatio;
            
            windEffect.Play(); //エフェクト再生

            //ゲージに表示を変える
            for (int i = 0; i < ballsImage.Length; i++)
            {
                ballsImage[i].SetActive(false);
            }
            guages.SetActive(true);

            //ゲージ数値変更
            nowTimeFever -= Time.deltaTime;
            float c = nowTimeFever / feverTime;
            guageInside.fillAmount = c;
        }

        else
        {
            //経過時間を初期化
            nowTimeFever = feverTime;

            _ratio = 1;

            windEffect.Stop(); // エフェクト停止

            //ゲージから表示を戻す
            for (int i = 0; i < ballsImage.Length; i++)
            {
                ballsImage[i].SetActive(true);
            }
            guages.SetActive(false);
            guageInside.fillAmount = 1;
        }

        SpeedChange(_ratio); //速度変更
    }

    IEnumerator FeverTimeCheck()
    {
        yield return new WaitForSecondsRealtime(feverTime);
        mainGameController.IsFever = false; //フィーバー終了
    }

    void SpeedChange(float speed)
    {
        // 子オブジェクトの速度倍率を変更
        foreach (Transform child in objectsParent)
        {
            if(child.GetComponent<ObjectsMove>() is var obj)
            {
                obj.Ratio = _ratio;
            }
        }
    }
}
