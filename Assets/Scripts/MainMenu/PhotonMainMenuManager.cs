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

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) // 방 삭제 되면 방카드 비활성화 후 방카드 인덱스 조정
            {
                PhotonManager.Instance.roomList.Remove(room);
            }
            else
            {
                if(!PhotonManager.Instance.roomList.Contains(room))
                {
                    PhotonManager.Instance.roomList.Add(room); // 새로 추가 된 List에 방 추가
                }
                else // 이전에 존재한 방인데 옵션이 바뀔 경우 List 삭제 후 새로 삽입
                {
                    PhotonManager.Instance.roomList.Remove(room);
                    PhotonManager.Instance.roomList.Add(room);
                }
            }
        }

        int index = 0;
        foreach(RoomInfo room in PhotonManager.Instance.roomList)
        {
            // TODO: 게임이 시작된 방이면 목록에서 안 보이게 (중도 탈주 고려)
            if (room.MaxPlayers != room.PlayerCount) // 방이 만원이면 목록에서 안 보이게
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
