using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CRoom : MonoBehaviour
{
    public Text nameText;

    public Button settingButton;
    public GameObject settingPanel;

    private void Start()
    {
        settingButton.onClick.AddListener(OnOpenSettingPanel);
        nameText.text = DatabaseManager.Instance.data.name; // �÷��̾� �̸� ���
    }

    private void OnOpenSettingPanel()
    {
        settingPanel.SetActive(true);
    }
}
