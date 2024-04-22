using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤー制御
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] MainGameController mainGameController;
    [SerializeField] FeverController feverController;
    [SerializeField] SoundController sound;
    PlayerInput playerInput;

    [SerializeField] float speed;

    //移動できる範囲の上限/下限まで移動しているかどうか
    bool isMax = false, isMin = false;

    [Header("ゲームオーバー演出")]
    [SerializeField, Header("揺れの速度")] float cycleSpeed = 10;
    [SerializeField, Header("揺れ幅")] float amplitude = 0.001f;

    SpriteRenderer sr;
    Collider2D col;

    // 有効化
    private void OnEnable()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();
    }

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
                if(!mainGameController.IsWarp) //ワープ中は動かせない
                {
                    Move();
                }
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

        // 指定キーが押されている間　かつ　移動上限じゃなかったら
        if (playerInput.Player.Up.IsPressed() && !isMax)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }

        if (playerInput.Player.Down.IsPressed() && !isMin)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    void Gameover()
    {
        float x = Mathf.Sin(Time.time * cycleSpeed) * amplitude;
        this.transform.position -= new Vector3(x, speed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //障害物にぶつかったとき　かつ　無敵状態じゃないとき
        if(collision.gameObject.CompareTag("Obstacle") && !mainGameController.IsInvincible && 
            mainGameController.state != MainGameController.STATE.GAMEOVER)
        {
            mainGameController.GameOver();
        }

        //フィーバータイム突入アイテム
        if(collision.gameObject.CompareTag("Ball"))
        {
            feverController.Ball++;
            sound.ItemCatch(); // SE
            Destroy(collision.gameObject); //アイテムを消す
        }

        //ワープホールに触れたとき
        if(collision.gameObject.CompareTag("WarpHole"))
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask; //マスクする
            col.enabled = false; //当たり判定を消す

            mainGameController.Warp();
            sound.Warp(); // SE
        }
    }
}
