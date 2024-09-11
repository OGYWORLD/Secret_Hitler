using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CPollButton : MonoBehaviour, IDeselectHandler
{
    public void OnDeselect(BaseEventData eventData) // ��ư�� ��Ŀ���� �Ҿ��� �� ��ǥ �ʱ�ȭ
    {
        PhotonManager.Instance.pollButtonDisable?.Invoke();
    }
}
