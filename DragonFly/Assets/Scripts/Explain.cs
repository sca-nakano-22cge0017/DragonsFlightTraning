using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explain : MonoBehaviour
{
    [SerializeField] GameObject window;

    public void Display()
    {
        window.SetActive(true);
    }

    public void UnDisplay()
    {
        window.SetActive(false);
    }
}
