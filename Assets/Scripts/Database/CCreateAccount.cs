using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* ȸ�������� �����ϴ� ��ũ��Ʈ �Դϴ�.*/
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

    protected Color darkgreen;
    protected Color darkred;

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

        darkgreen = new Color(18f / 255f, 166f / 255f, 104f / 255f);
        darkred = new Color(231f / 255f, 19f / 255f, 28f / 255f);
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
        if(emailInput.text.Length == 0)
        {
            dpCheckTMP.color = darkred;
            dpCheckTMP.text = "�̸��� ������ �����ּ���.";
        }
        else if (DatabaseManager.Instance.CheckEmailDuplication(emailInput.text))
        {
            isCheckEmailDP = true;

            dpCheckTMP.color = darkgreen;
            dpCheckTMP.text = "�̸����� �����Ǿ����ϴ�.";
            
        }
        else
        {
            dpCheckTMP.color = darkred;
            dpCheckTMP.text = "�ߺ��� �̸����Դϴ�.";
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
            pwCheckTMP.color = darkred;
            pwCheckTMP.text = "6�� �̻� �Է����ּ���.";
            isCheckPW = false;

            return;
        }

        if(pwInput.text.CompareTo(pwCheckInput.text) == 0)
        {
            pwCheckTMP.color = darkgreen;
            pwCheckTMP.text = "��й�ȣ�� �����մϴ�.";
            isCheckPW = true;
        }
        else
        {
            pwCheckTMP.color = darkred;
            pwCheckTMP.text = "��й�ȣ�� �ٸ��ϴ�.";
            isCheckPW = false;
        }
    }

    protected virtual void SuccessCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "��� ���� �Ϸ�";

        isCheckEmailDP = false;
    }

    protected virtual void FailCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "��� ���� ����";
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
            nameCheckTMP.color = darkred;
            nameCheckTMP.text = $"�ִ� ���ڼ��� {maxNameLength}�Դϴ�.";
        }
        else if(s.Length < minNameLength)
        {
            isNameLength = false;
            nameCheckTMP.color = darkred;
            nameCheckTMP.text = $"�ּ� ���ڼ��� {minNameLength}�Դϴ�.";
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
            colorBlock.normalColor = darkred;
            dpCheckButton.colors = colorBlock;
        }

        if (!isCheckPW)
        {
            ColorBlock colorBlock = pwInput.colors;
            colorBlock.normalColor = darkred;
            pwInput.colors = colorBlock;
        }

        if (!isNameLength)
        {
            ColorBlock colorBlock = nickNameInput.colors;
            colorBlock.normalColor = darkred;
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
