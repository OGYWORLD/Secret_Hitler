using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public PlayManager playManager;

    private TextMeshProUGUI readyOrStartTMP;

    private Color masterColor = new Color(1f, 1f, 144/255f);
    private Color readyColor = new Color(112f/255f, 1f, 249f/255f);
    private Color nonReadyColor = new Color(188/255f, 188 / 255f, 188 / 255f);

    private void Awake()
    {
        playManager.chatInputField.onEndEdit.AddListener(SendChattingMessage);
        playManager.chatInputField.onEndEdit.AddListener(CleanTextAfterSendChat);
    }

    private void Start()
    {
        playManager.outButton.onClick.AddListener(OnPlayerChoiceLeftRoom);
        playManager.readyButton.onClick.AddListener(SetReady);
        playManager.readyButton.onClick.AddListener(StartGame);

        readyOrStartTMP = playManager.readyButton.GetComponentInChildren<TextMeshProUGUI>();
        playManager.settingButtonObj.SetActive(false);
    }

    private void OnPlayerChoiceLeftRoom() // ������ ��ư�� ������ ��
    {
        PhotonManager.Instance.playerProperties["ready"] = false;
        PhotonManager.Instance.SetCustomProperty();

        // ���� ä�� ���� �����
        playManager.chatObj.SetActive(false);
        playManager.chatText.text = "";

        InitChatObj(); // �ٸ� ��� ä�� ���� �����

        NameStateListInit(); // �̸� ���� �ؽ�Ʈ ����

        PhotonNetwork.LeaveRoom();
        PhotonChatManager.Instance.LeaveChannel(PhotonNetwork.CurrentRoom.Name);
        PanelManager.Instance.InitPanel((int)Panel.enterDelayPanel);
    }

    public override void OnJoinedRoom() // �� �������� ��
    {
        PhotonChatManager.Instance.ConnectToServer(PhotonNetwork.CurrentRoom.Name); // ä�� ���� ����

        playManager.roomNameText.text = PhotonNetwork.CurrentRoom.Name; // �� �̸� ����
        UpdateOtherInfo();

        playManager.InitWhenJoinedRoom(); // ������Ʈ �ʱ�ȭ

        // ��ư�� Ȱ��ȭ
        playManager.readyButton.gameObject.SetActive(true);
        playManager.outButton.gameObject.SetActive(true);

        // Ŀ���� ǰ ������Ƽ �Ӽ� ���� (���� ���� ������ �ƴ���)
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonManager.Instance.roomProperties["ing"] = false;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonManager.Instance.roomProperties);
        }

        if (PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer) // �����̶�� ���ӽ��� & ����â ���̰�
        {
            //settingButtonObj.SetActive(true);
            PhotonManager.Instance.playerProperties["ready"] = true;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "���� ����";

            playManager.stateText.color = masterColor;
            playManager.stateText.text = "ȸ�� ������";

            playManager.readyButton.interactable = false;
        }
        else
        {
            //settingButtonObj.SetActive(false);
            PhotonManager.Instance.playerProperties["ready"] = false;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "���� �غ�";

            playManager.stateText.color = nonReadyColor;
            playManager.stateText.text = "ȸ�������� ���� ��...";


            playManager.readyButton.interactable = true;
        }

        PanelManager.Instance.InitPanel((int)Panel.roomPanel);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // �濡 ������ ������ ��
    {
        UpdateOtherInfo();

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CheckAllReady();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // �濡 ������ ������ ��
    {
        UpdateOtherInfo();

        if((bool)PhotonNetwork.CurrentRoom.CustomProperties["ing"]) // ���� �߿� ���� ������ ���� ����
        {
            if(PhotonNetwork.IsMasterClient) // ��� ������
            {
                playManager.KickAllPlayerRPC();
            }
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CheckAllReady();
        }
    }

    public override void OnLeftRoom() // ���� ������ ��
    {
        
    }

    public void UpdateOtherInfo() // �濡 �ִ� ��� ��� ������Ʈ
    {
        NameStateListInit();
        GetCurrentUserInfo();
    }

    public void SetReady() // �����ϱ�
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) return; // �����̶�� ��ȯ

        if ((bool)PhotonManager.Instance.playerProperties["ready"] == false) // ���� ���°� �ƴ϶��
        {
            PhotonManager.Instance.playerProperties["ready"] = true;
            readyOrStartTMP.text = "�غ� ���";

            playManager.stateText.color = readyColor;
            playManager.stateText.text = "�Թ� ȸ�� �غ� �Ϸ�";
        }
        else // ���� ���¶��
        {
            PhotonManager.Instance.playerProperties["ready"] = false;
            readyOrStartTMP.text = "���� �غ�";

            playManager.stateText.color = nonReadyColor;
            playManager.stateText.text = "ȸ�������� ���� ��...";
        }

        PhotonManager.Instance.SetCustomProperty();
    }

    public void StartGame() // ���� ����
    {
        if (PhotonNetwork.MasterClient != PhotonNetwork.LocalPlayer) return; // ������ �ƴ϶�� ��ȯ

        // ��ư ��Ȱ��ȭ
        playManager.readyButton.gameObject.SetActive(false);
        playManager.outButton.gameObject.SetActive(false);

        PickPosition(); // ���� �̱�
    }

    private void PickPosition() // ���� �̱�
    {
        // ������ �ƴ� �ٸ� ����� ��ư�� ��Ȱ��ȭ
        // �� Ŀ���� ������Ƽ�� �� ���̸� ��Ȱ��ȭ �ǵ��� ����
        PhotonManager.Instance.roomProperties["ing"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonManager.Instance.roomProperties);

        playManager.StateInitForGameStartRPC(); // ���� �����
        playManager.PickPosition(); // ���� �̱�
        playManager.SendPickPosition(); // ���� �� �˸�
        playManager.ShowPositionRPC(); // ���� �����ֱ�
    }

    private void CheckAllReady()
    {
        int readyCnt = 0;

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) // ������ ���� �ߴ��� Ȯ��
        {
            if ((bool)player.Value.CustomProperties["ready"])
            {
                readyCnt++;
            }
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient && readyCnt != PhotonNetwork.CurrentRoom.MaxPlayers) // ������ ���� �� �ߴٸ� ��ȯ
        {
            playManager.readyButton.interactable = false;
            return;
        }

        playManager.readyButton.interactable = true;
    }

    
    // ���� ���°� ���ߴٸ� state�� �ٲ��ֱ�
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateOtherInfo();
        CheckAllReady();
    }

    public void GetCurrentUserInfo()
    {
        int index = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value != PhotonNetwork.LocalPlayer)
            {
                playManager.nameCards[index].transform.position = playManager.cardTrans[index].position;
                playManager.nameCards[index].SetActive(true);
                playManager.cardDictionary[player.Value.NickName] = playManager.nameCards[index];

                Text[] texts = playManager.nameCards[index].GetComponentsInChildren<Text>(); // 0: name, 1: state

                texts[0].text = player.Value.NickName;

                if (player.Value.IsMasterClient)
                {
                    texts[1].color = masterColor;
                    texts[1].text = "ȸ�� ������";
                }
                else if ((bool)player.Value.CustomProperties["ready"])
                {
                    texts[1].color = readyColor;
                    texts[1].text = "�Թ� ȸ�� �غ� �Ϸ�";
                }
                else
                {
                    texts[1].color = nonReadyColor;
                    texts[1].text = "ȸ�������� ���� ��...";
                }

                index++;
            }
        }
    }

    private void NameStateListInit()
    {
        // ī�� ��Ȱ��ȭ �ʿ�
        foreach(var card in playManager.cardDictionary)
        {
            card.Value.SetActive(false);
        }

        playManager.cardDictionary.Clear();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer) // �����̶�� ���ӽ��� & ����â ���̰�
        {
            //settingButtonObj.SetActive(true);
            PhotonManager.Instance.playerProperties["ready"] = true;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "���� ����";

            playManager.stateText.color = masterColor;
            playManager.stateText.text = "ȸ�� ������";
            playManager.readyButton.interactable = false;
        }
        else
        {
            //settingButtonObj.SetActive(false);
            PhotonManager.Instance.playerProperties["ready"] = false;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "���� �غ�";

            playManager.stateText.color = nonReadyColor;
            playManager.stateText.text = "ȸ�������� ���� ��...";
        }
    }

    public void SendChattingMessage(string s) // ä�� ������
    {
        playManager.chatText.text = s;
        StartCoroutine(ShowMyChatImageAnim());
        PhotonChatManager.Instance.SendChat(s);
    }

    public void ShowChatImage(string name) // ä�� �̹��� ���̱�
    {
        if(name != DatabaseManager.Instance.name)
        {
            StartCoroutine(ShowChatImageAnim(name));
        }
    }

    private void InitChatObj() // �濡�� ���� �� ä�� �ؽ�Ʈ �ʱ�ȭ & �̹��� ��Ȱ��ȭ
    {
        foreach(var cardObj in playManager.cardDictionary)
        {
            GameObject chatObj = cardObj.Value.transform.GetChild(2).gameObject;

            Text chat = chatObj.GetComponentInChildren<Text>();
            chat.text = "";
            chatObj.SetActive(false);
        }
    }

    private void CleanTextAfterSendChat(string s)
    {
        playManager.chatInputField.text = "";
    }

    // Ŀ���� �� ������Ƽ ���� �� ȣ��
    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        PhotonHashtable hashTable = PhotonNetwork.CurrentRoom.CustomProperties;

        if ((bool)hashTable["ing"])
        {
            playManager.roomNameText.text = "";
            playManager.readyButton.gameObject.SetActive(false);
            playManager.outButton.gameObject.SetActive(false);
        }
        else if(!(bool)hashTable["ing"])
        {
            playManager.roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            playManager.readyButton.gameObject.SetActive(true);
            playManager.outButton.gameObject.SetActive(true);
        }
    }

    private IEnumerator ShowChatImageAnim(string name)
    {
        GameObject chatObj = playManager.cardDictionary[name].transform.GetChild(2).gameObject;
        chatObj.SetActive(true);

        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.InRoom)
        {
            Text chatText = chatObj.GetComponentInChildren<Text>();
            chatObj.SetActive(false);
            chatText.text = "";
        }
    }

    private IEnumerator ShowMyChatImageAnim()
    {
        playManager.chatObj.SetActive(true);

        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.InRoom)
        {
            playManager.chatObj.SetActive(false);
            playManager.chatText.text = "";
        }
    }
}
