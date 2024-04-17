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

    private void Awake()
    {
        dis = PlayerPrefs.GetFloat("Distance", 0);
        lastDis = PlayerPrefs.GetFloat("LastDistance", 0);
        newScoreText.enabled = false;

        sceneChange.FadeIn();
        StartCoroutine(FadeEndCheck());
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
                //前回のスコアより高かったら
                if (lastDis < dis)
                {
                    //新記録の表示
                    newScoreText.enabled = true;

                    //スコア更新
                    lastDis = dis;
                    PlayerPrefs.SetFloat("lastScore", lastDis);
                }
            }
        }

        //データ削除コマンド
        if(Input.GetKey(KeyCode.C))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
