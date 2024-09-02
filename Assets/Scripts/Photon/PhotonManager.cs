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

    public void JoinOrCreateRoom(string name, int n)
    {
        RoomOptions option = new()
        {
            MaxPlayers = n,
        };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: option);

        PanelManager.Instance.InitPanel((int)Panel.roomPanel);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinedRoom()
    {
        print("¹æ ÂÉÀÎ ¼º°ø!");
    }
}
