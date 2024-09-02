using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    public Button speedRoomButton;
    public Button makeRoomButton;
    public Button settingButton;

    public GameObject settingPanel;
    public GameObject makeRoomPanel;

    public List<GameObject> roomCards = new List<GameObject>();

    private void Awake()
    {
        makeRoomButton.onClick.AddListener(ToMakeRoom);
        settingButton.onClick.AddListener(ToSetting);
        speedRoomButton.onClick.AddListener(OnSpeedRoom);

        settingPanel.SetActive(false);
    }

    private void ToMakeRoom()
    {
        makeRoomPanel.SetActive(true);
    }

    private void ToSetting()
    {
        settingPanel.SetActive(true);
    }

    private void OnSpeedRoom()
    {
        PhotonManager.Instance.JoinOrCreateRoom("빠른 시작 해요~", 5);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomCardInit(); // 방 목록 초기화

        int index = 0;
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) // 방 삭제 되면 방카드 비활성화 후 방카드 인덱스 조정
            {
                continue;
            }
            else // 방 생성되면 방카드 활성화 후 방카드 인덱스 조정
            {
                // 방 정보 수정
                CRoomCardInfo cardInfo = roomCards[index].GetComponent<CRoomCardInfo>();
                cardInfo.roomNameText.text = room.Name;
                cardInfo.maxPeopleNum.text = room.MaxPlayers.ToString();
                cardInfo.curPeopleNum.text = room.PlayerCount.ToString();
                cardInfo.liberalNum.text = PhotonManager.Instance.cntDictionary[room.MaxPlayers].Item1.ToString();
                cardInfo.pacistNum.text = PhotonManager.Instance.cntDictionary[room.MaxPlayers].Item2.ToString();

                roomCards[index].SetActive(true);
                index++;
            }
        }
    }

    private void RoomCardInit()
    {
        foreach (GameObject room in roomCards)
        {
            room.SetActive(false);
        }
    }
}
