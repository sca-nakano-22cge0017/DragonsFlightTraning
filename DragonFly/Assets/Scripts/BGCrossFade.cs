using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGCrossFade : MonoBehaviour
{
    [SerializeField, Header("フェードにかかる時間")] float fadeTime; 
    float nowTime = 0;

    [SerializeField] Image[] bg;

    Material[] material;


    private void Awake()
    {
        material = new Material[bg.Length];

        //マテリアルの取得
        for(int i = 0; i < bg.Length; i++)
        {
            material[i] = bg[i].material;

            if(i == 0) material[i].color = new Color(1, 1, 1, 1);
            else material[i].color = new Color(1, 1, 1, 0);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// クロスフェード
    /// </summary>
    /// <param name="lastModeNum">前のモード</param>
    /// <param name="modeNum">今のモード</param>
    public void CrossFade(int lastModeNum, int modeNum)
    {
        nowTime = 0;

        StartCoroutine(FadeOut(lastModeNum));
        StartCoroutine(FadeIn(modeNum));
    }

    IEnumerator FadeIn(int modeNum)
    {
        float alpha = 0;
        while(alpha < 1)
        {
            nowTime += Time.deltaTime;
            alpha = nowTime / fadeTime;
            material[modeNum].color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        material[modeNum].color = new Color(1, 1, 1, 1);
    }

    IEnumerator FadeOut(int lastModeNum)
    {
        float alpha = 1;
        while (alpha > 0)
        {
            nowTime += Time.deltaTime;
            alpha = 1 - (nowTime / fadeTime);
            material[lastModeNum].color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        material[lastModeNum].color = new Color(1, 1, 1, 0);
    }
}
