using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 障害物の移動・消去
/// </summary>
public class ObstacleController : MonoBehaviour
{
    MainGameController mainGameController;

    [SerializeField, Header("障害物移動速度")] float obstacleSpeed;
    [SerializeField, Header("次の障害物生成ライン")] float createPosX;
    [SerializeField, Header("消去位置")] float destroyPosX;

    bool canCreate = true;
    //次の障害物生成のフラグ 特定位置まで来たら次の障害物を生成する

    public float ObstacleSpeed
    {
        get { return obstacleSpeed; }
        set { obstacleSpeed = value; }
    }

    void Start()
    {
        mainGameController = GameObject.FindObjectOfType<MainGameController>();
    }

    void FixedUpdate()
    {
        if (mainGameController.state == MainGameController.STATE.PLAY)
        {
            transform.Translate(Vector3.left * obstacleSpeed * Time.deltaTime);
        }

        //特定位置まで来たら一度だけ次の障害物を生成する
        if(createPosX > transform.position.x && canCreate)
        {
            canCreate = false;
            mainGameController.ObstacleCreate();
        }

        //特定位置まで来たらオブジェクト削除
        if(destroyPosX > transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
