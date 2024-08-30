using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEditAccount : CCreateAccount
{

    private Color green = new Color(18f / 255f, 166f / 255f, 104f / 255f);
    private Color red = new Color(231f / 255f, 19f / 255f, 28f / 255f);

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
        infoPanelTMP.text = "������ �Ϸ�Ǿ����ϴ�.";

        DatabaseManager.Instance.UpdateUserData();
    }

    protected override void FailCreate()
    {
        infoPanel.SetActive(true);
        infoPanelTMP.text = "�ٽ� �õ����ּ���.";
    }

    protected override void CheckEmailDuplication()
    {
        if (DatabaseManager.Instance.CheckEmailDuplication(emailInput.text)
            || emailInput.text.CompareTo(DatabaseManager.Instance.data.email) == 0)
        {
            isCheckEmailDP = true;

            dpCheckTMP.color = green;
            dpCheckTMP.text = "�̸����� �����Ǿ����ϴ�.";

        }
        else
        {
            dpCheckTMP.color = red;
            dpCheckTMP.text = "�ߺ��� �̸����Դϴ�.";
        }
    }
}
