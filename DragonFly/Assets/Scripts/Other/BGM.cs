using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private static BGM instance;
    public static BGM Instance => instance;

    AudioSource audioSource;

    private BGM() { }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        DontDestroyOnLoad(this.gameObject);
    }

    public void BGMStart()
    {
        audioSource.Play();
    }

    public void BGMStop()
    {
        audioSource.Stop();
    }
}
