using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

/* 시작화면에서 로그인을 관리하는 스크립트입니다.
 */

public class CLogin : MonoBehaviourPunCallbacks
{
    public TMP_InputField emailInput;
    public TMP_InputField pwInput;

    public Button loginButton;
    public Button createButton;

    public GameObject loginFailTMP;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLogin);
        createButton.onClick.AddListener(OnToCreateAccount);
    }

    private void OnEnable()
    {
        loginFailTMP.SetActive(false);

        emailInput.text = "";
        pwInput.text = "";

        emailInput.interactable = true;
        pwInput.interactable = true;
        loginButton.interactable = true;
        createButton.interactable = true;
    }

    private void OnLogin()
    {
        DatabaseManager.Instance.Login(emailInput.text, pwInput.text, SuccessLogin, FailLogin);

        emailInput.interactable = false;
        pwInput.interactable = false;
        loginButton.interactable = false;
        createButton.interactable = false;
    }

    private void SuccessLogin()
    {
        PhotonManager.Instance.TryConnect();
    }

    private void FailLogin()
    {
        loginFailTMP.SetActive(true);
    }

    private void OnToCreateAccount()
    {
        PanelManager.Instance.InitPanel((int)Panel.createAccountPanel);
    }
}
