using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
    [SerializeField] GameObject player;
    ObjectController objectController;
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

    //飛行距離
    [SerializeField] Text distance;
    float dis;

    //ゲームオーバー
    [SerializeField] SceneChange sceneChange;
    [SerializeField, Header("リザルトへ遷移するまでの時間")] float toResultWait;

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
        dis += Time.deltaTime * 10; //!　オブジェクト移動速度に応じて数値を足す
        
        //UIの表示・非表示は一つにまとめる
        uiDisp.TextPutIn(distance, dis.ToString("f0") + "m");
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

    /// <summary>
    /// ワープ演出
    /// </summary>
    /// <returns></returns>
    IEnumerator WarpEnd()
    {
        yield return new WaitForSeconds(0.5f);

        SpriteRenderer pSR = player.GetComponent<SpriteRenderer>();
        pSR.enabled = false; //プレイヤー非表示

        yield return new WaitUntil(() => !warpFade.IsWarpFade); //演出が終わったら

        pSR.enabled = true; //プレイヤー表示

        //ワープホールの出口を生成
        objectController.WarpExitCreate();

        //距離を足す
        dis += warpDis;

        yield return new WaitForSeconds(0.5f);

        isWarp = false; //ワープ終了
    }

    public void GameOver()
    {
        state = STATE.GAMEOVER;
        StartCoroutine(GameEnd());
    }

    IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(toResultWait);

        //飛行距離保存
        PlayerPrefs.SetFloat("Distance", dis);

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
