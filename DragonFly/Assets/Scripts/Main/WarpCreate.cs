using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ワープアイテム オブジェクトプール管理
/// </summary>
public class WarpCreate : MonoBehaviour
{
    [SerializeField, Header("生成場所")] Transform parent;
    [SerializeField] ObjectsMove warpObjects;

    private ObjectPool<ObjectsMove> pool;

    Vector2 pos;
    public Vector2 PosSet { set { pos = value; } }

    void Start()
    {
        pool = new ObjectPool<ObjectsMove>(
            OnCreatePlloedObject,
            OnGetFromPool,
            OnReleaseToPool,
            OnDestroyPooledObject
            );
    }

    /// <summary>
    /// ゲームオブジェクト生成処理の関数
    /// </summary>
    /// <returns></returns>
    public ObjectsMove OnCreatePlloedObject()
    {
        ObjectsMove gameObject = Instantiate(warpObjects, pos, Quaternion.identity, parent);
        return gameObject;
    }

    /// <summary>
    /// オブジェクトプールからゲームオブジェクトを取得する関数
    /// </summary>
    /// <returns></returns>
    public void OnGetFromPool(ObjectsMove target)
    {
        target.gameObject.SetActive(true);
    }

    /// <summary>
    /// ゲームオブジェクトをオブジェクトプールに返却する関数
    /// </summary>
    /// <returns></returns>
    public void OnReleaseToPool(ObjectsMove target)
    {
        target.gameObject.SetActive(false);
    }

    /// <summary>
    /// ゲームオブジェクトを削除する関数
    /// </summary>
    /// <param name="target"></param>
    public void OnDestroyPooledObject(ObjectsMove target)
    {
        Destroy(target.gameObject);
    }
}