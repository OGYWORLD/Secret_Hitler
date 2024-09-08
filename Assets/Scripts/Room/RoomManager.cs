﻿using Photon.Pun;
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

    private int maxPlayer; // 모든 인원에 대한 커스텀 플레이어 프로퍼티 수정 콜백함수가 호출됐는지 카운트하기 위한 변수

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

        playManager.InitWhenJoinedRoom(); // 오브젝트 초기화

        // 버튼들 활성화
        playManager.readyButton.gameObject.SetActive(true);
        playManager.outButton.gameObject.SetActive(true);

        // 커스텀 품 프로퍼티 속성 설정 (게임 실행 중인지 아닌지)
        if (PhotonNetwork.IsMasterClient)
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

        if((bool)PhotonNetwork.CurrentRoom.CustomProperties["ing"]) // 게임 중에 방을 떠나면 게임 종료
        {
            if(PhotonNetwork.IsMasterClient) // 모두 내보냄
            {
                playManager.KickAllPlayerRPC();
            }
        }

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

        // 버튼 비활성화
        playManager.readyButton.gameObject.SetActive(false);
        playManager.outButton.gameObject.SetActive(false);

        playManager.SufflePolicy(); // 정책카드 섞기
        playManager.PassSufflePolicyRPC(playManager.policyArray); // 섞은 거 RPC로 전달
        playManager.SetPlayerOrder(); // 플레이 순서 정하기
        playManager.PassPlayerOrderRPC(playManager.playerOrder); // 순서 모두에게 저장
        PickPosition(); // 역할 뽑기
    }

    private void PickPosition() // 역할 뽑기
    {
        // 방장이 아닌 다른 사용자 버튼도 비활성화
        // 룸 커스텀 프로퍼티에 겜 중이면 비활성화 되도록 지정
        PhotonManager.Instance.roomProperties["ing"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonManager.Instance.roomProperties);

        playManager.PickPosition(); // 역할 뽑기
        playManager.SetPlayerCustomForPosition(); // 뽑은 역할 프로퍼티 저장
        playManager.StateInitForGameStartRPC(); // 상태 지우기
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

    public override void OnMasterClientSwitched(Player newMasterClient) // 마스터 클라이언트가 변경되었다면
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

    // 플레이어 커스텀 프로퍼티가 변경됐다면 호출되는 콜백함수 ####################
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("ready") && 
            !(bool)PhotonNetwork.CurrentRoom.CustomProperties["ing"]) // 레디 상태가 변했다면 state를 바꿔주기
        {
            UpdateOtherInfo();
            CheckAllReady();
        }

        if (changedProps.ContainsKey("position")) // 역할이 정해졌다면 포지션 카드 스프라이트 변경
        {
            maxPlayer++;
            playManager.SetPositionCard(targetPlayer);
        }

        // 해당 조건문은 게임 시작 후 역할 선정 후 최초이자 마지막으로 호출됩니다.
        if (maxPlayer == PhotonNetwork.CurrentRoom.MaxPlayers) // 모든 플레이어의 포지션이 정해졌다면
        {
            maxPlayer = 0;

            // 첫 번째 대통령 지정
            PhotonManager.Instance.roomProperties["president"] = PhotonNetwork.CurrentRoom.Players[playManager.playerOrder[0]];
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonManager.Instance.roomProperties);

            // 현재 순서 저장 (currentOrder 저장)
            PhotonManager.Instance.roomProperties["currentOrder"] = 0;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonManager.Instance.roomProperties);

            playManager.ShowPositionRPC(); // 역할 보여주기
        }
    }

    // 커스텀 룸 프로퍼티 변경 시 호출 #####################################
    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        PhotonHashtable hashTable = PhotonNetwork.CurrentRoom.CustomProperties;

        // 레디 상태 여부에 따른 제목, 레디 버튼, 나가기 버튼 활성화 설정
        if ((bool)hashTable["ing"]) // 현재 게임 중이라면
        {
            playManager.roomNameText.text = "";
            playManager.readyButton.gameObject.SetActive(false);
            playManager.outButton.gameObject.SetActive(false);
        }
        else if(!(bool)hashTable["ing"]) // 현재 게임 중이 아니라면
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
