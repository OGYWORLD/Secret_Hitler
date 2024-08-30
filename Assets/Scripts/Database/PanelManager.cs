using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*시작화면의 로그인 패널과 계정 생성 패널을 관리하는 패널 매니저입니다.
 */

public enum Panel
{
    introPanel,
    loginPanel,
    createAccountPanel,
    mainMenuPanel,
    matchingPanel
}

public delegate void ToMainMenu();

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    public GameObject[] panels;

    public ToMainMenu toMainMenu;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitPanel((int)Panel.introPanel);
    }

    public void InitPanel(int whichPanel)
    {
        foreach(GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        panels[whichPanel].SetActive(true);
    }
}
