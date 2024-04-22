using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioClip wind;
    [SerializeField] AudioSource _wind;

    [SerializeField] AudioClip item;
    [SerializeField] AudioClip warp;
    [SerializeField] AudioClip gameover;
    [SerializeField] AudioSource SE;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ItemCatch()
    {
        SE.PlayOneShot(item);
    }

    public void Warp()
    {
        SE.PlayOneShot(warp);
    }
    
    public void WindStart()
    {
        _wind.PlayOneShot(wind);
    }

    public void WindStop()
    {
        _wind.Stop();
    }

    public void GameOver()
    {
        SE.PlayOneShot(gameover);
    }
}
