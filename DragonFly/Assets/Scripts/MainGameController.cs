using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
    [SerializeField] GameObject player;

    public enum STATE { WAIT = 0, PLAY, GAMEOVER, }
    public STATE state = 0;

    [Header("障害物生成")]
    [SerializeField, Header("障害物を生成するときの親オブジェクト")] GameObject obstacles;
    [SerializeField, Header("障害物生成位置")] Vector3 createPos = new Vector3(10, 0, 0);
    [SerializeField, Header("障害物のPrefab")] GameObject[] obstacle;

    [Header("アイテム生成")]
    [SerializeField, Header("アイテムを生成するときの親オブジェクト")] GameObject items;
    [SerializeField, Header("アイテム生成位置　X")] int itemPosX;
    [SerializeField, Header("アイテム生成位置　Y")] int[] itemPosY;
    [SerializeField, Header("回復アイテムのPrefab")] GameObject healItem;
    [SerializeField, Header("ワープホールのPrefab")] GameObject warpHole;
    [SerializeField, Header("フィーバーアイテムのPrefab")] GameObject feverItem;
    [SerializeField, Header("回復アイテム　生成確率")] float healProb;
    float _healProb = 0; //計算用
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

    [Header("HP")]
    [SerializeField] Image[] heart;
    [SerializeField, Header("初期HP")] int defaultHp;
    int hp; //HP
    int lateHp; //前フレームのHP
    public int HP { get { return hp;}  set { hp = value; } }

    [Header("フィーバー")]
    [SerializeField] Image[] balls;
    int ball = 0; //獲得アイテムの数
    int lateBall;
    [SerializeField, Header("最大個数")] int maxBall = 7;
    public int Ball { get { return ball; } set { ball = value; } }

    [SerializeField, Header("継続時間")] float feverTime;
    [SerializeField, Header("フィーバー時の速度上昇倍率")] float feverRatio;
    float _ratio = 1;
    [SerializeField, Header("風エフェクト")] ParticleSystem windEffect;

    bool isFever = false;
    public bool IsFever { get { return isFever; } set { isFever = value; } }

    //飛行距離
    [SerializeField] Text distance;
    float dis;

    //ゲームオーバー
    [SerializeField] SceneChange sceneChange;
    [SerializeField, Header("リザルトへ遷移するまでの時間")] float toResultWait;
    bool isGameover = false;

    void Awake()
    {
        //HP初期化
        hp = defaultHp;
        lateHp = hp;
        UIDisplay(heart, hp, true);

        //フィーバーアイテム個数初期化
        ball = 0;
        lateBall = 0;
        UIDisplay(balls, balls.Length, false);

        //最初の障害物を生成
        ObstacleCreate();

        //生成確率初期化
        _healProb = healProb;
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
                HpControll();
                FeverControll();
                FlyDis();
                CreateProbability();

                //障害物の移動速度を上げる
                if(maxSpeed > _addSpeed) _addSpeed += addSpeed * Time.deltaTime;
                break;

            case STATE.GAMEOVER:
                break;
        }
    }

    /// <summary>
    /// 障害物生成
    /// </summary>
    public void ObstacleCreate()
    {
        int num = Random.Range(0, obstacle.Length);
        var obj = Instantiate(obstacle[num], createPos, Quaternion.identity, obstacles.transform);

        if (obj)
        {
            SpeedChange(obj, objSpeed, false);
            SpeedChange(obj, _addSpeed, true);
        }
    }

    /// <summary>
    /// アイテム生成確率の調整
    /// </summary>
    void CreateProbability()
    {
        //HPが最大のときは回復アイテムが生成されないようにする 他アイテムの生成確率を上げる
        if (hp >= defaultHp)
        {
            _healProb = 0;
            _warpProb += _healProb / 4;
            _feverProb += _healProb / 2;
        }
        else if(!isFever)
        {
            //初期値に戻す
            _healProb = healProb;
            _warpProb = warpProb;
            _feverProb = feverProb;
        }

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

        //回復アイテム生成
        if (num <= _healProb)
        {
            obj = Instantiate(healItem, pos, Quaternion.identity);
        }
        //ワープホール生成
        else if(num <= _healProb + _warpProb)
        {
            obj = Instantiate(warpHole, pos, Quaternion.identity);
        }
        //フィーバーアイテム生成
        else if(num <= _healProb + _warpProb + _feverProb)
        {
            obj = Instantiate(feverItem, pos, Quaternion.identity);
        }

        if (obj)
        {
            SpeedChange(obj, objSpeed, false);
            SpeedChange(obj, _addSpeed, true);
        }
    }

    /// <summary>
        /// 障害物/アイテムの速度変更
        /// </summary>
        /// <param name="obj">速度を変えるオブジェクト</param>
        /// <param name="speed">加算/代入速度</param>
        /// <param name="isAdd">加算かどうか trueで加算 falseで代入</param>
    void SpeedChange(GameObject obj, float speed, bool isAdd)
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

    /// <summary>
    /// HP管理
    /// </summary>
    void HpControll()
    {
        if(hp >= defaultHp) hp = defaultHp;

        //前フレームからHPに変更があれば表示をし直す
        if (lateHp != hp)
        {
            //ハートをすべて消す
            UIDisplay(heart, heart.Length, false);

            //現在のHPの数だけ再表示
            UIDisplay(heart, hp, true);

            lateHp = hp;
        }

        //hpが0以下になったらゲームオーバー
        if(hp <= 0)
        {
            state = STATE.GAMEOVER;
            if(!isGameover)
            {
                isGameover = true;
                StartCoroutine(GameOver());
            }
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
        }
        else
        {
            _ratio = 1;
            Time.timeScale = 1;
            windEffect.Stop();
        }
    }

    IEnumerator FeverTimeCheck()
    {
        yield return new WaitForSecondsRealtime(feverTime);
        isFever = false;
    }

    /// <summary>
    /// HPイラストの表示・非表示
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
        var obj = Instantiate(warpExit, pPos + new Vector3(2, 0, 0), Quaternion.identity, obstacles.transform);
        if (obj)
        {
            SpeedChange(obj, objSpeed, false);
            SpeedChange(obj, _addSpeed, true);
        } //移動速度変更

        dis += warpDis;

        yield return new WaitForSeconds(1f);

        isWarp = false; //ワープ終了
    }

    IEnumerator GameOver()
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
