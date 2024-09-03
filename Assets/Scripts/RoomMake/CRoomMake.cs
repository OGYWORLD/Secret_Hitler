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
    public Button cancelButton;

    private void Awake()
    {
        peopleNum.onValueChanged.AddListener(OnValueChange);
        makeRoomButton.onClick.AddListener(OnMakeRoomButton);
        cancelButton.onClick.AddListener(SetHidePanel);
        roomNameInputField.onValueChanged.AddListener(OnNameChange);
    }

    private void OnEnable()
    {
        liberalCnt.text = PhotonManager.Instance.cntDictionary[5].Item1.ToString();
        pacistCnt.text = PhotonManager.Instance.cntDictionary[5].Item2.ToString(); ;
        roomNameInputField.text = "";
        peopleNum.value = 0;
    }

    private void OnNameChange(string name)
    {

    }

    private void OnValueChange(int n)
    {
        liberalCnt.text = PhotonManager.Instance.cntDictionary[n+5].Item1.ToString();
        pacistCnt.text = PhotonManager.Instance.cntDictionary[n+5].Item2.ToString();
    }

    private void OnMakeRoomButton()
    {
        if(roomNameInputField.text.Length == 0)
        {
           // 방이름을 지정해주세요
        }
        else
        {
            PhotonManager.Instance.CreateRoom(roomNameInputField.text, (peopleNum.value + 5));
        }
        SetHidePanel();
    }

    private void SetHidePanel()
    {
        gameObject.SetActive(false);
    }
}
