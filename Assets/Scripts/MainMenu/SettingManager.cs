using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    public Button closeButton;
    public Button accountButton;

    public GameObject mainPanel;
    public GameObject accountEditPanel;

    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseSetting);
        accountButton.onClick.AddListener(ToAccountEdit);
    }

    private void OnEnable()
    {
        mainPanel.SetActive(true);
        accountEditPanel.SetActive(false);
    }

    private void OnCloseSetting()
    {
        gameObject.SetActive(false);
    }

    private void ToAccountEdit()
    {
        mainPanel.SetActive(false);
        accountEditPanel.SetActive(true);
    }
}
