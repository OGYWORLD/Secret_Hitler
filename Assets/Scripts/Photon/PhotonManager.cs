using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; set; }
    public Dictionary<int, (int, int)> cntDictionary { get; set; } = new Dictionary<int, (int, int)>(); // 인원수에 따른 리버럴, 파시스트 인원수

    public List<RoomInfo> roomList { get; set; } = new List<RoomInfo>();

    private void Awake()
    {
        if (Instance == null)
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
        PhotonNetwork.LocalPlayer.NickName = DatabaseManager.Instance.data.name;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinedLobby()
    {
        SoundManager.Instance.PlayBGM(SoundManager.Instance.lobyBGM);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PanelManager.Instance.InitPanel((int)Panel.mainMenuPanel);
        PanelManager.Instance.toMainMenu?.Invoke();
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

    public void EnterRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    public int CheckDPRoomName(string name)
    {
        int dpCnt = 0;
        foreach(RoomInfo room in roomList)
        {
            if(name.CompareTo(room.Name) == 0)
            {
                dpCnt++;
            }
        }

        return dpCnt;
    }

    public string GetCurrentRoomName()
    {
        return PhotonNetwork.CurrentRoom.Name;
    }

    public int GetMaxPlayer()
    {
        return PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public void ChangeNickName()
    {
        PhotonNetwork.LocalPlayer.NickName = DatabaseManager.Instance.data.name;
    }
}
