using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] SoundController sound;
    ObjectController objectController;
    FeverController feverController;
    UIDisp uiDisp;

    public enum STATE { WAIT = 0, PLAY, GAMEOVER, }
    public STATE state = 0;

    public enum MODE { DAY = 0, EVENIG, NIGHT }
    public MODE mode = 0;

    [Header("ワープ")]
    [SerializeField, Header("ワープ時の移動量")] float warpDis;
    [SerializeField, Header("ワープ時の円形フェード")] CircleFade warpFade;
    bool isWarp =false;
    public bool IsWarp { get { return isWarp; } set { isWarp = value; } }

    bool isFever = false;
    public bool IsFever { get { return isFever; } set { isFever = value; } }

    bool isInvincible = false;
    public bool IsInvincible { get { return isInvincible; } set { isInvincible = value; } }

    //飛行距離
    [SerializeField] Text distance;
    float dis;

    //ゲームオーバー
    [SerializeField] SceneChange sceneChange;
    [SerializeField, Header("リザルトへ遷移するまでの時間")] float toResultWait;

    [SerializeField] DataSaver dataSaver;

    private void Start()
    {
        ScriptsSet();

        //フェードイン
        sceneChange.FadeIn();
        StartCoroutine(FadeEndCheck());
    }

    /// <summary>
    /// 他スクリプトの取得・確認
    /// </summary>
    void ScriptsSet()
    {
        if (GetComponent<ObjectController>() is var oc)
        {
            objectController = oc;
        }

        if (GetComponent<UIDisp>() is var ud)
        {
            uiDisp = ud;
        }

        if (GetComponent<FeverController>() is var fc)
        {
            feverController = fc;
        }
    }

    void Update()
    {
        switch (state)
        {
            case STATE.WAIT:
                break;

            case STATE.PLAY:
                FlyDis();
                break;

            case STATE.GAMEOVER:
                break;
        }
    }

    /// <summary>
    /// 飛行距離管理
    /// </summary>
    void FlyDis()
    {
        dis += Time.deltaTime * feverController.Fever * objectController.Speed * 2;
        
        //UIの表示・非表示は一つにまとめる
        uiDisp.TextPutIn(distance, Mathf.Floor(dis).ToString() + "m");
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

    const float spriteFalseTime = 0.3f;
    const float warpEndTime = 0.5f;

    /// <summary>
    /// ワープ演出
    /// </summary>
    /// <returns></returns>
    IEnumerator WarpEnd()
    {
        yield return new WaitForSeconds(spriteFalseTime);

        SpriteRenderer pSR = player.GetComponent<SpriteRenderer>();
        pSR.enabled = false; //プレイヤー非表示

        objectController.AllRelease(); // オブジェクト全返却

        yield return new WaitUntil(() => !warpFade.IsWarpFade); //演出が終わったら

        pSR.enabled = true; //プレイヤー表示

        //距離を足す
        dis += warpDis;

        //ワープホールの出口を生成
        objectController.WarpExitCreate();

        yield return new WaitForSeconds(warpEndTime);

        objectController.ObstacleCreate(); // 再生成

        isWarp = false; //ワープ終了
    }

    public void GameOver()
    {
        state = STATE.GAMEOVER;
        
        if(GameObject.FindObjectOfType<BGM>().GetComponent<BGM>() is var bgm)
        {
            bgm.BGMStop(); // BGM停止
        }

        sound.GameOver(); // SE

        StartCoroutine(GameEnd());
    }

    IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(toResultWait);

        //飛行距離保存
        dataSaver.saveLatestData(dis);

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
