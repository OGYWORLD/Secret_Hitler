using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; set; }
    public Dictionary<int, (int, int)> cntDictionary { get; set; } = new Dictionary<int, (int, int)>(); // 인원수에 따른 리버럴 파시스트 인원수

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 인원수 딕셔너리 초기화
        cntDictionary[5] = (3, 2);
        cntDictionary[6] = (4, 2);
        cntDictionary[7] = (4, 3);
        cntDictionary[8] = (5, 3);
        cntDictionary[9] = (5, 4);
        cntDictionary[10] = (6, 4);
    }

    public void TryConnect()
    {
        //TODO: 커스텀 머시기해서 이메일로 다시 바꾸기
        //PhotonNetwork.LocalPlayer.NickName = DatabaseManager.Instance.data.email;
        PhotonNetwork.LocalPlayer.NickName = DatabaseManager.Instance.data.name;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PanelManager.Instance.InitPanel((int)Panel.mainMenuPanel);
        PanelManager.Instance.toMainMenu?.Invoke();
    }

    public override void OnJoinedLobby()
    {
        print("OnJoinedLobby 호출");

    }

    public void JoinOrCreateRoom(string name, int n)
    {
        PanelManager.Instance.InitPanel((int)Panel.enterDelayPanel);

        RoomOptions option = new()
        {
            MaxPlayers = n,
        };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: option);
    }

    public void CreateRoom(string name, int n)
    {
        PanelManager.Instance.InitPanel((int)Panel.enterDelayPanel);

        RoomOptions option = new()
        {
            MaxPlayers = n,
        };
        PhotonNetwork.CreateRoom(roomName: name, roomOptions: option);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
