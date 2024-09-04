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

    public void OnConnected() // ������ ����Ǿ��� �� ȣ��Ǵ� �Լ�
    {
        Debug.Log("���� ä�� ������ ����Ǿ����ϴ�.");

        chatClient.Subscribe(new string[] { roomName }); // �۷ι� ä�� ����
        print($"{roomName} ä�� ���� ����");
    }

    // �ٸ� ����� ���� �޽����� ���� �� ȣ��Ǵ� �޽���
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

    // ä�� �޽��� ������
    public void SendChat(string message)
    {
        chatClient.PublishMessage(roomName, message);
    }

    public void LeaveChannel(string channelName)
    {
        print($"{channelName} ä�� ���� ����");
        // Ư�� ä�ο��� ������ ���� (ä�� ������)
        chatClient.Unsubscribe(new string[] { channelName });
    }


    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }

    // ��� ��������
    public void OnDisconnected()
    {
        
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    // ���� ���� ������Ʈ
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
