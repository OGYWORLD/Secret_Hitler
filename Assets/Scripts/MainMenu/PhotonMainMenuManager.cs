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
        RoomCardInit(); // �� ��� �ʱ�ȭ

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList) // �� ���� �Ǹ� ��ī�� ��Ȱ��ȭ �� ��ī�� �ε��� ����
            {
                PhotonManager.Instance.roomList.Remove(room);
            }
            else
            {
                if(!PhotonManager.Instance.roomList.Contains(room))
                {
                    PhotonManager.Instance.roomList.Add(room); // ���� �߰� �� List�� �� �߰�
                }
                else // ������ ������ ���ε� �ɼ��� �ٲ� ��� List ���� �� ���� ����
                {
                    PhotonManager.Instance.roomList.Remove(room);
                    PhotonManager.Instance.roomList.Add(room);
                }
            }
        }

        int index = 0;
        foreach(RoomInfo room in PhotonManager.Instance.roomList)
        {
            // TODO: ������ ���۵� ���̸� ��Ͽ��� �� ���̰� (�ߵ� Ż�� ���)
            if (room.MaxPlayers != room.PlayerCount) // ���� �����̸� ��Ͽ��� �� ���̰�
            {
                // �� ���� ����
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
