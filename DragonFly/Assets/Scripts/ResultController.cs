using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultController : MonoBehaviour
{
    [SerializeField] SceneChange sceneChange;

    [SerializeField] Text distance;
    float dis = 0;
    float d = 0;
    float lastDis = 0;
    [SerializeField] Text newScoreText;

    bool canMove = false;

    [SerializeField] Text[] rankingScore;
    float[] scores = new float[4];
    bool isUpdated = false;

    private void Awake()
    {
        dis = PlayerPrefs.GetFloat("Distance", 0);
        lastDis = PlayerPrefs.GetFloat("LastDistance", 0);
        newScoreText.enabled = false;

        sceneChange.FadeIn();
        StartCoroutine(FadeEndCheck());

        //ランキング取得
        for(int i = 0; i < rankingScore.Length; i++)
        {
            scores[i] = PlayerPrefs.GetFloat(i.ToString(), 0f);
        }

        for (int i = 0; i < rankingScore.Length; i++)
        {
            rankingScore[i].enabled = false;
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

                //スペース/エンターを押したらスコアのカウントアップをスキップする
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                {
                    d = dis;
                }

                distance.text = d.ToString("f0") + "m";
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
                    for (int i = 0; i < scores.Length; i++)
                    {
                        PlayerPrefs.SetFloat(i.ToString(), scores[i]);
                    }

                    //ランキング表示
                    for (int i = 0; i < rankingScore.Length; i++)
                    {
                        rankingScore[i].enabled = true;
                        rankingScore[i].text = scores[i].ToString("f0") + "m";
                    }
                }
            }
        }

        //データ削除コマンド
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
