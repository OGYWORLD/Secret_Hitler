using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CIntroTextPrint : MonoBehaviour
{
    public TextMeshProUGUI introTMP;

    private string introSentence = "당신은 히틀러를 멈출 수 있는가.";
    private Animator tmpAnim;

    private IEnumerator Start()
    {
        tmpAnim = GetComponentInChildren<Animator>();

        for(int i = 0; i < introSentence.Length; i++)
        {
            if(Input.anyKey)
            {
                break;
            }

            introTMP.text += introSentence[i];

            yield return new WaitForSeconds(0.2f);
        }

        tmpAnim?.SetTrigger("isDown");

        yield return new WaitForSeconds(0.5f);

        PanelManager.Instance.InitPanel((int)Panel.loginPanel);
    }


}
