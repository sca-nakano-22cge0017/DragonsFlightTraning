using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] DataSaver dataSaver;
    [SerializeField] SceneChange sceneChange;

    [SerializeField] Text distance;
    float dis = 0;
    float d = 0;
    [SerializeField] Text newScoreText;

    bool canMove = false;

    [SerializeField] Text[] rankingScore;
    float[] scores = new float[4];
    bool isUpdated = false;

    [SerializeField] Text skipExplainForPC;
    [SerializeField] Text skipExplainForAndroid;

    private void Awake()
    {
#if UNITY_EDITOR
        skipExplainForPC.enabled = true;
        skipExplainForAndroid.enabled = false;
#endif
#if UNITY_ANDROID
        skipExplainForPC.enabled = false;
        skipExplainForAndroid.enabled = true;
#endif

        // さっきのスコアを取得
        dis = dataSaver.loadLatestData();

        newScoreText.enabled = false;

        sceneChange.FadeIn();
        StartCoroutine(FadeEndCheck());

        // ランキング取得
        scores = dataSaver.loadRankingData(scores.Length);

        for (int i = 0; i < rankingScore.Length; i++)
        {
            rankingScore[i].enabled = false;
        }

        if (GameObject.FindObjectOfType<BGM>().GetComponent<BGM>() is var bgm)
        {
            bgm.BGMStart(); // BGM再生
        }
    }

    /// <summary>
    /// フェードインが完了したかチェックする
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeEndCheck()
    {
        yield return new WaitUntil(() => sceneChange.IsFadeInEnd);
        canMove = true;
    }

    void Update()
    {
        if(canMove)
        {
            if (d < dis)
            {
                d++;

                //スペース/エンターを押したら/画面をタップしたらスコアのカウントアップをスキップする
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
                {
                    d = dis;
                }

                distance.text = Mathf.Floor(d).ToString() + "m";
            }
            else if (d >= dis)
            {
                d = dis;

                //１位スコアより高かったら
                if (scores[0] < dis)
                {
                    //新記録の表示
                    newScoreText.enabled = true;
                }

                if(!isUpdated) //ランキング更新・表示
                {
                    isUpdated = true;
                    scores[scores.Length - 1] = dis;

                    //降順ソート
                    System.Array.Sort(scores);
                    System.Array.Reverse(scores);

                    //ランキング保存
                    dataSaver.saveRankingData(scores);

                    //ランキング表示
                    for (int i = 0; i < rankingScore.Length; i++)
                    {
                        rankingScore[i].enabled = true;
                        rankingScore[i].text = Mathf.Floor(scores[i]).ToString() + "m";
                    }
                }
            }
        }

        //データ削除コマンド
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            // データ初期化
            dataSaver.DataInitialize(scores.Length);
        }
    }
}
