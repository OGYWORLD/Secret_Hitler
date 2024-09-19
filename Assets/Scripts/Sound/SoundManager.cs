using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource bgmAS;
    public AudioSource effectAS;

    public AudioClip startBGM;

    private void Start()
    {
        bgmAS.clip = startBGM;
        bgmAS.Play();
    }

    public void StartGame()
    {

    }
}
