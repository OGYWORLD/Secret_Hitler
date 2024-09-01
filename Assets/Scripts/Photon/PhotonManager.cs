using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; set; }

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
    }

    public void TryConnect()
    {
        PhotonNetwork.LocalPlayer.NickName = DatabaseManager.Instance.data.email;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PanelManager.Instance.InitPanel((int)Panel.mainMenuPanel);
        PanelManager.Instance.toMainMenu?.Invoke();
    }

    public void JoinOrCreateRoom()
    {
        RoomOptions option = new()
        {
            MaxPlayers = 4,
        };
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: option);

        // TODO: 방으로 들어가기
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        print("방 쪼인 성공!");
    }
}
