using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CRoomMake : MonoBehaviour
{
    public InputField roomNameInputField;

    public TextMeshProUGUI liberalCntTMP;
    public TextMeshProUGUI pacistCntTMP;

    public GameObject lengthTextobj;

    public TMP_Dropdown peopleNum;

    public Button makeRoomButton;
    public Button cancelButton;

    protected Text titleWarningText;

    protected int maxLenght = 19;
    protected bool isLengthFine = true;

    protected void Awake()
    {
        peopleNum.onValueChanged.AddListener(OnValueChange);
        makeRoomButton.onClick.AddListener(OnMakeRoomButton);
        cancelButton.onClick.AddListener(SetHidePanel);
        roomNameInputField.onValueChanged.AddListener(OnNameChange);

        titleWarningText = lengthTextobj.GetComponent<Text>();
    }

    protected virtual void OnEnable()
    {
        lengthTextobj.SetActive(false);
        liberalCntTMP.text = PhotonManager.Instance.cntDictionary[5].Item1.ToString();
        pacistCntTMP.text = PhotonManager.Instance.cntDictionary[5].Item2.ToString();
        roomNameInputField.text = "";
        peopleNum.value = 0;
    }

    protected void OnNameChange(string name) // 방 제목 글자수 제한
    {
        if(name.Length > maxLenght)
        {
            titleWarningText.text = $"최대 글자수는 {maxLenght}자입니다";
            lengthTextobj.SetActive(true);
            isLengthFine = false;
        }
        else
        {
            lengthTextobj.SetActive(false);
            isLengthFine = true;
        }
    }

    protected void OnValueChange(int n)
    {
        liberalCntTMP.text = PhotonManager.Instance.cntDictionary[n+5].Item1.ToString();
        pacistCntTMP.text = PhotonManager.Instance.cntDictionary[n+5].Item2.ToString();
    }

    protected virtual void OnMakeRoomButton()
    {
        if(!isLengthFine)
        {
            // 글자수 초과시 함수 반환
            ColorBlock colorBlock = roomNameInputField.colors;
            colorBlock.normalColor = Color.red;
            roomNameInputField.colors = colorBlock;

            StartCoroutine(SetColorToWhite());

            return;
        }

        if(roomNameInputField.text.Length == 0)
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
            string roomName = DPRoomNameCheck();
            PhotonManager.Instance.CreateRoom(roomName, (peopleNum.value + 5));
        }
        SetHidePanel();
    }

    protected void SetHidePanel()
    {
        gameObject.SetActive(false);
    }

    protected string DPRoomNameCheck()
    {
        int dpCnt = PhotonManager.Instance.CheckDPRoomName(roomNameInputField.text);
        if (PhotonManager.Instance.CheckDPRoomName(roomNameInputField.text) == 0)
        {
            return roomNameInputField.text;
        }
        else
        {
            return $"{roomNameInputField.text} ({dpCnt})";
        }
    }

    protected IEnumerator SetColorToWhite()
    {
        yield return new WaitForSeconds(0.5f);

        ColorBlock colorBlock = roomNameInputField.colors;
        colorBlock.normalColor = Color.white;
        roomNameInputField.colors = colorBlock;
    }
}
