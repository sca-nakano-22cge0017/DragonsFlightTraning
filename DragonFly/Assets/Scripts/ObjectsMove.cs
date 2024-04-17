using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトの移動
/// </summary>
public class ObjectsMove : MonoBehaviour
{
    MainGameController mainGameController;

    float defaultSpeed = 5;
    float speed = 0;
    float destroyPosX = -10;

    float ratio = 1;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float Speed
    {
        get { return defaultSpeed; }
        set { defaultSpeed = value;}
    }

    /// <summary>
    /// 速度上昇倍率
    /// </summary>
    public float Ratio
    {
        get { return ratio; }
        set { ratio = value; }
    }

    void Start()
    {
        if (GameObject.FindObjectOfType<MainGameController>() is MainGameController mg)
        {
            mainGameController = mg;
        }

        speed = defaultSpeed;
    }

    void FixedUpdate()
    {
        if (mainGameController.state == MainGameController.STATE.PLAY)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime * ratio);
        }

        //特定位置まで来たらオブジェクト削除
        if (destroyPosX > transform.position.x)
        {
            Destroy(gameObject);
        }
    }
}
