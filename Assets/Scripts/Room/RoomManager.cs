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

    public override void OnJoinedRoom() // �� �������� ��
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // �� �̸� ����
        UpdateOtherNames();

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
        UpdateOtherNames();

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CheckAllReady();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) // �濡 ������ ������ ��
    {
        UpdateOtherNames();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CheckAllReady();
        }
    }

    public override void OnLeftRoom() // ���� ������ ��
    {
        PhotonManager.Instance.playerProperties["ready"] = false;
        PhotonManager.Instance.SetCustomProperty();
        NameStateListInit();
    }

    public void UpdateOtherNames() // �濡 �ִ� ��� ��� ������Ʈ
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
}
