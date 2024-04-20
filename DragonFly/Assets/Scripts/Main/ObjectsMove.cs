using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトの移動
/// </summary>
public class ObjectsMove : MonoBehaviour
{
    MainGameController mainGameController;

    [SerializeField] float speed = 0;
    /// <summary>
    /// 移動速度
    /// </summary>
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    const float destroyPosX = -10;

    float ratio = 1;
    /// <summary>
    /// 速度上昇倍率
    /// </summary>
    public float Ratio
    {
        get { return ratio; }
        set { ratio = value; }
    }

    [SerializeField] int num = -1; //管理番号
    public int Num { get { return num; } }

    void Start()
    {
        if (GameObject.FindObjectOfType<MainGameController>() is MainGameController mg)
        {
            mainGameController = mg;
        }
    }

    void FixedUpdate()
    {
        if (mainGameController.state == MainGameController.STATE.PLAY)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime * ratio);
        }

        //特定位置まで来たらオブジェクト非表示　オブジェクトプールに返却
        if (destroyPosX > transform.position.x)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
}