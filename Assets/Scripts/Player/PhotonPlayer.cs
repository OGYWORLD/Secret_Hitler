using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonPlayer : MonoBehaviourPunCallbacks
{
    public Text testText;
    public InputField testInputField;

    private void Start()
    {
        testInputField.onEndEdit.AddListener(PhotonSendText);
    }

    [PunRPC]
    public void SendText(string s)
    {
        testText.text = s;
    }

    public void PhotonSendText(string s)
    {
        photonView.RPC("SendText", RpcTarget.All, s);
    }
}
