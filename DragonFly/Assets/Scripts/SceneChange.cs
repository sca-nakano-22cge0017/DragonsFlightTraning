using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ボタンによるシーン遷移を制御
/// </summary>
public class SceneChange : MonoBehaviour
{
    /// <summary>
    /// メインゲームへ
    /// </summary>
    public void ToMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    /// <summary>
    /// タイトル画面へ
    /// </summary>
    public void ToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void GameEnd()
    {
        Application.Quit();
    }
}
