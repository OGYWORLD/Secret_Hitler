using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 회원가입을 수행하는 스크립트 입니다.*/
public class CCreateAccount : MonoBehaviour
{
    public InputField emailInput;
    public InputField pwInput;
    public InputField pwCheckInput;
    public InputField nickNameInput;

    public Button emailDPCheckButton;
    public Button nickNameDPCheckButton;
    public Button createButton;
    public Button homeButton;
    public Button infoPanelButton;

    public Text dpCheckText;
    public Text pwCheckText;
    public Text nameCheckText;
    public Text nameDPCheckText;

    public GameObject infoPanel;

    protected TextMeshProUGUI infoPanelTMP;

    protected bool isCheckEmailDP;
    protected bool isCheckPW;
    protected bool isNameLength;
    protected bool isCheckNameDP;

    protected int maxNameLength = 7;
    protected int minNameLength = 1;
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
        emailDPCheckButton.onClick.AddListener(CheckEmailDuplication);
        createButton.onClick.AddListener(OnCreateAccount);
        nickNameDPCheckButton.onClick.AddListener(CheckNameDuplication);

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

        dpCheckText.text = "";
        pwCheckText.text = "";
        nameCheckText.text = "";
        nameDPCheckText.text = "";

        isCheckEmailDP = false;
        isNameLength = false;
        isCheckPW = false;

        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.paperSF);
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
            dpCheckText.color = darkred;
            dpCheckText.text = "이메일 형식을 지켜주세요.";
        }
        else if (DatabaseManager.Instance.CheckEmailDuplication(emailInput.text))
        {
            isCheckEmailDP = true;

            dpCheckText.color = darkgreen;
            dpCheckText.text = "이메일이 인증되었습니다.";
            
        }
        else
        {
            isCheckEmailDP = false; // 추가함

            dpCheckText.color = darkred;
            dpCheckText.text = "중복된 이메일입니다.";
        }
    }

    protected virtual void CheckNameDuplication()
    {
        if (DatabaseManager.Instance.CheckNickNameDuplication(nickNameInput.text))
        {
            isCheckNameDP = true;

            nameDPCheckText.color = darkgreen;
            nameDPCheckText.text = "닉네임이 인증되었습니다.";

        }
        else
        {
            isCheckNameDP = false;

            nameDPCheckText.color = darkred;
            nameDPCheckText.text = "중복된 닉네임입니다.";
        }
    }

    protected void OnEmailValueChanged(string s)
    {
        isCheckEmailDP = false;
        dpCheckText.text = "";
    }

    protected void OnCheckPWSame(string s)
    {
        if(pwInput.text.Length < minPWLength)
        {
            pwCheckText.color = darkred;
            pwCheckText.text = "6자 이상 입력해주세요.";
            isCheckPW = false;

            return;
        }

        if(pwInput.text.CompareTo(pwCheckInput.text) == 0)
        {
            pwCheckText.color = darkgreen;
            pwCheckText.text = "비밀번호가 동일합니다.";
            isCheckPW = true;
        }
        else
        {
            pwCheckText.color = darkred;
            pwCheckText.text = "비밀번호가 다릅니다.";
            isCheckPW = false;
        }
    }

    protected virtual void SuccessCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "당원 가입 완료";

        isCheckEmailDP = false;
    }

    protected virtual void FailCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "당원 가입 실패";
    }

    protected void OnClosedInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    protected void OnNameLengthCheck(string s)
    {
        isCheckNameDP = false;
        nameDPCheckText.text = "";

        if (s.Length > maxNameLength)
        {
            isNameLength = false;
            nameCheckText.color = darkred;
            nameCheckText.text = $"최대 글자수는 {maxNameLength}입니다.";
        }
        else if(s.Length < minNameLength)
        {
            isNameLength = false;
            nameCheckText.color = darkred;
            nameCheckText.text = $"최소 글자수는 {minNameLength}입니다.";
        }
        else
        {
            isNameLength = true;
            nameCheckText.text = "";
        }
    }

    protected bool CheckValid()
    {
        if (!isCheckEmailDP)
        {
            ColorBlock colorBlock = emailDPCheckButton.colors;
            colorBlock.normalColor = darkred;
            emailDPCheckButton.colors = colorBlock;
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

        if(!isCheckNameDP)
        {
            ColorBlock colorBlock = nickNameDPCheckButton.colors;
            colorBlock.normalColor = darkred;
            nickNameDPCheckButton.colors = colorBlock;
        }

        if (!isCheckEmailDP || !isCheckPW || !isNameLength || !isCheckNameDP)
        {
            StartCoroutine(InitColor());
            return false;
        }

        return true;
    }

    protected IEnumerator InitColor()
    {
        yield return new WaitForSeconds(0.5f);

        ColorBlock colorBlock = emailDPCheckButton.colors;
        colorBlock.normalColor = Color.white;

        emailDPCheckButton.colors = colorBlock;
        pwInput.colors = colorBlock;
        nickNameInput.colors = colorBlock;
        nickNameDPCheckButton.colors = colorBlock;
    }
}
