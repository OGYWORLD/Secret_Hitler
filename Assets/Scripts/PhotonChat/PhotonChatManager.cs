using Photon.Chat;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    public static PhotonChatManager Instance { get; set; }

    private ChatClient chatClient;
    private string roomName;

    public RoomManager roomManager;

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

    private void Update()
    {
        if(chatClient != null)
        {
            chatClient.Service();
        }
    }

    public void ConnectToServer(string roomName)
    {
        this.roomName = roomName;

        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(DatabaseManager.Instance.data.name));
    }

    public void OnConnected() // 서버가 연결되었을 때 호출되는 함수
    {
        Debug.Log("포톤 채팅 서버가 연결되었습니다.");

        chatClient.Subscribe(new string[] { roomName }); // 글로벌 채널 구독
        print($"{roomName} 채팅 서버 연결");
    }

    // 다른 사람이 보낸 메시지를 받을 때 호출되는 메시지
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for(int i = 0; i < senders.Length; i++)
        {
            if(senders[i] != DatabaseManager.Instance.data.name)
            {
                roomManager.chatDictionary[senders[i]].Item2.text = (string)messages[i];
                roomManager.ShowChatImage(senders[i]);
            }
        }
    }

    // 채팅 메시지 보내기
    public void SendChat(string message)
    {
        chatClient.PublishMessage(roomName, message);
    }

    public void LeaveChannel(string channelName)
    {
        print($"{channelName} 채팅 서버 끊음");
        // 특정 채널에서 구독을 해제 (채널 나가기)
        chatClient.Unsubscribe(new string[] { channelName });
    }


    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    // 통신 끊겼을때
    public void OnDisconnected()
    {
        
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    // 유저 상태 업데이트
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
}
