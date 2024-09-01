using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CRoomMake : MonoBehaviour
{
    public TMP_InputField roomNameTMP;

    public TextMeshProUGUI liberalCnt;
    public TextMeshProUGUI pacistCnt;

    public TMP_Dropdown peopleNum;

    public Button makeRoomButton;

    //Key: �ο���, Value: Liberal, Pacist
    private Dictionary<int, (int, int)> cntDictionary = new Dictionary<int, (int, int)>(); // �ο����� ���� ������ �Ľý�Ʈ �ο���

    private void Awake()
    {
        peopleNum.onValueChanged.AddListener(OnValueChange);
        makeRoomButton.onClick.AddListener(OnMakeRoomButton);

        // �ο��� ��ųʸ� �ʱ�ȭ
        cntDictionary[5] = (3, 2);
        cntDictionary[6] = (4, 2);
        cntDictionary[7] = (4, 3);
        cntDictionary[8] = (5, 3);
        cntDictionary[9] = (5, 4);
        cntDictionary[10] = (6, 4);
    }

    private void OnValueChange(int n)
    {
        liberalCnt.text = cntDictionary[n+5].Item1.ToString();
        pacistCnt.text = cntDictionary[n+5].Item2.ToString();
    }

    private void OnMakeRoomButton()
    {
        PhotonManager.Instance.JoinOrCreateRoom();
    }
}
