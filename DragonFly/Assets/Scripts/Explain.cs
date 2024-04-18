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

    private void Update()
    {
        for(int i = 0; i < text.Length; i++)
        {
            if(i == page) text[i].enabled = true;
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

    public void LastPage()
    {
        if(page > 0) page--;
        if(page <= 0) page = text.Length;
    }

    public void NextPage()
    {
        if(page < text.Length) page++;
        if(page >= text.Length) page = 0;
    }
}
