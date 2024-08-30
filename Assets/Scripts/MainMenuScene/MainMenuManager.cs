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
        settingButton.onClick.AddListener(ToSetting);

        settingPanel.SetActive(false);
    }

    private void ToTrain()
    {
        // TODO: 훈련장으로 이동
    }

    private void ToPVE()
    {
        
    }

    private void ToSetting()
    {
        settingPanel.SetActive(true);
    }
}
