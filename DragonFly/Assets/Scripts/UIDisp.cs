using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI制御
/// </summary>
public class UIDisp : MonoBehaviour
{
    /// <summary>
    /// UIイラストの表示・非表示
    /// </summary>
    /// <param name="image">対象のイラスト</param>
    /// <param name="num">表示・非表示にする数</param>
    /// <param name="isDisp">trueのとき表示、falseのとき非表示</param>
    public void UIDisplay(Image[] image, int num, bool isDisp)
    {
        for (int i = 0; i < num; i++)
        {
            image[i].enabled = isDisp;
        }
    }

    /// <summary>
    /// テキストの代入
    /// </summary>
    /// <param name="text">入れるText</param>
    /// <param name="str">入れたい文字列</param>
    public void TextPutIn(Text text, string str)
    {
        text.text = str;
    }
}
