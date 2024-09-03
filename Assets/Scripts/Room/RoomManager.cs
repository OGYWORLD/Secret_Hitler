using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public Text roomNameText;
    public Text stateText;

    public Text[] otherNameTexts;
    public HashSet<string> names = new HashSet<string>();

    public Button readyButton;
    public Button outButton;

    private void Start()
    {
        outButton.onClick.AddListener(OnPlayerChoiceLeftRoom);
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

        PanelManager.Instance.InitPanel((int)Panel.roomPanel);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // 방에 누군가 들어왔을 시
    {
        names.Add(newPlayer.NickName);
        UpdateOtherNames();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // 방에 누군가 떠났을 시
    {
        names.Remove(otherPlayer.NickName);
        UpdateOtherNames();
    }

    public override void OnLeftRoom() // 방을 떠났을 시
    {
        names.Clear();
    }

    public void UpdateOtherNames() // 방에 있는 사람 목록 업데이트
    {
        NameListInit();
        GetCurrentUser();
    }

    public void SetState() // TODO: Ready 설정
    {

    }

    public void GetCurrentUser()
    {
        int index = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value != PhotonNetwork.LocalPlayer)
            {
                otherNameTexts[index].text = player.Value.NickName;
                index++;
            }
        }
    }

    private void NameListInit()
    {
        foreach (Text name in otherNameTexts)
        {
            name.text = "";
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {

        }
    }
}
