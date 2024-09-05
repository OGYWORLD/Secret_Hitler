using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Position
{
    liberal,
    pacist,
    hitler
}

public class PlayManager : MonoBehaviourPunCallbacks // 싱글톤으로 올릴려다가 든 게 많아서 내림
{ 
    public PhotonView view;

    public Text roomNameText;
    public Text stateText;
    public Text chatText;
    public GameObject chatObj;

    public GameObject[] nameCards;
    public Transform[] cardTrans;

    public TMP_InputField chatInputField;

    public Dictionary<string, GameObject> cardDictionary = new Dictionary<string, GameObject>();

    public Button readyButton;
    public Button outButton;
    public GameObject settingButtonObj;

    public int[] totalPosition { get; set; }
    private List<bool> isSelected = new List<bool>();
    public Position myPosition { get; set; }

    public Image fadeImage;

    public Sprite[] posCardImg;

    public GameObject posInfoTextObj; // 당신의 신분은?
    public GameObject[] posObj; // 신분 정보 오브젝트 (0: 리버럴, 1: 파시스트, 2: 히틀러)

    private void OnEnable()
    {
        // 페이드인아웃 용 이미지 알파값 0으로 초기화
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;
    }

    public void PickPosition() // 역할 뽑기
    {
        totalPosition = new int[PhotonNetwork.CurrentRoom.MaxPlayers];
        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++) // 역할 방문 배열 초기화
        {
            isSelected.Add(false);
            totalPosition[i] = (int)Position.liberal;
        }

        if (!PhotonNetwork.IsMasterClient) return; // 마스터 클라이언트가 뽑음

        int liberalCnt = PhotonManager.Instance.cntDictionary[PhotonNetwork.CurrentRoom.MaxPlayers].Item1;
        int pacistCnt = PhotonManager.Instance.cntDictionary[PhotonNetwork.CurrentRoom.MaxPlayers].Item2;

        int totalCnt = liberalCnt + pacistCnt;

        for (int i = 0; i < totalCnt;) // List에 역할을 랜덤하게 넣음
        {
            int random = Random.Range(0, totalCnt);
 
            if (!isSelected[random])
            {
                if (liberalCnt > 0)
                {
                    totalPosition[random] = (int)Position.liberal;
                    isSelected[random] = true;
                    liberalCnt--;
                }
                else
                {
                    if (pacistCnt == 1)
                    {
                        totalPosition[random] = (int)Position.hitler;
                        isSelected[random] = true;
                        pacistCnt--;
                    }
                    else
                    {
                        totalPosition[random] = (int)Position.pacist;
                        isSelected[random] = true;
                        pacistCnt--;
                    }
                }

                i++;
            }

        }
    }

    // TODO: 게임 종료 후 레디 미완료 문구도 넣어야함
    [PunRPC]
    private void StateInitForGameStart() // 게임 시작 후 레디 상태 문구 지우기
    {
        foreach (GameObject obj in cardDictionary.Values)
        {
            Text stateText = obj.GetComponentsInChildren<Text>()[1];
            stateText.text = "";
        }
    }

    public void StateInitForGameStartRPC()
    {
        view.RPC("StateInitForGameStart", RpcTarget.All);
    }

    [PunRPC]
    public void PickPosition(Position p, int[] posArray)
    {
        totalPosition = posArray;
        myPosition = p;
    }

    public void SendPickPositionRPC()
    {
        int index = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            view.RPC("PickPosition", player.Value, totalPosition[index], totalPosition);
            index++;
        }
    }

    public void ShowPositionRPC()
    {
        view.RPC("ShowPosition", RpcTarget.All);
    }

    [PunRPC]
    public void ShowPosition()
    {
        StartCoroutine(FadeInOut());
    }

    private IEnumerator FadeInOut()
    {
        float sumTime = 0f;
        float fadeTime = 2f;

        Color color = fadeImage.color;

        while (sumTime <= fadeTime)
        {
            color.a = Mathf.Lerp(0, 1, sumTime / fadeTime);
            fadeImage.color = color;

            sumTime += Time.deltaTime;

            yield return null;
        }

        color.a = 1;
        fadeImage.color = color;

        posInfoTextObj.SetActive(true);
        posObj[(int)myPosition].SetActive(true);

        yield return new WaitForSeconds(3f);

        posInfoTextObj.SetActive(false);
        posObj[(int)myPosition].SetActive(false);

        sumTime = 0f;

        while (sumTime <= fadeTime)
        {
            color.a = Mathf.Lerp(1, 0, sumTime / fadeTime);
            fadeImage.color = color;

            sumTime += Time.deltaTime;

            yield return null;
        }

        color.a = 0;
        fadeImage.color = color;

        yield break;
    }
}
