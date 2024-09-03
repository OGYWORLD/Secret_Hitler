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
        tips.Add("히틀러는 누가 파시스트인지 모릅니다!");
        tips.Add("히틀러가 수상이 되는 걸 주의하세요!");
        tips.Add("대통령은 신중하게 선택하세요!");
        tips.Add("정책 결과를 신중하게 확인하세요!");
    }
    private void OnEnable()
    {
        int index = Random.Range(0, tips.Count);
        tipTMP.text = tips[index];
    }
}
