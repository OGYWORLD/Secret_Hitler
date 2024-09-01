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

        dpCheckTMP.text = "";
        pwCheckTMP.text = "";
        nameCheckTMP.text = "";

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
        infoPanelTMP.text = "수정이 완료되었습니다.";

        DatabaseManager.Instance.UpdateUserData();
    }

    protected override void FailCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "다시 시도해주세요.";
    }

    protected override void CheckEmailDuplication()
    {
        if (DatabaseManager.Instance.CheckEmailDuplication(emailInput.text)
            || emailInput.text.CompareTo(DatabaseManager.Instance.data.email) == 0)
        {
            isCheckEmailDP = true;

            dpCheckTMP.color = darkgreen;
            dpCheckTMP.text = "이메일이 인증되었습니다.";

        }
        else
        {
            dpCheckTMP.color = darkred;
            dpCheckTMP.text = "중복된 이메일입니다.";
        }
    }
}
