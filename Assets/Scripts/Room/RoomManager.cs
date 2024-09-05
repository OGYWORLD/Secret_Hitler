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

    //public Dictionary<string, (GameObject, Text)> chatDictionary = new Dictionary<string, (GameObject, Text)>(); // <닉네임, (채팅obj, 채팅창)>
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
        // 이름이랑 상태(레디) 받아오기
        /*
        for(int i = 0; i < nameCards.Length; i++)
        {
            Text[] texts = nameCards[i].GetComponentsInChildren<Text>();
            otherNameTexts.Add(texts[0]);
            otherStateTexts.Add(texts[1]);

            GameObject obj = nameCards[i].transform.GetChild(2).gameObject; // 채팅 오브젝트 가져오기
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

    private void OnPlayerChoiceLeftRoom() // 나가기 버튼을 눌렀을 때
    {
        PhotonNetwork.LeaveRoom();
        PhotonChatManager.Instance.LeaveChannel(PhotonNetwork.CurrentRoom.Name);
        PanelManager.Instance.InitPanel((int)Panel.enterDelayPanel);
    }

    public override void OnJoinedRoom() // 방 입장했을 시
    {
        PhotonChatManager.Instance.ConnectToServer(PhotonNetwork.CurrentRoom.Name); // 채팅 서버 연결

        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // 방 이름 설정
        UpdateOtherInfo();

        PhotonNetwork.Instantiate("PhotonPlayer", Vector3.zero, Quaternion.identity); // RPC쓸 포톤 프리팹 생성

        if (PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer) // 방장이라면 게임시작 & 설정창 보이게
        {
            //settingButtonObj.SetActive(true);
            PhotonManager.Instance.playerProperties["ready"] = true;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "게임 시작";

            stateText.color = masterColor;
            stateText.text = "회의 위원장";

            readyButton.interactable = false;
        }
        else
        {
            //settingButtonObj.SetActive(false);
            PhotonManager.Instance.playerProperties["ready"] = false;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "게임 준비";

            stateText.color = nonReadyColor;
            stateText.text = "회의장으로 가는 중...";


            readyButton.interactable = true;
        }

        PanelManager.Instance.InitPanel((int)Panel.roomPanel);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // 방에 누군가 들어왔을 시
    {
        UpdateOtherInfo();

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CheckAllReady();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // 방에 누군가 떠났을 시
    {
        UpdateOtherInfo();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CheckAllReady();
        }
    }

    public override void OnLeftRoom() // 방을 떠났을 시
    {
        PhotonManager.Instance.playerProperties["ready"] = false;
        PhotonManager.Instance.SetCustomProperty();

        // 내꺼 채팅 정보 지우기
        chatObj.SetActive(false);
        chatText.text = "";

        InitChatObj(); // 다른 사람 채팅 정보 지우기

        NameStateListInit(); // 이름 상태 텍스트 비우기
    }

    public void UpdateOtherInfo() // 방에 있는 사람 목록 업데이트
    {
        NameStateListInit();
        GetCurrentUserInfo();
    }

    public void SetReady() // 레디하기
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) return; // 방장이라면 반환

        if ((bool)PhotonManager.Instance.playerProperties["ready"] == false) // 레디 상태가 아니라면
        {
            PhotonManager.Instance.playerProperties["ready"] = true;
            readyOrStartTMP.text = "준비 취소";

            stateText.color = readyColor;
            stateText.text = "입법 회의 준비 완료";
        }
        else // 레디 상태라면
        {
            PhotonManager.Instance.playerProperties["ready"] = false;
            readyOrStartTMP.text = "게임 준비";

            stateText.color = nonReadyColor;
            stateText.text = "회의장으로 가는 중...";
        }

        PhotonManager.Instance.SetCustomProperty();
    }

    public void StartGame() // 게임 시작
    {
        if (PhotonNetwork.MasterClient != PhotonNetwork.LocalPlayer) return; // 방장이 아니라면 반환

        // 게임 시작
    }

    private void CheckAllReady()
    {
        int readyCnt = 0;

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) // 전원이 레디를 했는지 확인
        {
            if ((bool)player.Value.CustomProperties["ready"])
            {
                readyCnt++;
            }
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient && readyCnt != PhotonNetwork.CurrentRoom.MaxPlayers) // 전원이 레디를 안 했다면 반환
        {
            readyButton.interactable = false;
            return;
        }

        readyButton.interactable = true;
    }

    
    // 레디 상태가 변했다면 state를 바꿔주기
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdateOtherInfo();
        CheckAllReady();
    }

    public void GetCurrentUserInfo() // 여기서 말풍선도 지정해줘야 될 듯
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
                    otherStateTexts[index].text = "회의 위원장";
                }
                else if ((bool)player.Value.CustomProperties["ready"])
                {
                    otherStateTexts[index].color = readyColor;
                    otherStateTexts[index].text = "입법 회의 준비 완료";
                }
                else
                {
                    otherStateTexts[index].color = nonReadyColor;
                    otherStateTexts[index].text = "회의장으로 가는 중...";
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
                    texts[1].text = "회의 위원장";
                }
                else if ((bool)player.Value.CustomProperties["ready"])
                {
                    texts[1].color = readyColor;
                    texts[1].text = "입법 회의 준비 완료";
                }
                else
                {
                    texts[1].color = nonReadyColor;
                    texts[1].text = "회의장으로 가는 중...";
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

        // 카드 비활성화 필요
        foreach(var card in cardDictionary)
        {
            card.Value.SetActive(false);
        }

        cardDictionary.Clear();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer) // 방장이라면 게임시작 & 설정창 보이게
        {
            //settingButtonObj.SetActive(true);
            PhotonManager.Instance.playerProperties["ready"] = true;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "게임 시작";

            stateText.color = masterColor;
            stateText.text = "회의 위원장";
            readyButton.interactable = false;
        }
        else
        {
            //settingButtonObj.SetActive(false);
            PhotonManager.Instance.playerProperties["ready"] = false;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "게임 준비";

            stateText.color = nonReadyColor;
            stateText.text = "회의장으로 가는 중...";
        }
    }

    public void SendChattingMessage(string s) // 채팅 보내기
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
