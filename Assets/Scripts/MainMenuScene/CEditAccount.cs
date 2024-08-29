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
        infoPanelTMP.text = "Edit Success!";

        DatabaseManager.Instance.UpdateUserData();
    }

    protected override void FailCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "Edit Fail. Try Again.";
    }

    protected override void CheckEmailDuplication()
    {
        if (DatabaseManager.Instance.CheckEmailDuplication(emailInput.text)
            || emailInput.text.CompareTo(DatabaseManager.Instance.data.email) == 0)
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
}
