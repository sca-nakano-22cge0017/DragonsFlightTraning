using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン遷移を制御
/// </summary>
public class SceneChange : MonoBehaviour
{
    [SerializeField] Fade fadeIn;
    bool isFadeIn;
    [SerializeField] Fade fadeOut;
    bool isFadeOut;
    [SerializeField] float fadeTime;

    string sceneName = "MainScene";

    bool isFadeInEnd = false;

    public bool IsFadeInEnd
    {
        get { return isFadeInEnd; }
    }

    private void Update()
    {
        //フェードイン
        if(isFadeIn)
        {
            isFadeIn = false;
            fadeIn.FadeIn(fadeTime, () => isFadeInEnd = true);
        }

        //フェードアウト
        if(isFadeOut)
        {
            isFadeOut = false;

            if (sceneName == "GameEnd")
            {
                fadeOut.FadeOut(fadeTime, () => Application.Quit()); //ゲーム終了
            }
            else fadeOut.FadeOut(fadeTime, () => SceneManager.LoadScene(sceneName)); //シーン遷移
        }
    }

    public void FadeIn()
    {
        isFadeIn = true;
    }

    /// <summary>
    /// メインゲームへ
    /// </summary>
    public void ToMain()
    {
        isFadeOut = true;
        sceneName = "MainScene";
    }

    /// <summary>
    /// タイトル画面へ
    /// </summary>
    public void ToTitle()
    {
        isFadeOut = true;
        sceneName = "TitleScene";
    }

    /// <summary>
    /// リザルト画面へ
    /// </summary>
    public void ToResult()
    {
        isFadeOut = true;
        sceneName = "ResultScene";
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void GameEnd()
    {
        isFadeOut = true;
        sceneName = "GameEnd";
    }
}
