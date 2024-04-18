using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
    [SerializeField] GameObject player;

    public enum STATE { WAIT = 0, PLAY, GAMEOVER, }
    public STATE state = 0;

    public enum MODE { DAY = 0, EVENIG, NIGHT }
    public MODE mode = 0;
    int modeNum = 0;
    int loopNum = 0; //3種のモードを何回ループしたか
    int lastLoopNum = 0;

    [Header("モード")]
    [SerializeField, Header("各モードの時間")] float modeInterval;
    [SerializeField, Header("背景")] Image bg;
    [SerializeField, Header("背景画像")] Sprite[] bgSprite;

    [Header("障害物生成")]
    [SerializeField, Header("障害物を生成するときの親オブジェクト")] GameObject obstacles;
    [SerializeField, Header("障害物生成位置")] Vector3 createPos = new Vector3(10, 0, 0);
    [SerializeField, Header("障害物")] GameObject[] obstacle;
    int lastObj = 3; //直近に生成した障害物

    [Header("アイテム生成")]
    [SerializeField, Header("アイテムを生成するときの親オブジェクト")] GameObject items;
    [SerializeField, Header("アイテム生成位置　X")] int itemPosX;
    [SerializeField, Header("アイテム生成位置　Y")] int[] itemPosY;
    [SerializeField, Header("ワープホールのPrefab")] GameObject warpHole;
    [SerializeField, Header("フィーバーアイテムのPrefab")] GameObject feverItem;
    [SerializeField, Header("ワープホール　生成確率")] float warpProb;
    float _warpProb = 0; //計算用
    [SerializeField, Header("フィーバーアイテム　生成確率")] float feverProb;
    float _feverProb = 0; //計算用

    [SerializeField, Header("初期速度")] float objSpeed;
    [SerializeField, Header("移動速度　上昇量")] float addSpeed;
    [SerializeField, Header("移動速度　最大値")] float maxSpeed;
    float _addSpeed; //実際に計算に使う値

    [Header("ワープ")]
    [SerializeField, Header("ワープ時の移動量")] float warpDis;
    [SerializeField, Header("ワープ時の円形フェード")] CircleFade warpFade;
    bool isWarp =false;
    public bool IsWarp { get { return isWarp; } set { isWarp = value; } }
    [SerializeField, Header("ワープ出口")] GameObject warpExit;

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
    [SerializeField, Header("フィーバー時の速度上昇倍率")] float feverRatio;
    float _ratio = 1;
    [SerializeField, Header("風エフェクト")] ParticleSystem windEffect;
    public float Ratio { get { return feverRatio; } }

    bool isFever = false;
    public bool IsFever { get { return isFever; } set { isFever = value; } }

    //飛行距離
    [SerializeField] Text distance;
    float dis;

    //ゲームオーバー
    [SerializeField] SceneChange sceneChange;
    [SerializeField, Header("リザルトへ遷移するまでの時間")] float toResultWait;

    void Awake()
    {
        //フィーバーアイテム個数初期化
        ball = 0;
        lateBall = 0;
        UIDisplay(balls, balls.Length, false);

        //最初の障害物を生成
        ObstacleCreate();

        //生成確率初期化
        _warpProb = warpProb;
        _feverProb = feverProb;

        //フェードイン
        sceneChange.FadeIn();
        StartCoroutine(FadeEndCheck());

        //風エフェクト
        windEffect.Stop();
    }

    void Update()
    {
        switch (state)
        {
            case STATE.WAIT:
                break;

            case STATE.PLAY:
                FeverControll();
                FlyDis();
                CreateProbability();
                ModeChange();
                break;

            case STATE.GAMEOVER:
                break;
        }
    }

    float nowTimeMode = 0; //経過時間
    /// <summary>
    /// モード変更
    /// </summary>
    void ModeChange()
    {
        nowTimeMode += Time.deltaTime;

        if(nowTimeMode >= modeInterval)
        {
            nowTimeMode = 0;
            
            if(modeNum < MODE.GetNames(typeof(MODE)).Length - 1)
            {
                modeNum++;
            }
            else
            {
                modeNum = 0;
                loopNum++; //ループ回数追加
            }
        }

        mode = (MODE) modeNum;
        bg.sprite = bgSprite[modeNum];

        //モード1周したら
        if(lastLoopNum != loopNum)
        {
            //障害物の移動速度を上げる
            if (maxSpeed > _addSpeed + objSpeed) _addSpeed += addSpeed;

            lastLoopNum = loopNum;
        }
    }

    /// <summary>
    /// 障害物生成
    /// </summary>
    public void ObstacleCreate()
    {
        int num = 0;

        //モードによって生成物を変更
        switch (mode)
        {
            case MODE.DAY:
                num = Random.Range(0, 3); //2マス空いてるオブジェクトのみ
                break;

            case MODE.EVENIG:
                switch(lastObj) //直近のオブジェクトと最大上下1マスずれる
                {
                    case 3:
                        num = lastObj + Random.Range(0, 2);
                        break;
                    case 4:
                    case 5:
                        num = lastObj + Random.Range(-1, 2);
                        break;
                    case 6:
                        num = lastObj + Random.Range(-1, 1);
                        break;
                }
                lastObj = num; //生成したオブジェクトの番号を保持
                break;

            case MODE.NIGHT:
                switch (lastObj) //直近のオブジェクトと上下に最小2マス、最大3マスずれる
                {
                    case 3:
                        num = lastObj + Random.Range(2, 4);
                        break;
                    case 4:
                        num = lastObj + 2;
                        break;
                    case 5:
                        num = lastObj - 2;
                        break;
                    case 6:
                        num = lastObj + Random.Range(-3, -1);
                        break;
                }
                lastObj = num; //生成したオブジェクトの番号を保持
                break;

            default:
                //完全ランダム
                num = Random.Range(0, obstacle.Length);
                break;
        }
        
        var obj = Instantiate(obstacle[num], createPos, Quaternion.identity, obstacles.transform);

        if (obj)
        {
            ObjSpeedChange(obj, objSpeed, false);
            ObjSpeedChange(obj, _addSpeed, true);
        }
    }

    /// <summary>
    /// アイテム生成確率の調整
    /// </summary>
    void CreateProbability()
    {
        //フィーバー中はフィーバーアイテム・ワープアイテムが生成されないようにする
        if (isFever) { _feverProb = 0; _warpProb = 0; }
        else { _warpProb = warpProb; _feverProb = feverProb; }
    }

    /// <summary>
    /// アイテムの生成
    /// </summary>
    public void ItemCreate()
    {
        //一定確率で生成する
        int num = Random.Range(1, 101);

        //生成位置をランダムに算出
        int n = Random.Range(0, itemPosY.Length);
        Vector3 pos = new Vector3(itemPosX, itemPosY[n], 0);
        GameObject obj = null;

        //ワープホール生成
        if(num <= _warpProb)
        {
            obj = Instantiate(warpHole, pos, Quaternion.identity);
        }
        //フィーバーアイテム生成
        else if(num <= _warpProb + _feverProb)
        {
            obj = Instantiate(feverItem, pos, Quaternion.identity);
        }

        if (obj)
        {
            ObjSpeedChange(obj, objSpeed, false);
            ObjSpeedChange(obj, _addSpeed, true);
        }
    }

    /// <summary>
        /// 障害物/アイテムの速度変更
        /// </summary>
        /// <param name="obj">速度を変えるオブジェクト</param>
        /// <param name="speed">加算/代入速度</param>
        /// <param name="isAdd">加算かどうか trueで加算 falseで代入</param>
    void ObjSpeedChange(GameObject obj, float speed, bool isAdd)
    {
        if (obj.GetComponent<ObjectsMove>() is ObjectsMove om)
        {
            if(isAdd)
            {
                om.Speed += speed; //加算
            }
            else om.Speed = speed; //代入
        }
    }

    float nowTimeFever = 0f; // 経過時間
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
            UIDisplay(balls, balls.Length, false);

            //現在のアイテム獲得数だけ再表示
            UIDisplay(balls, ball, true);

            lateBall = ball;
        }

        //7個獲得したらフィーバー突入
        if (ball >= maxBall)
        {
            ball = 0; //初期化
            isFever = true;
            StartCoroutine(FeverTimeCheck());
        }

        //フィーバータイム
        if(isFever)
        {
            _ratio = feverRatio;
            Time.timeScale = feverRatio; //速度上昇
            windEffect.Play(); //エフェクト再生

            for(int i = 0; i < ballsImage.Length; i++)
            {
                ballsImage[i].SetActive(false);
            }

            guages.SetActive(true);

            //ゲージ数値変更
            nowTimeFever -= Time.deltaTime / feverRatio;
            float c = nowTimeFever / feverTime;
            guageInside.fillAmount = c;
        }
        else
        {
            nowTimeFever = feverTime;

            _ratio = 1;
            Time.timeScale = 1;
            windEffect.Stop();

            for (int i = 0; i < ballsImage.Length; i++)
            {
                ballsImage[i].SetActive(true);
            }

            guages.SetActive(false);
            guageInside.fillAmount = 1;
        }
    }

    IEnumerator FeverTimeCheck()
    {
        yield return new WaitForSecondsRealtime(feverTime);
        isFever = false;
    }

    /// <summary>
    /// UIイラストの表示・非表示
    /// </summary>
    /// <param name="image">対象のイラスト</param>
    /// <param name="num">表示・非表示にする数</param>
    /// <param name="isDisp">trueのとき表示、falseのとき非表示</param>
    void UIDisplay(Image[] image, int num, bool isDisp)
    {
        for (int i = 0; i < num; i++)
        {
            image[i].enabled = isDisp;
        }
    }

    /// <summary>
    /// 飛行距離管理
    /// </summary>
    void FlyDis()
    {
        dis += Time.deltaTime * 10 * _ratio + _addSpeed / 100;
        distance.text = dis.ToString("f0") + "m";
    }

    /// <summary>
    /// ワープホールに触れたときの処理
    /// </summary>
    public void Warp()
    {
        warpFade.IsWarpFade = true; //ワープ演出
        isWarp = true;

        StartCoroutine(WarpEnd());
    }

    /// <summary>
    /// ワープ演出
    /// </summary>
    /// <returns></returns>
    IEnumerator WarpEnd()
    {
        yield return new WaitForSeconds(0.5f);

        SpriteRenderer pSR = player.GetComponent<SpriteRenderer>();
        pSR.enabled = false; //プレイヤー非表示

        yield return new WaitUntil(() => !warpFade.IsWarpFade); //演出が終わったら

        pSR.enabled = true; //プレイヤー表示

        //ワープホールの出口を生成
        Vector3 pPos = player.GetComponent<Transform>().transform.position;
        Vector3 bPos = new Vector3(2, 0, 0); //ボール生成位置
        var obj = Instantiate(warpExit, pPos + bPos, Quaternion.identity, obstacles.transform);
        if (obj)
        {
            ObjSpeedChange(obj, objSpeed, false);
            ObjSpeedChange(obj, _addSpeed, true);
        } //移動速度変更

        dis += warpDis;

        yield return new WaitForSeconds(0.5f);

        isWarp = false; //ワープ終了
    }

    public void GameOver()
    {
        state = STATE.GAMEOVER;
        StartCoroutine(GameEnd());
    }

    IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(toResultWait);

        //飛行距離保存
        PlayerPrefs.SetFloat("Distance", dis);

        //シーン遷移
        sceneChange.ToResult();
    }

    /// <summary>
    /// フェードインが完了したかチェックする
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeEndCheck()
    {
        yield return new WaitUntil(() => sceneChange.IsFadeInEnd);
        state = STATE.PLAY;
    }
}
