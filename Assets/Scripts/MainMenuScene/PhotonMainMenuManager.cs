using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonMainMenuManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> roomCards = new List<GameObject>();

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
