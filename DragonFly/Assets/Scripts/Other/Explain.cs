using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explain : MonoBehaviour
{
    [SerializeField] GameObject window;
    [SerializeField] Text[] text;
    int page = 0;

    private void Start()
    {
        for(int i = 0; i < text.Length; i++)
        {
            if (i == page) text[i].enabled = true;
            else text[i].enabled = false;
        }
    }

    public void Display()
    {
        window.SetActive(true);
    }

    public void UnDisplay()
    {
        window.SetActive(false);
    }

    /// <summary>
    /// 前のページへ
    /// </summary>
    public void LastPage()
    {
        page--;
        if(page < 0) page = text.Length - 1;

        TextUpdate();
    }

    /// <summary>
    /// 次のページへ
    /// </summary>
    public void NextPage()
    {
        page++;
        if(page > text.Length - 1) page = 0;

        TextUpdate();
    }

    /// <summary>
    /// 更新
    /// </summary>
    void TextUpdate()
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (i == page) text[i].enabled = true;
            else text[i].enabled = false;
        }
    }
}
