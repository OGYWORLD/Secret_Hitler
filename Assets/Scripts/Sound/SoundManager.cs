using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource bgmAS; // 배경음악 Audio Source
    public AudioSource effectAS; // 효과음 Audio Source
    public AudioSource effect2AS; // 효과음 Audio Source2

    /* 배경음악 */
    public AudioClip startBGM; // 시작화면 배경음악
    public AudioClip waitBGM; // 시작 전 대기 room 배경음악
    public AudioClip lobyBGM; // 로비 배경음악
    public AudioClip defaultBGM; // 게임 시작 후 대기 room 배경음악
    public AudioClip liberalBGM; // 리버럴 배경음악
    public AudioClip pacistBGM; // 파시스트 배경음악

    /* 효과음 */
    public AudioClip bellSF; // 종소리
    public AudioClip clickSF; // 클릭 소리
    public AudioClip paperSF; // 종이 소리
    public AudioClip shortBellSF; // 짧은 종소리
    public AudioClip typeBellSF; // 타자기 종소리
    public AudioClip charmBellSF; // 밝은 종소리
    public AudioClip killBellSF; // 총살 종소리
    

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        bgmAS.clip = startBGM;
        bgmAS.Play();
    }

    public void PlayBGM(AudioClip bgm)
    {
        bgmAS.Stop();
        bgmAS.clip = bgm;
        bgmAS.Play();
    }

    public void PlaySoundEffect(AudioClip soundeffect)
    {
        effectAS.Stop();
        effectAS.clip = soundeffect;
        effectAS.Play();
    }

    public void PlaySoundEffect2(AudioClip soundeffect)
    {
        effect2AS.Stop();
        effect2AS.clip = soundeffect;
        effect2AS.Play();
    }
}
