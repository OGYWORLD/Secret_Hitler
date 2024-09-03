using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CRoom : MonoBehaviour
{
    public Text nameText;

    private void Start()
    {
        nameText.text = DatabaseManager.Instance.data.name; // 플레이어 이름 출력
    }

   
}
