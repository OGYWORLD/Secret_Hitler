using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    public Button speedRoomButton;
    public Button makeRoomButton;
    public Button settingButton;

    public GameObject settingPanel;
    public GameObject makeRoomPanel;

    private void Awake()
    {
        makeRoomButton.onClick.AddListener(ToMakeRoom);
        settingButton.onClick.AddListener(ToSetting);
        speedRoomButton.onClick.AddListener(OnSpeedRoom);

        settingPanel.SetActive(false);
        makeRoomPanel.SetActive(false);
    }

    private void OnEnable()
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.shortBellSF);
    }

    private void ToMakeRoom()
    {
        makeRoomPanel.SetActive(true);
    }

    private void ToSetting()
    {
        settingPanel.SetActive(true);
    }

    private void OnSpeedRoom()
    {
        PhotonManager.Instance.JoinOrCreateRoom("빠른 시작 해요~", 5);
    }
}
