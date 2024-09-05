using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public Text roomNameText;
    public Text stateText;
    public Text chatText;
    public GameObject chatObj;

    public GameObject[] nameCards;
    public Transform[] cardTrans;

    public TMP_InputField chatInputField;

    //private List<Text> otherNameTexts = new List<Text>();
    //private List<Text> otherStateTexts = new List<Text>();
    //private List<Text> otherChatTexts = new List<Text>();

    //public Dictionary<string, (GameObject, Text)> chatDictionary = new Dictionary<string, (GameObject, Text)>(); // <�г���, (ä��obj, ä��â)>
    //private List<GameObject> chatObjList = new List<GameObject>();

    public Dictionary<string, GameObject> cardDictionary = new Dictionary<string, GameObject>();

    public Button readyButton;
    public Button outButton;
    public GameObject settingButtonObj;

    private TextMeshProUGUI readyOrStartTMP;

    private Color masterColor = new Color(1f, 1f, 144/255f);
    private Color readyColor = new Color(112f/255f, 1f, 249f/255f);
    private Color nonReadyColor = new Color(188/255f, 188 / 255f, 188 / 255f);

    private void Awake()
    {
        chatInputField.onEndEdit.AddListener(SendChattingMessage);
    }

    private void Start()
    {
        // �̸��̶� ����(����) �޾ƿ���
        /*
        for(int i = 0; i < nameCards.Length; i++)
        {
            Text[] texts = nameCards[i].GetComponentsInChildren<Text>();
            otherNameTexts.Add(texts[0]);
            otherStateTexts.Add(texts[1]);

            GameObject obj = nameCards[i].transform.GetChild(2).gameObject; // ä�� ������Ʈ ��������
            chatObjList.Add(obj);

            Text t = obj.GetComponentInChildren<Text>();
            otherChatTexts.Add(t);
        }
        */

        outButton.onClick.AddListener(OnPlayerChoiceLeftRoom);
        readyButton.onClick.AddListener(SetReady);
        readyButton.onClick.AddListener(StartGame);

        readyOrStartTMP = readyButton.GetComponentInChildren<TextMeshProUGUI>();
        settingButtonObj.SetActive(false);
    }

    private void OnPlayerChoiceLeftRoom() // ������ ��ư�� ������ ��
    {
        PhotonNetwork.LeaveRoom();
        PhotonChatManager.Instance.LeaveChannel(PhotonNetwork.CurrentRoom.Name);
        PanelManager.Instance.InitPanel((int)Panel.enterDelayPanel);
    }

    public override void OnJoinedRoom() // �� �������� ��
    {
        PhotonChatManager.Instance.ConnectToServer(PhotonNetwork.CurrentRoom.Name); // ä�� ���� ����

        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // �� �̸� ����
        UpdateOtherInfo();

        PhotonNetwork.Instantiate("PhotonPlayer", Vector3.zero, Quaternion.identity); // RPC�� ���� ������ ����

        if (PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer) // �����̶�� ���ӽ��� & ����â ���̰�
        {
            //settingButtonObj.SetActive(true);
            PhotonManager.Instance.playerProperties["ready"] = true;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "���� ����";

            stateText.color = masterColor;
            stateText.text = "ȸ�� ������";

            readyButton.interactable = false;
        }
        else
        {
            //settingButtonObj.SetActive(false);
            PhotonManager.Instance.playerProperties["ready"] = false;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "���� �غ�";

            stateText.color = nonReadyColor;
            stateText.text = "ȸ�������� ���� ��...";


            readyButton.interactable = true;
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

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CheckAllReady();
        }
    }

    public override void OnLeftRoom() // ���� ������ ��
    {
        PhotonManager.Instance.playerProperties["ready"] = false;
        PhotonManager.Instance.SetCustomProperty();

        // ���� ä�� ���� �����
        chatObj.SetActive(false);
        chatText.text = "";

        InitChatObj(); // �ٸ� ��� ä�� ���� �����

        NameStateListInit(); // �̸� ���� �ؽ�Ʈ ����
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

            stateText.color = readyColor;
            stateText.text = "�Թ� ȸ�� �غ� �Ϸ�";
        }
        else // ���� ���¶��
        {
            PhotonManager.Instance.playerProperties["ready"] = false;
            readyOrStartTMP.text = "���� �غ�";

            stateText.color = nonReadyColor;
            stateText.text = "ȸ�������� ���� ��...";
        }

        PhotonManager.Instance.SetCustomProperty();
    }

    public void StartGame() // ���� ����
    {
        if (PhotonNetwork.MasterClient != PhotonNetwork.LocalPlayer) return; // ������ �ƴ϶�� ��ȯ

        // ���� ����
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
            readyButton.interactable = false;
            return;
        }

        readyButton.interactable = true;
    }

    
    // ���� ���°� ���ߴٸ� state�� �ٲ��ֱ�
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateOtherInfo();
        CheckAllReady();
    }

    public void GetCurrentUserInfo() // ���⼭ ��ǳ���� ��������� �� ��
    {
        int index = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value != PhotonNetwork.LocalPlayer)
            {
                /*
                otherNameTexts[index].text = player.Value.NickName;

                chatDictionary[player.Value.NickName] = (chatObjList[index], otherChatTexts[index]);

                if(player.Value.IsMasterClient)
                {
                    otherStateTexts[index].color = masterColor;
                    otherStateTexts[index].text = "ȸ�� ������";
                }
                else if ((bool)player.Value.CustomProperties["ready"])
                {
                    otherStateTexts[index].color = readyColor;
                    otherStateTexts[index].text = "�Թ� ȸ�� �غ� �Ϸ�";
                }
                else
                {
                    otherStateTexts[index].color = nonReadyColor;
                    otherStateTexts[index].text = "ȸ�������� ���� ��...";
                }
                */

                nameCards[index].transform.position = cardTrans[index].position;
                nameCards[index].SetActive(true);
                cardDictionary[player.Value.NickName] = nameCards[index];

                Text[] texts = nameCards[index].GetComponentsInChildren<Text>(); // 0: name, 1: state

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
        /*
        foreach (Text name in otherNameTexts)
        {
            name.text = "";
        }

        foreach (Text state in otherStateTexts)
        {
            state.text = "";
        }

        foreach(Text chat in otherChatTexts)
        {
            chat.text = "";
        }

        foreach(GameObject obj in chatObjList)
        {
            obj.SetActive(false);
        }

        chatDictionary.Clear();
        */

        // ī�� ��Ȱ��ȭ �ʿ�
        foreach(var card in cardDictionary)
        {
            card.Value.SetActive(false);
        }

        cardDictionary.Clear();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer) // �����̶�� ���ӽ��� & ����â ���̰�
        {
            //settingButtonObj.SetActive(true);
            PhotonManager.Instance.playerProperties["ready"] = true;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "���� ����";

            stateText.color = masterColor;
            stateText.text = "ȸ�� ������";
            readyButton.interactable = false;
        }
        else
        {
            //settingButtonObj.SetActive(false);
            PhotonManager.Instance.playerProperties["ready"] = false;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "���� �غ�";

            stateText.color = nonReadyColor;
            stateText.text = "ȸ�������� ���� ��...";
        }
    }

    public void SendChattingMessage(string s) // ä�� ������
    {
        chatText.text = s;
        StartCoroutine(ShowMyChatImageAnim());
        PhotonChatManager.Instance.SendChat(s);
    }

    public void ShowChatImage(string name)
    {
        if(name != DatabaseManager.Instance.name)
        {
            StartCoroutine(ShowChatImageAnim(name));
        }
    }

    private void InitChatObj()
    {
        foreach(var cardObj in cardDictionary)
        {
            GameObject chatObj = cardObj.Value.transform.GetChild(2).gameObject;

            Text chat = chatObj.GetComponent<Text>();
            chat.text = "";
            chatObj.SetActive(false);
        }
    }


    private IEnumerator ShowChatImageAnim(string name)
    {
        /*
        chatDictionary[name].Item1.SetActive(true);

        yield return new WaitForSeconds(3f);

        if(PhotonNetwork.InRoom)
        {
            chatDictionary[name].Item1.SetActive(false);
            chatDictionary[name].Item2.text = "";
        }
        */

        GameObject chatObj = cardDictionary[name].transform.GetChild(2).gameObject;
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
        chatObj.SetActive(true);

        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.InRoom)
        {
            chatObj.SetActive(false);
            chatText.text = "";
        }
    }
}
