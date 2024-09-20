using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/* 시작화면에서 로그인을 관리하는 스크립트입니다.
 */

public class CLogin : MonoBehaviourPunCallbacks
{
    public InputField emailInput;
    public InputField pwInput;

    public Button loginButton;
    public Button createButton;

    public GameObject loginFailTMP;

    public Image titleImage;

    public GameObject afterTitleObj;

    public GameObject checkingTMP;
    public GameObject serverTMP;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLogin);
        createButton.onClick.AddListener(OnToCreateAccount);

        afterTitleObj.SetActive(false);
        checkingTMP.SetActive(false);
        serverTMP.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(IntroAnim());
    }

    private void OnEnable()
    {
        StartCoroutine(IntroAnim());

        loginFailTMP.SetActive(false);
        afterTitleObj.SetActive(false);
        checkingTMP.SetActive(false);
        serverTMP.SetActive(false);

        emailInput.text = "";
        pwInput.text = "";

        emailInput.interactable = true;
        pwInput.interactable = true;
        loginButton.interactable = true;
        createButton.interactable = true;

        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.bellSF);
    }

    private void OnLogin()
    {
        DatabaseManager.Instance.Login(emailInput.text, pwInput.text, SuccessLogin, FailLogin);
    }

    private void SuccessLogin()
    {
        loginFailTMP.SetActive(false);
        emailInput.interactable = false;
        pwInput.interactable = false;
        loginButton.interactable = false;
        createButton.interactable = false;

        PhotonManager.Instance.TryConnect();

        checkingTMP.SetActive(true);
    }

    private void FailLogin()
    {
        loginFailTMP.SetActive(true);
        checkingTMP.SetActive(false);

        emailInput.interactable = true;
        pwInput.interactable = true;
        loginButton.interactable = true;
        createButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        serverTMP.SetActive(true);
    }

    private void OnToCreateAccount()
    {
        PanelManager.Instance.InitPanel((int)Panel.createAccountPanel);
    }

    private IEnumerator IntroAnim()
    {
        float sumTime = 0f;
        float totalTime = 0.7f;

        while (sumTime <= totalTime)
        {
            float t = sumTime / totalTime;
            titleImage.rectTransform.localScale = Vector3.Lerp(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f), t);

            sumTime += Time.deltaTime;

            yield return null;
        }

        titleImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        afterTitleObj.SetActive(true);
    }
}
