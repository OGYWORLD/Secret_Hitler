using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CLobby : MonoBehaviourPunCallbacks
{

    public override void OnEnable()
    {
        StartCoroutine(WaitCreateInstance());

        PhotonManager.Instance.JoinOrCreateRoom();
    }


    private void Update()
    {
        BackHome();
    }

    private void BackHome()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonManager.Instance.LeaveRoom();
            PanelManager.Instance.InitPanel((int)Panel.mainMenuPanel);
        }
    }

    private IEnumerator WaitCreateInstance()
    {
        yield return new WaitForEndOfFrame();
    }

}
