using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CInputFieldSF : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.clickSF);
    }
}
