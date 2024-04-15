using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 障害物の移動・消去
/// </summary>
public class ObstacleController : MonoBehaviour
{
    MainGameController mainGameController;

    [SerializeField, Header("次の障害物生成ライン")] float createPosX;

    bool canCreate = true;
    //次の障害物生成のフラグ 特定位置まで来たら次の障害物を生成する

    void Start()
    {
        if (GameObject.FindObjectOfType<MainGameController>() is MainGameController mg)
        {
            mainGameController = mg;
        }
    }

    private void OnEnable()
    {
        canCreate = true;
    }

    void FixedUpdate()
    {
        //特定位置まで来たら一度だけ次の障害物を生成する
        if(createPosX > transform.position.x && canCreate)
        {
            canCreate = false;
            mainGameController.ObstacleCreate();
            mainGameController.ItemCreate(); //一緒にアイテムの生成判定
        }
    }
}
