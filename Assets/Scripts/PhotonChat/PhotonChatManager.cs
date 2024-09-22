using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
    public static PhotonChatManager Instance { get; private set; }

    private ChatClient chatClient;
    private string roomName;

    public PlayManager playManager;
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
        chatClient.Subscribe(new string[] { roomName }); // �۷ι� ä�� ����
    }

    // �ٸ� ����� ���� �޽����� ���� �� ȣ��Ǵ� �޽���
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for(int i = 0; i < senders.Length; i++)
        {
            if(senders[i] != DatabaseManager.Instance.data.name)
            {
                GameObject chatObj = playManager.cardDictionary[senders[i]].transform.GetChild(2).gameObject;
                Text t = chatObj.GetComponentInChildren<Text>();

                t.text = (string)messages[i];
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
