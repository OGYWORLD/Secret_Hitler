using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CPollButton : MonoBehaviour, IDeselectHandler
{
    public void OnDeselect(BaseEventData eventData) // 버튼이 포커스를 잃었을 때 투표 초기화
    {
        PhotonManager.Instance.pollButtonDisable?.Invoke();
    }
}
