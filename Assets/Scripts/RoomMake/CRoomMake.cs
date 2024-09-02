using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CRoomMake : MonoBehaviour
{
    public InputField roomNameInputField;

    public TextMeshProUGUI liberalCnt;
    public TextMeshProUGUI pacistCnt;

    public TMP_Dropdown peopleNum;

    public Button makeRoomButton;

    private void Awake()
    {
        peopleNum.onValueChanged.AddListener(OnValueChange);
        makeRoomButton.onClick.AddListener(OnMakeRoomButton);
    }

    private void OnValueChange(int n)
    {
        liberalCnt.text = PhotonManager.Instance.cntDictionary[n+5].Item1.ToString();
        pacistCnt.text = PhotonManager.Instance.cntDictionary[n+5].Item2.ToString();
    }

    private void OnMakeRoomButton()
    {
        PhotonManager.Instance.CreateRoom(roomNameInputField.text, (peopleNum.value+5));
    }
}
