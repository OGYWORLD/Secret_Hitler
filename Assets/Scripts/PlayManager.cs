using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
    public GameObject myPosObj;

    public GameObject[] nameCards;
    public Transform[] cardTrans;

    public TMP_InputField chatInputField;

    public Dictionary<string, GameObject> cardDictionary = new Dictionary<string, GameObject>();

    public Button readyButton;
    public Button outButton;
    public GameObject settingButtonObj;

    public int[] totalPosition { get; set; }
    private List<bool> isSelected = new List<bool>();

    public Image fadeImage;

    public Sprite[] posCardImg; // 신분 카드 (0: hitler, 1~2: pacist, 3~9: liberal)

    public GameObject posInfoTextObj; // 당신의 신분은?
    public GameObject[] posObj; // 신분 정보 오브젝트 (0: 리버럴, 1: 파시스트, 2: 히틀러)
    public Text[] pacistListTexts; // 역할 공개 장면 파시스트 리스트 (파시스트)
    public Text[] pacistHitlerListTexts; // 역할 공개 장면 파시스트 리스트 (히틀러)
    public GameObject hitler56obj; // 5-6인 시, 히틀러가 가지는 파시스트 리스트 오브젝트

    public GameObject[] pacistBoads; // 파시스트 보드판 0: 5~6, 1: 7~8, 2: 9~10

    public GameObject infoPanel; // 내각 구성, 찬반 투표 등 현재 상황을 알려주는 판넬

    public int[] playerOrder; // 플레이어 순서 - Actor Number로 저장

    private int totalPolicyNum = 17;
    public int[] policyArray; // 정책 배열 (0: liberal, 1: pacist) liberal 6장, pacist 11장

    private void Start()
    {
        policyArray = new int[17];
    }

    public void InitWhenJoinedRoom()
    {
        // 보드판 인원수에 맞게 등장
        foreach(GameObject board in pacistBoads)
        {
            board.SetActive(false);
        }

        if(PhotonNetwork.CurrentRoom.MaxPlayers == 5 || PhotonNetwork.CurrentRoom.MaxPlayers == 6)
        {
            pacistBoads[0].SetActive(true);
        }

        if (PhotonNetwork.CurrentRoom.MaxPlayers == 7 || PhotonNetwork.CurrentRoom.MaxPlayers == 8)
        {
            pacistBoads[1].SetActive(true);
        }

        if (PhotonNetwork.CurrentRoom.MaxPlayers == 9 || PhotonNetwork.CurrentRoom.MaxPlayers == 10)
        {
            pacistBoads[2].SetActive(true);
        }

        // 페이드인아웃 용 이미지 알파값 0으로 초기화
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;

        foreach (Text t in pacistListTexts)
        {
            t.text = "";
        }

        foreach (Text t in pacistHitlerListTexts)
        {
            t.text = "";
        }

        // 버튼들 활성화
        readyButton.gameObject.SetActive(true);
        outButton.gameObject.SetActive(true);

        // 자신의 신분 카드 비활성화
        myPosObj.gameObject.SetActive(false);

        // 다른 사람의 신분 카드 비활성화
        foreach(GameObject obj in cardDictionary.Values)
        {
            obj.transform.GetChild(3).gameObject.SetActive(false);
        }

        // info panel 비활성화
        infoPanel.SetActive(false);
    }

    public int[] SufflePolicy() // 정책 배열 섞기
    {
        int liberalCnt = 6;
        int pacistCnt = 11;

        for(int i = 0; i < totalPolicyNum;)
        {
            int policy = UnityEngine.Random.Range(0, 2); // 랜덤하게 정책 뽑기 (0: liberal, 1: pacist)
            
            if(policy == 0 && liberalCnt > 0)
            {
                policyArray[i] = policy;
                liberalCnt--;

                i++;
            }
            else if(policy == 1 && pacistCnt > 0)
            {
                policyArray[i] = policy;
                pacistCnt--;

                i++;
            }
        }

        return policyArray;
    }

    public void PassSufflePolicyRPC(int[] p)
    {
        view.RPC("PassPSufflePolicy", RpcTarget.All, p);
    }

    [PunRPC]
    public void PassPSufflePolicy(int[] p)
    {
        policyArray = p;
    }

    public int[] SetPlayerOrder() // 플레이 순서 정하기
    {
        int index = 0;
        playerOrder = new int[PhotonNetwork.CurrentRoom.MaxPlayers];
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            playerOrder[index] = player.Value.ActorNumber;

            index++;
        }

        return playerOrder;
    }

    public void PassPlayerOrderRPC(int[] p)
    {
        view.RPC("PassPlayerOrder", RpcTarget.All, p);
    }

    [PunRPC]
    public void PassPlayerOrder(int[] p)
    {
        playerOrder = p;
    }

    public void PickPosition() // 역할 뽑기
    {
        totalPosition = new int[PhotonNetwork.CurrentRoom.MaxPlayers];
        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++) // 역할 방문 배열 초기화
        {
            isSelected.Add(false);
            totalPosition[i] = (int)Position.liberal;
        }

        if (!PhotonNetwork.IsMasterClient) return; // 마스터 클라이언트(방장)가 뽑음

        int liberalCnt = PhotonManager.Instance.cntDictionary[PhotonNetwork.CurrentRoom.MaxPlayers].Item1;
        int pacistCnt = PhotonManager.Instance.cntDictionary[PhotonNetwork.CurrentRoom.MaxPlayers].Item2;

        int totalCnt = liberalCnt + pacistCnt;

        for (int i = 0; i < totalCnt;) // List에 역할을 랜덤하게 넣음
        {
            int random = UnityEngine.Random.Range(0, totalCnt);
 
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
    public void StateInitForGameStart() // 게임 시작 후 레디 상태 문구 지우기
    {
        stateText.text = "";

        foreach (var obj in cardDictionary)
        {
            Text state = obj.Value.GetComponentsInChildren<Text>()[1];
            state.text = "";
        }
    }

    public void StateInitForGameStartRPC()
    {
        view.RPC("StateInitForGameStart", RpcTarget.All);
    }

    public void SetPlayerCustomForPosition() // 포지션 뽑기
    {
        int index = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // 플레이어 커스텀 프로퍼티(포지션 - 리버럴, 파시스트, 히틀러) 지정
            PhotonManager.Instance.playerProperties["position"] = (Position)totalPosition[index];
            player.SetCustomProperties(PhotonManager.Instance.playerProperties);

            index++;
        }
    }

    public void ShowPickChancellorInfo(Action pickChancellor) // 수상 뽑으라고 안내
    {
        // 0: 내각구성, 1: 대통령, 2: 대통령이름, 3: 다음 대통령, 4: 대통령은 수상을 선정~
        Text[] texts = infoPanel.GetComponentsInChildren<Text>();

        texts[0].text = "내각 구성"; 
        texts[1].text = "대통령"; 
        texts[2].text = PhotonNetwork.CurrentRoom.Players[playerOrder[(int)PhotonManager.Instance.roomProperties["currentOrder"]]].NickName; // TODO: currentOrder 증가할 때 0으로 초기화
        texts[3].text = $"다음 대통령은 {PhotonNetwork.CurrentRoom.Players[playerOrder[(int)PhotonManager.Instance.roomProperties["currentOrder"]+ 1]].NickName} 입니다."; 
        texts[4].text = "대통령은 수상을 선정하십시오.";

        infoPanel.SetActive(true);

        // 수상 뽑기 함수 호출
        pickChancellor?.Invoke();
    }

    public void PickChanellorInfo() // 수상 뽑기
    {
        // 대통령이라면 수상 선택
        if((Player)PhotonNetwork.CurrentRoom.CustomProperties["president"] == PhotonNetwork.LocalPlayer)
        {
            roomNameText.text = "수상을 선택해주십시오.";
        }
        else // 아니라면 대기
        {
            roomNameText.text = $"{PhotonNetwork.CurrentRoom.Players[playerOrder[(int)PhotonManager.Instance.roomProperties["currentOrder"]]].NickName}이(가) 수상을 선정 중입니다.";
        }
    }

    public void SetPositionCard(Player player)
    {
        // 플레이어 UI에 보이는 역할 카드 스프라이트 교체
        int spriteIdx = 0;

        if ((Position)player.CustomProperties["position"] == Position.liberal)
        {
            spriteIdx = UnityEngine.Random.Range(3, 10);

            if(player == PhotonNetwork.LocalPlayer)
            {
                myPosObj.gameObject.GetComponent<Image>().sprite = posCardImg[spriteIdx];
                posObj[0].GetComponentInChildren<Image>().sprite = posCardImg[spriteIdx];
            }
        }
        else if ((Position)player.CustomProperties["position"] == Position.pacist)
        {
            spriteIdx = UnityEngine.Random.Range(1, 3);

            if (player == PhotonNetwork.LocalPlayer)
            {
                myPosObj.gameObject.GetComponent<Image>().sprite = posCardImg[spriteIdx];
                posObj[1].GetComponentInChildren<Image>().sprite = posCardImg[spriteIdx];
            }
        }
        else
        {
            spriteIdx = 0;

            if (player == PhotonNetwork.LocalPlayer)
            {
                myPosObj.gameObject.GetComponent<Image>().sprite = posCardImg[spriteIdx];
                posObj[2].GetComponentInChildren<Image>().sprite = posCardImg[spriteIdx];
            }
        }

        if(player != PhotonNetwork.LocalPlayer)
        {
            cardDictionary[player.NickName].transform.GetChild(3).gameObject.GetComponent<Image>().sprite
            = posCardImg[spriteIdx];
        }
    }

    public void ShowPositionRPC() // 게임 시작 후 신분 보여주기
    {
        view.RPC("ShowPosition", RpcTarget.All);
    }

    [PunRPC]
    public void ShowPosition()
    {
        StartCoroutine(GameStartIntro());
    }

    public void KickAllPlayerRPC() // 누구 한 명 나가면 방 없애기
    {
        view.RPC("KickOut", RpcTarget.All);
    }

    [PunRPC]
    public void KickOut()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void PickChancellor()
    {

    }

    private IEnumerator GameStartIntro() // 게임 시작 후 신분 인트로
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

        // 신분 공개 오브젝트 활성화
        posInfoTextObj.SetActive(true);
        posObj[(int)PhotonNetwork.LocalPlayer.CustomProperties["position"]].SetActive(true);

        // 자신의 신분 카드 활성화
        myPosObj.gameObject.SetActive(true);

        int index = 0;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values) // 신분 공개 때 파시스트 리스트 출력
        {
            if (PhotonNetwork.CurrentRoom.MaxPlayers <= 6 && player != PhotonNetwork.LocalPlayer)
            {
                hitler56obj.SetActive(true);
                if ((Position)player.CustomProperties["position"] == Position.pacist || (Position)player.CustomProperties["position"] == Position.hitler)
                {
                    if((Position)PhotonNetwork.LocalPlayer.CustomProperties["position"] == Position.pacist)
                    {
                        pacistListTexts[index].text = $"{player.NickName}";

                        // 파시스트들의 신분카드 활성화
                        cardDictionary[player.NickName].transform.GetChild(3).gameObject.SetActive(true);
                        index++;
                    }
                    else if((Position)PhotonNetwork.LocalPlayer.CustomProperties["position"] == Position.hitler)
                    {
                        pacistHitlerListTexts[index].text = $"{player.NickName}";

                        // 파시스트들의 신분카드 활성화
                        cardDictionary[player.NickName].transform.GetChild(3).gameObject.SetActive(true);
                        index++;
                    }
                }
            }
            else if(PhotonNetwork.CurrentRoom.MaxPlayers > 6 && player != PhotonNetwork.LocalPlayer)
            {
                hitler56obj.SetActive(false);
                if ((Position)PhotonNetwork.LocalPlayer.CustomProperties["position"] == Position.pacist)
                {
                    if ((Position)player.CustomProperties["position"] == Position.pacist)
                    {
                        pacistListTexts[index].text = $"{player.NickName}";

                        // 파시스트들의 신분카드 활성화
                        cardDictionary[player.NickName].transform.GetChild(3).gameObject.SetActive(true);

                        index++;
                    }
                }
            }
        }

        yield return new WaitForSeconds(3f);

        posInfoTextObj.SetActive(false);
        posObj[(int)PhotonNetwork.LocalPlayer.CustomProperties["position"]].SetActive(false);

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

        yield return new WaitForSeconds(0.5f);

        ShowPickChancellorInfo(PickChanellorInfo);

        yield break;
    }
}
