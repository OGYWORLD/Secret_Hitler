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
        infoPanelTMP.text = "수정이 완료되었습니다.";

        DatabaseManager.Instance.UpdateUserData();

        PhotonManager.Instance.ChangeNickName();
    }

    protected override void FailCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "다시 시도해주세요.";
    }

    protected override void CheckEmailDuplication()
    {
        if (DatabaseManager.Instance.CheckEmailDuplication(emailInput.text)
            || nickNameInput.text.CompareTo(DatabaseManager.Instance.data.name) == 0)
        {
            isCheckEmailDP = true;

            dpCheckText.color = darkgreen;
            dpCheckText.text = "닉네임이 인증되었습니다.";

        }
        else
        {
            dpCheckText.color = darkred;
            dpCheckText.text = "중복된 닉네임입니다.";
        }
    }

    protected override void CheckNameDuplication()
    {
        if (DatabaseManager.Instance.CheckNickNameDuplication(nickNameInput.text)
            || emailInput.text.CompareTo(DatabaseManager.Instance.data.email) == 0)
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
}
