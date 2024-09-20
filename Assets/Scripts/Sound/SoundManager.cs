using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource bgmAS; // ������� Audio Source
    public AudioSource effectAS; // ȿ���� Audio Source
    public AudioSource effect2AS; // ȿ���� Audio Source2

    /* ������� */
    public AudioClip startBGM; // ����ȭ�� �������
    public AudioClip waitBGM; // ���� �� ��� room �������
    public AudioClip lobyBGM; // �κ� �������
    public AudioClip defaultBGM; // ���� ���� �� ��� room �������
    public AudioClip liberalBGM; // ������ �������
    public AudioClip pacistBGM; // �Ľý�Ʈ �������

    /* ȿ���� */
    public AudioClip bellSF; // ���Ҹ�
    public AudioClip clickSF; // Ŭ�� �Ҹ�
    public AudioClip paperSF; // ���� �Ҹ�
    public AudioClip shortBellSF; // ª�� ���Ҹ�
    public AudioClip typeBellSF; // Ÿ�ڱ� ���Ҹ�
    public AudioClip charmBellSF; // ���� ���Ҹ�
    public AudioClip killBellSF; // �ѻ� ���Ҹ�
    

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
