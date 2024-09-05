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

    private void OnPlayerChoiceLeftRoom() // 나가기 버튼을 눌렀을 때
    {
        PhotonManager.Instance.playerProperties["ready"] = false;
        PhotonManager.Instance.SetCustomProperty();

        // 내꺼 채팅 정보 지우기
        playManager.chatObj.SetActive(false);
        playManager.chatText.text = "";

        InitChatObj(); // 다른 사람 채팅 정보 지우기

        NameStateListInit(); // 이름 상태 텍스트 비우기

        PhotonNetwork.LeaveRoom();
        PhotonChatManager.Instance.LeaveChannel(PhotonNetwork.CurrentRoom.Name);
        PanelManager.Instance.InitPanel((int)Panel.enterDelayPanel);
    }

    public override void OnJoinedRoom() // 방 입장했을 시
    {
        PhotonChatManager.Instance.ConnectToServer(PhotonNetwork.CurrentRoom.Name); // 채팅 서버 연결

        playManager.roomNameText.text = PhotonNetwork.CurrentRoom.Name; // 방 이름 설정
        UpdateOtherInfo();

        // 커스텀 품 프로퍼티 속성 설정 (게임 실행 중인지 아닌지)
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonManager.Instance.roomProperties["ing"] = false;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonManager.Instance.roomProperties);
        }

        if (PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer) // 방장이라면 게임시작 & 설정창 보이게
        {
            //settingButtonObj.SetActive(true);
            PhotonManager.Instance.playerProperties["ready"] = true;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "게임 시작";

            playManager.stateText.color = masterColor;
            playManager.stateText.text = "회의 위원장";

            playManager.readyButton.interactable = false;
        }
        else
        {
            //settingButtonObj.SetActive(false);
            PhotonManager.Instance.playerProperties["ready"] = false;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "게임 준비";

            playManager.stateText.color = nonReadyColor;
            playManager.stateText.text = "회의장으로 가는 중...";


            playManager.readyButton.interactable = true;
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

            playManager.stateText.color = readyColor;
            playManager.stateText.text = "입법 회의 준비 완료";
        }
        else // 레디 상태라면
        {
            PhotonManager.Instance.playerProperties["ready"] = false;
            readyOrStartTMP.text = "게임 준비";

            playManager.stateText.color = nonReadyColor;
            playManager.stateText.text = "회의장으로 가는 중...";
        }

        PhotonManager.Instance.SetCustomProperty();
    }

    public void StartGame() // 게임 시작
    {
        if (PhotonNetwork.MasterClient != PhotonNetwork.LocalPlayer) return; // 방장이 아니라면 반환

        playManager.readyButton.interactable = false;
        playManager.outButton.interactable = false;

        PickPosition(); // 역할 뽑기
    }

    private void PickPosition() // 역할 뽑기
    {
        // 방장이 아닌 다른 사용자 버튼도 비활성화
        // 룸 커스텀 프로퍼티에 겜 중이면 비활성화 되도록 지정
        PhotonManager.Instance.roomProperties["ing"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonManager.Instance.roomProperties);

        playManager.StateInitForGameStartRPC(); // 상태 지우기
        playManager.PickPosition(); // 역할 뽑기
        playManager.SendPickPositionRPC(); // 뽑은 거 알림
        playManager.ShowPositionRPC(); // 역할 보여주기
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
            playManager.readyButton.interactable = false;
            return;
        }

        playManager.readyButton.interactable = true;
    }

    
    // 레디 상태가 변했다면 state를 바꿔주기
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
        // 카드 비활성화 필요
        foreach(var card in playManager.cardDictionary)
        {
            card.Value.SetActive(false);
        }

        playManager.cardDictionary.Clear();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer) // 방장이라면 게임시작 & 설정창 보이게
        {
            //settingButtonObj.SetActive(true);
            PhotonManager.Instance.playerProperties["ready"] = true;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "게임 시작";

            playManager.stateText.color = masterColor;
            playManager.stateText.text = "회의 위원장";
            playManager.readyButton.interactable = false;
        }
        else
        {
            //settingButtonObj.SetActive(false);
            PhotonManager.Instance.playerProperties["ready"] = false;
            PhotonManager.Instance.SetCustomProperty();
            readyOrStartTMP.text = "게임 준비";

            playManager.stateText.color = nonReadyColor;
            playManager.stateText.text = "회의장으로 가는 중...";
        }
    }

    public void SendChattingMessage(string s) // 채팅 보내기
    {
        playManager.chatText.text = s;
        StartCoroutine(ShowMyChatImageAnim());
        PhotonChatManager.Instance.SendChat(s);
    }

    public void ShowChatImage(string name) // 채팅 이미지 보이기
    {
        if(name != DatabaseManager.Instance.name)
        {
            StartCoroutine(ShowChatImageAnim(name));
        }
    }

    private void InitChatObj() // 방에서 나갈 때 채팅 텍스트 초기화 & 이미지 비활성화
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

    // 커스텀 룸 프로퍼티 변경 시 호출
    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        PhotonHashtable hashTable = PhotonNetwork.CurrentRoom.CustomProperties;

        if ((bool)hashTable["ing"])
        {
            playManager.readyButton.interactable = false;
            playManager.outButton.interactable = false;
        }
        else if(!(bool)hashTable["ing"])
        {
            playManager.readyButton.interactable = true;
            playManager.outButton.interactable = true;
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
