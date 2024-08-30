using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CCharaInfo : MonoBehaviour
{
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI level;

    public Image expBar;

    private void Start()
    {
        DatabaseManager.Instance.changeUserInfo += PrintUserInfo;
        PrintUserInfo();
    }

    private void PrintUserInfo()
    {
        if(DatabaseManager.Instance.data is not null)
        {
            nickname.text = DatabaseManager.Instance.data.name;
            level.text = (DatabaseManager.Instance.data.EXP / 100 + 1).ToString();

            // TODO: fill amount 맞게 계산했는지 추후 경험치 얻는 로직 넣고 확인
            expBar.fillAmount = (DatabaseManager.Instance.data.EXP % 100) * 0.001f;
        }
    }
}
