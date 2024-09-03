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

    public override void OnJoinedRoom() // �� �������� ��
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // �� �̸� ����
        UpdateOtherNames();

        PanelManager.Instance.InitPanel((int)Panel.roomPanel);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // �濡 ������ ������ ��
    {
        names.Add(newPlayer.NickName);
        UpdateOtherNames();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // �濡 ������ ������ ��
    {
        names.Remove(otherPlayer.NickName);
        UpdateOtherNames();
    }

    public override void OnLeftRoom() // ���� ������ ��
    {
        names.Clear();
    }

    public void UpdateOtherNames() // �濡 �ִ� ��� ��� ������Ʈ
    {
        NameListInit();
        GetCurrentUser();
    }

    public void SetState() // TODO: Ready ����
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
