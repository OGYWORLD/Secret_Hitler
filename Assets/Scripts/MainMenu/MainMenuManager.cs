using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    public Button speedRoomButton;
    public Button makeRoomButton;
    public Button settingButton;

    public GameObject settingPanel;
    public GameObject makeRoomPanel;

    private void Awake()
    {
        makeRoomButton.onClick.AddListener(ToMakeRoom);
        settingButton.onClick.AddListener(ToSetting);
        speedRoomButton.onClick.AddListener(OnSpeedRoom);

        settingPanel.SetActive(false);
        makeRoomPanel.SetActive(false);
    }

    private void ToMakeRoom()
    {
        makeRoomPanel.SetActive(true);
    }

    private void ToSetting()
    {
        settingPanel.SetActive(true);
    }

    private void OnSpeedRoom()
    {
        PhotonManager.Instance.JoinOrCreateRoom("빠른 시작 해요~", 5);
    }
}
