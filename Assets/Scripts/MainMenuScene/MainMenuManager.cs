using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public Button makeRoomButton;
    public Button settingButton;

    public GameObject settingPanel;
    public GameObject makeRoomPanel;

    private void Awake()
    {
        makeRoomButton.onClick.AddListener(ToMakeRoom);
        settingButton.onClick.AddListener(ToSetting);

        settingPanel.SetActive(false);
    }

    private void ToMakeRoom()
    {
        makeRoomPanel.SetActive(true);
    }

    private void ToSetting()
    {
        settingPanel.SetActive(true);
    }
}
