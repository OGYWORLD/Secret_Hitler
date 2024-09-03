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

    public Text[] otherNameTexts;
    public Text[] otherStateTexts;

    public Button readyButton;
    public Button outButton;
    public GameObject settingButtonObj;

    private TextMeshProUGUI readyOrStartTMP;

    private Color masterColor = new Color(1f, 1f, 144/255f);
    private Color readyColor = new Color(112f/255f, 1f, 249f/255f);
    private Color nonReadyColor = new Color(188/255f, 188 / 255f, 188 / 255f);

    private void Start()
    {
        outButton.onClick.AddListener(OnPlayerChoiceLeftRoom);
        readyButton.onClick.AddListener(SetReady);
        readyButton.onClick.AddListener(StartGame);

        readyOrStartTMP = readyButton.GetComponentInChildren<TextMeshProUGUI>();
        settingButtonObj.SetActive(false);
    }

    private void OnPlayerChoiceLeftRoom()
    {
        PhotonNetwork.LeaveRoom();
        PanelManager.Instance.InitPanel((int)Panel.enterDelayPanel);
    }

    public override void OnJoinedRoom() // 방 입장했을 시
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // 방 이름 설정
        UpdateOtherNames();

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
        UpdateOtherNames();

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CheckAllReady();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // 방에 누군가 떠났을 시
    {
        UpdateOtherNames();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CheckAllReady();
        }
    }

    public override void OnLeftRoom() // 방을 떠났을 시
    {
        PhotonManager.Instance.playerProperties["ready"] = false;
        PhotonManager.Instance.SetCustomProperty();
        NameStateListInit();
    }

    public void UpdateOtherNames() // 방에 있는 사람 목록 업데이트
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
        UpdateOtherNames();
        CheckAllReady();
    }

    public void GetCurrentUserInfo()
    {
        int index = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value != PhotonNetwork.LocalPlayer)
            {
                otherNameTexts[index].text = player.Value.NickName;

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

                index++;
            }
        }
    }

    private void NameStateListInit()
    {
        foreach (Text name in otherNameTexts)
        {
            name.text = "";
        }

        foreach (Text state in otherStateTexts)
        {
            state.text = "";
        }
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
}
