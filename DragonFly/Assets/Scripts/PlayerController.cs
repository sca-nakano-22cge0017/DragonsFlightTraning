using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー制御
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] MainGameController mainGameController;

    [SerializeField] float speed;

    //移動できる範囲の上限/下限まで移動しているかどうか
    bool isMax = false, isMin = false;

    //ダメージ、無敵
    bool isDamage = false, isInvincible = false;

    [Header("ゲームオーバー演出")]
    [SerializeField, Header("揺れの速度")] float cycleSpeed = 10;
    [SerializeField, Header("揺れ幅")] float amplitude = 0.001f;

    SpriteRenderer sr;
    Collider2D col;

    void Start()
    {
        if(GetComponent<SpriteRenderer>() is var s)
        {
            sr = s;
        }

        if(GetComponent<Collider2D>() is var c)
        {
            col = c;
        }
    }

    void Update()
    {
        switch(mainGameController.state)
        {
            case MainGameController.STATE.WAIT:
                break;

            case MainGameController.STATE.PLAY:
                Move();
                Damage();
                break;

            case MainGameController.STATE.GAMEOVER:
                Gameover();
                break;
        }

        //ワープ中じゃないとき
        if(!mainGameController.IsWarp)
        {
            col.enabled = true; //当たり判定
            sr.maskInteraction = SpriteMaskInteraction.None; //マスク解除
        }
    }

    /// <summary>
    /// プレイヤー移動
    /// </summary>
    void Move()
    {
        if(transform.position.y >= 2) isMax = true;
        else isMax = false;

        if(transform.position.y <= -4) isMin = true;
        else isMin = false;

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !isMax)
        {
            transform.Translate(Vector3.up * speed);
        }

        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !isMin)
        {
            transform.Translate(Vector3.down * speed);
        }
    }

    //ダメージ処理
    void Damage()
    {
        if(isDamage)
        {
            isDamage = false;
            mainGameController.HP--;
            StartCoroutine(DamageDirection());
        }
    }

    Color nomalColor = new Color(1f, 1f, 1f, 1f);
    Color damageColor = new Color(1f, 1f, 1f, 0.7f);
    [SerializeField, Header("被ダメ時の明滅速度")] float damageSpeed;

    //ダメージ演出
    IEnumerator DamageDirection()
    {
        //点滅
        for (int i = 0; i < 3; i++)
        {
            gameObject.GetComponent<SpriteRenderer>().color = damageColor;
            yield return new WaitForSeconds(damageSpeed);
            gameObject.GetComponent<SpriteRenderer>().color = nomalColor;
            yield return new WaitForSeconds(damageSpeed);
        }

        //無敵終了
        isInvincible = false;
    }

    void Gameover()
    {
        float x = Mathf.Sin(Time.time * cycleSpeed) * amplitude;
        this.transform.position -= new Vector3(x, speed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //障害物にぶつかったとき　かつ　無敵状態じゃないとき
        if(collision.gameObject.CompareTag("Obstacle") && !isInvincible)
        {
            isDamage = true;
            isInvincible = true;
        }

        //回復アイテムに触れたとき
        if(collision.gameObject.CompareTag("Heal"))
        {
            mainGameController.HP++;
        }

        if(collision.gameObject.CompareTag("Ball"))
        {
            mainGameController.Ball++;
        }

        //ワープホールに触れたとき
        if(collision.gameObject.CompareTag("WarpHole"))
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask; //マスクする
            col.enabled = false; //当たり判定を消す
            mainGameController.Warp();
        }

        //アイテムに触れた時、アイテムを消す
        if (collision.gameObject.CompareTag("Heal") || collision.gameObject.CompareTag("Ball"))
        {
            Destroy(collision.gameObject);
        }
    }
}
