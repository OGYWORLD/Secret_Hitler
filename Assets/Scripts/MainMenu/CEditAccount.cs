using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEditAccount : CCreateAccount
{
    private void OnEnable()
    {
        emailInput.text = DatabaseManager.Instance.data.email;
        nickNameInput.text = DatabaseManager.Instance.data.name;
        pwInput.text = "";
        pwCheckInput.text = "";

        nameDPCheckText.text = "";
        dpCheckText.text = "";
        pwCheckText.text = "";
        nameCheckText.text = "";

        isCheckNameDP = true;
        isNameLength = true;
        isCheckEmailDP = true;
        isNameLength = true;
    }

    protected override void OnCreateAccount()
    {
        if(CheckValid())
        {
            DatabaseManager.Instance.EditAccount(emailInput.text, pwInput.text, nickNameInput.text, SuccessCreate, FailCreate);
        }
    }

    protected override void SuccessCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "������ �Ϸ�Ǿ����ϴ�.";

        DatabaseManager.Instance.UpdateUserData();

        PhotonManager.Instance.ChangeNickName();
    }

    protected override void FailCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "�ٽ� �õ����ּ���.";
    }

    protected override void CheckEmailDuplication()
    {
        if (DatabaseManager.Instance.CheckEmailDuplication(emailInput.text)
            || nickNameInput.text.CompareTo(DatabaseManager.Instance.data.name) == 0)
        {
            isCheckEmailDP = true;

            dpCheckText.color = darkgreen;
            dpCheckText.text = "�г����� �����Ǿ����ϴ�.";

        }
        else
        {
            dpCheckText.color = darkred;
            dpCheckText.text = "�ߺ��� �г����Դϴ�.";
        }
    }

    protected override void CheckNameDuplication()
    {
        if (DatabaseManager.Instance.CheckNickNameDuplication(nickNameInput.text)
            || emailInput.text.CompareTo(DatabaseManager.Instance.data.email) == 0)
        {
            isCheckNameDP = true;

            nameDPCheckText.color = darkgreen;
            nameDPCheckText.text = "�г����� �����Ǿ����ϴ�.";

        }
        else
        {
            isCheckNameDP = false;

            nameDPCheckText.color = darkred;
            nameDPCheckText.text = "�ߺ��� �г����Դϴ�.";
        }
    }
}
