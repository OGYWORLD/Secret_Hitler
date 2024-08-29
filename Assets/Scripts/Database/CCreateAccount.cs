using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 회원가입을 수행하는 스크립트 입니다.*/
public class CCreateAccount : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField pwInput;
    public TMP_InputField pwCheckInput;
    public TMP_InputField nickNameInput;

    public Button dpCheckButton;
    public Button createButton;
    public Button homeButton;
    public Button infoPanelButton;

    public TextMeshProUGUI dpCheckTMP;
    public TextMeshProUGUI pwCheckTMP;
    public TextMeshProUGUI nameCheckTMP;

    public GameObject infoPanel;

    protected TextMeshProUGUI infoPanelTMP;

    protected bool isCheckEmailDP;
    protected bool isCheckPW;
    protected bool isNameLength;

    protected int maxNameLength = 50;
    protected int minNameLength = 3;
    protected int minPWLength = 6;

    protected void Awake()
    {
        emailInput.onValueChanged.AddListener(OnEmailValueChanged);
        pwInput.onValueChanged.AddListener(OnCheckPWSame);
        pwCheckInput.onValueChanged.AddListener(OnCheckPWSame);
        nickNameInput.onValueChanged.AddListener(OnNameLengthCheck);

        homeButton.onClick.AddListener(OnToHomeButton);
        dpCheckButton.onClick.AddListener(CheckEmailDuplication);
        createButton.onClick.AddListener(OnCreateAccount);

        infoPanelButton.onClick.AddListener(OnClosedInfoPanel);

        infoPanelTMP = infoPanel?.GetComponentInChildren<TextMeshProUGUI>();
        infoPanel.SetActive(false);
    }

    private void OnEnable()
    {
        emailInput.text = "";
        nickNameInput.text = "";
        pwInput.text = "";
        pwCheckInput.text = "";

        dpCheckTMP.text = "";
        pwCheckTMP.text = "";
        nameCheckTMP.text = "";

        isCheckEmailDP = false;
        isNameLength = false;
        isCheckPW = false;
    }

    protected virtual void OnCreateAccount()
    {
        if(CheckValid())
        {
            DatabaseManager.Instance.CreateAccount(emailInput.text, pwInput.text, nickNameInput.text, SuccessCreate, FailCreate);
        }
    }

    protected void OnToHomeButton()
    {
        PanelManager.Instance.InitPanel((int)Panel.loginPanel);
    }

    protected virtual void CheckEmailDuplication()
    {
        if (DatabaseManager.Instance.CheckEmailDuplication(emailInput.text))
        {
            isCheckEmailDP = true;

            dpCheckTMP.color = Color.green;
            dpCheckTMP.text = "Verified Duplication Email";
            
        }
        else
        {
            dpCheckTMP.color = Color.red;
            dpCheckTMP.text = "Email is Duplicated";
        }
    }

    protected void OnEmailValueChanged(string s)
    {
        isCheckEmailDP = false;
        dpCheckTMP.text = "";
    }

    protected void OnCheckPWSame(string s)
    {
        if(pwInput.text.Length < minPWLength)
        {
            pwCheckTMP.color = Color.red;
            pwCheckTMP.text = "more 6 dixit.";
            isCheckPW = false;

            return;
        }

        if(pwInput.text.CompareTo(pwCheckInput.text) == 0)
        {
            pwCheckTMP.color = Color.green;
            pwCheckTMP.text = "Password is Same.";
            isCheckPW = true;
        }
        else
        {
            pwCheckTMP.color = Color.red;
            pwCheckTMP.text = "Password is Different.";
            isCheckPW = false;
        }
    }

    protected virtual void SuccessCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "Create Success!";

        isCheckEmailDP = false;
    }

    protected virtual void FailCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "Create Fail. Try Again.";
    }

    protected void OnClosedInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    protected void OnNameLengthCheck(string s)
    {
        if(s.Length > maxNameLength)
        {
            isNameLength = false;
            nameCheckTMP.color = Color.red;
            nameCheckTMP.text = $"Max Length is {maxNameLength}";
        }
        else if(s.Length < minNameLength)
        {
            isNameLength = false;
            nameCheckTMP.color = Color.red;
            nameCheckTMP.text = $"Min Length is {minNameLength}";
        }
        else
        {
            isNameLength = true;
            nameCheckTMP.text = "";
        }
    }

    protected bool CheckValid()
    {
        if (!isCheckEmailDP)
        {
            ColorBlock colorBlock = dpCheckButton.colors;
            colorBlock.normalColor = Color.red;
            dpCheckButton.colors = colorBlock;
        }

        if (!isCheckPW)
        {
            ColorBlock colorBlock = pwInput.colors;
            colorBlock.normalColor = Color.red;
            pwInput.colors = colorBlock;
        }

        if (!isNameLength)
        {
            ColorBlock colorBlock = nickNameInput.colors;
            colorBlock.normalColor = Color.red;
            nickNameInput.colors = colorBlock;
        }

        if (!isCheckEmailDP || !isCheckPW || !isNameLength)
        {
            StartCoroutine(InitColor());
            return false;
        }

        return true;
    }

    protected IEnumerator InitColor()
    {
        yield return new WaitForSeconds(0.5f);

        ColorBlock colorBlock = dpCheckButton.colors;
        colorBlock.normalColor = Color.white;
        dpCheckButton.colors = colorBlock;
        pwInput.colors = colorBlock;
        nickNameInput.colors = colorBlock;
    }
}
