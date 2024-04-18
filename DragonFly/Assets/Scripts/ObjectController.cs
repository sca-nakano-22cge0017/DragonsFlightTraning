using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! ToDo オブジェクトプールの実装

/// <summary>
/// オブジェクトの生成・速度変更
/// </summary>
public class ObjectController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] MainGameController mainGameController;

    [SerializeField, Header("生成場所")] GameObject parent;

    [Header("障害物生成")]
    [SerializeField, Header("生成位置")] Vector3 createPos = new Vector3(10, 0, 0);
    [SerializeField, Header("障害物")] GameObject[] obstacle;
    int lastObj = 3; //直近に生成した障害物

    [Header("アイテム生成")]
    [SerializeField, Header("アイテム生成位置　X")] int itemPosX;
    [SerializeField, Header("アイテム生成位置　Y")] int[] itemPosY;
    [SerializeField, Header("ワープホールのPrefab")] GameObject warpHole;
    [SerializeField, Header("フィーバーアイテムのPrefab")] GameObject feverItem;
    [SerializeField, Header("ワープホール　生成確率")] float warpProb;
    float _warpProb = 0; //計算用
    [SerializeField, Header("フィーバーアイテム　生成確率")] float feverProb;
    float _feverProb = 0; //計算用

    [Header("ワープ")]
    [SerializeField, Header("ワープ出口")] GameObject warpExit;

    [SerializeField, Header("初期速度")] float objSpeed;
    [SerializeField, Header("移動速度　上昇量")] float addSpeed;
    [SerializeField, Header("移動速度　最大値")] float maxSpeed;
    float _addSpeed; //実際に計算に使う値

    void Awake()
    {
        //生成確率初期化
        _warpProb = warpProb;
        _feverProb = feverProb;

        //最初の障害物を生成
        ObstacleCreate();
    }

    void Update()
    {
        if (mainGameController.state == MainGameController.STATE.PLAY)
        {
            CreateProbability();
        }
    }

    /// <summary>
    /// 障害物生成
    /// </summary>
    public void ObstacleCreate()
    {
        int num = 0;

        //モードによって生成物を変更
        switch (mainGameController.mode)
        {
            case MainGameController.MODE.DAY:
                num = Random.Range(0, 3); //2マス空いてるオブジェクトのみ
                break;

            case MainGameController.MODE.EVENIG:
                switch (lastObj) //直近のオブジェクトと最大上下1マスずれる
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

            case MainGameController.MODE.NIGHT:
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

        var obj = Instantiate(obstacle[num], createPos, Quaternion.identity, parent.transform);

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
        if (mainGameController.IsFever) { _feverProb = 0; _warpProb = 0; }
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
        if (num <= _warpProb)
        {
            obj = Instantiate(warpHole, pos, Quaternion.identity);
        }
        //フィーバーアイテム生成
        else if (num <= _warpProb + _feverProb)
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
    /// ワープの出口生成
    /// </summary>
    public void WarpExitCreate()
    {
        Vector3 pPos = player.GetComponent<Transform>().transform.position;
        Vector3 bPos = new Vector3(2, 0, 0); //ボール生成位置
        var obj = Instantiate(warpExit, pPos + bPos, Quaternion.identity, parent.transform);
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
            if (isAdd)
            {
                om.Speed += speed; //加算
            }
            else om.Speed = speed; //代入
        }
    }

    /// <summary>
    /// オブジェクト 移動速度上昇
    /// </summary>
    public void SpeedUp()
    {
        //障害物の移動速度を上げる
        if (maxSpeed > _addSpeed + objSpeed) _addSpeed += addSpeed;
    }
}
