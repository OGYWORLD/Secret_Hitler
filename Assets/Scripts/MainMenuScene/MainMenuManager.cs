using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public Button trainButton;
    public Button pveButton;
    public Button settingButton;

    public GameObject settingPanel;

    private void Awake()
    {
        trainButton.onClick.AddListener(ToTrain);
        pveButton.onClick.AddListener(ToPVE);
        settingButton.onClick.AddListener(ToSetting);

        settingPanel.SetActive(false);
    }

    private void ToTrain()
    {
        // TODO: 훈련장으로 이동
    }

    private void ToPVE()
    {
        PanelManager.Instance.InitPanel((int)Panel.matchingPanel);
    }

    private void ToSetting()
    {
        settingPanel.SetActive(true);
    }
}
