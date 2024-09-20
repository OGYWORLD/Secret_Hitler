using UnityEngine;
using UnityEngine.UI;

public class CRoomEdit : CRoomMake
{
    private string curRoomName;

    protected override void OnEnable()
    {
        lengthTextobj.SetActive(false);
        int curMax = PhotonManager.Instance.GetMaxPlayer();
        curRoomName = PhotonManager.Instance.GetCurrentRoomName();

        liberalCntTMP.text = PhotonManager.Instance.cntDictionary[curMax].Item1.ToString();
        pacistCntTMP.text = PhotonManager.Instance.cntDictionary[curMax].Item2.ToString();
        roomNameInputField.text = curRoomName;
        peopleNum.value = curMax;
    }

    protected override void OnMakeRoomButton()
    {
        if (!isLengthFine)
        {
            // 글자수 초과시 함수 반환
            ColorBlock colorBlock = roomNameInputField.colors;
            colorBlock.normalColor = Color.red;
            roomNameInputField.colors = colorBlock;

            StartCoroutine(SetColorToWhite());

            return;
        }

        if (roomNameInputField.text.Length == 0)
        {
            // 방이름을 지정해주세요
            ColorBlock colorBlock = roomNameInputField.colors;
            colorBlock.normalColor = Color.red;
            roomNameInputField.colors = colorBlock;

            StartCoroutine(SetColorToWhite());

            titleWarningText.text = "방 이름을 지정해주세요";
            lengthTextobj.SetActive(true);

            return;
        }
        else
        {
            string roomName;
            if (curRoomName.CompareTo(roomNameInputField.text) == 0) // 방이름이 동일하면 그냥 저장
            {
                roomName = roomNameInputField.text;
            }
            else
            {
                roomName = DPRoomNameCheck();
            }
            PhotonManager.Instance.CreateRoom(roomName, (peopleNum.value + 5));
        }
        SetHidePanel();
    }
}
