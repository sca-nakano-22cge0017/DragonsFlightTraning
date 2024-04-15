using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainGameController : MonoBehaviour
{
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
    [SerializeField, Header("回復アイテム　生成確率")] float healProb;
    [SerializeField, Header("ワープホール　生成確率")] float warpProb;
    [SerializeField, Header("ワープ時の移動量")] float warpDis;
    List<GameObject> itemList = new List<GameObject>();

    [SerializeField, Header("障害物/アイテム初期速度")] float objSpeed;
    [SerializeField, Header("障害物/アイテム移動速度 上昇量")] float addSpeed;
    float _addSpeed; //実際に計算に使う値

    [Header("HP")]
    [SerializeField] Image[] heart;
    [SerializeField, Header("初期HP")] int defaultHp;
    int hp; //HP
    int lateHp; //前フレームのHP
    public int HP { get { return hp;}  set { hp = value; } }

    //飛行距離
    [SerializeField] Text distance;
    float dis;

    //ゲームオーバー
    [SerializeField, Header("リザルトへ遷移するまでの時間")] float toResultWait;
    bool isGameover = false;

    void Start()
    {
        //HP初期化
        hp = defaultHp;
        lateHp = hp;
        HpDisplay(heart, hp, true);

        //最初の障害物を生成
        ObstacleCreate();
    }

    void Update()
    {
        switch (state)
        {
            case STATE.WAIT:
                break;

            case STATE.PLAY:
                HpControll();
                FlyDis();

                //障害物の移動速度を上げる
                _addSpeed += addSpeed * Time.deltaTime;
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
        if (num <= healProb)
        {
            obj = Instantiate(healItem, pos, Quaternion.identity);
        }
        //ワープホール生成
        else if(num <= healProb + warpProb)
        {
            obj = Instantiate(warpHole, pos, Quaternion.identity);
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
            HpDisplay(heart, heart.Length, false);

            //現在のHPの数だけ再表示
            HpDisplay(heart, hp, true);

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
    /// HPイラストの表示・非表示
    /// </summary>
    /// <param name="image">対象のイラスト</param>
    /// <param name="num">表示・非表示にする数</param>
    /// <param name="isDisp">trueのとき表示、falseのとき非表示</param>
    void HpDisplay(Image[] image, int num, bool isDisp)
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
        dis += Time.deltaTime * 10 + _addSpeed / 100;
        distance.text = dis.ToString("f0") + "m";
    }

    /// <summary>
    /// ワープホールに触れたときの処理
    /// </summary>
    public void Warp()
    {
        dis += warpDis;
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(toResultWait);

        //飛行距離保存
        PlayerPrefs.SetFloat("Distance", dis);

        //シーン遷移
        SceneManager.LoadScene("ResultScene");
    }
}
