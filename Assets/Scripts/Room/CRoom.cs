using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CRoom : MonoBehaviourPunCallbacks
{
    public Text roomNameText;
    public Text nameText;
    public Text stateText;

    public Text[] otherNameTexts;
    public HashSet<string> names = new HashSet<string>();

    public Button readyButton;

    private void Start()
    {
        nameText.text = DatabaseManager.Instance.data.name;
    }

    public override void OnJoinedRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        UpdateOtherNames();

        PanelManager.Instance.InitPanel((int)Panel.roomPanel);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        names.Add(newPlayer.NickName);
        UpdateOtherNames();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        names.Remove(otherPlayer.NickName);
        UpdateOtherNames();
    }

    public override void OnLeftRoom()
    {
        names.Clear();
    }

    public void UpdateOtherNames()
    {
        NameListInit();
        GetCurrentUser();
    }

    public void SetState()
    {
        
    }

    public void GetCurrentUser()
    {
        print("이름 업데이트");
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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {

        }
    }

    private void NameListInit()
    {
        foreach(Text name in otherNameTexts)
        {
            name.text = "";
        }
    }
}
