using UnityEngine;
using UnityEngine.UI;

public class CRoomCardInfo : MonoBehaviour
{
    public Text roomNameText;
    public Text liberalNum;
    public Text pacistNum;
    public Text maxPeopleNum;
    public Text curPeopleNum;

    public Button roomCardButton;

    private void Awake()
    {
        roomCardButton.onClick.AddListener(OnEnterRoom);
    }

    private void OnEnterRoom()
    {
        PhotonManager.Instance.EnterRoom(roomNameText.text);
        PanelManager.Instance.InitPanel((int)Panel.enterDelayPanel);
    }
}
