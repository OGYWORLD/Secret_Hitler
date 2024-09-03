using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CEnterDelay : MonoBehaviour
{
    public TextMeshProUGUI tipTMP;

    private List<string> tips = new List<string>();

    private void Awake()
    {
        tips.Add("��Ʋ���� ���� �Ľý�Ʈ���� �𸨴ϴ�!");
        tips.Add("��Ʋ���� ������ �Ǵ� �� �����ϼ���!");
        tips.Add("������� �����ϰ� �����ϼ���!");
        tips.Add("��å ����� �����ϰ� Ȯ���ϼ���!");
    }
    private void OnEnable()
    {
        int index = Random.Range(0, tips.Count);
        tipTMP.text = tips[index];
    }
}
