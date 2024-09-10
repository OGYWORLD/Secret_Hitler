using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

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

    public Dictionary<string, int> nameActorDictionary = new Dictionary<string, int>(); // <플레이어, 액터넘버>

    private int totalPolicyNum = 17;
    public int[] policyArray; // 정책 배열 (0: liberal, 1: pacist) liberal 6장, pacist 11장

    public GameObject chanPanel; // 수상 선택 패널
    public ToggleGroup chanToggleGroup; // 수상 선택 토글 그룹
    public Toggle[] chanToggles; // 수상 선택 토글 배열

    private int minPlayer = 5; // 게임 최소 인원수

    public Button chanPickBtn; // 수상 선정 버튼

    public GameObject pollPanel; // 투표 패널
    public Button[] pollCardsBtn; // 투표 카드 (0: ja, 1: nein)
    public Button pollFinBtn; // 투표 완료 버튼
    public GameObject pollWaringTextObj; // 투표 미완료 경고 오브젝트

    private int myJaNein = -1; // 찬반 투표 선택값 (-1: 아직 하지 않음, 0: ja, 1: nein) 

    private Hashtable pollResultHash = new Hashtable();

    public GameObject pollResultPanel; // 투표 결과 패널
    public Text[] jaPollResultTexts; // 투표 결과 중 찬성 배열
    public Text[] neinPollResultTexts; // 투표 결과 중 반대 배열

    public Image baseImg; // 배경 화면 - 엔딩 시 필요
    public Sprite[] endingBase; // 엔딩에 따른 배경화면 sprite
    public GameObject endingPanel; // 엔딩 패널 (0: pacism, 1: liberal)

    public Sprite[] policyImg; // 정책 스프라이트 배열 (0: liberal, 1: pacist)
    public GameObject policyPanel; // 대통령 정책 결정 패널
    public Button[] policyBtn; // 정책 버튼 배열

    public GameObject[] pickedLiberal; // 보드판 위 리버럴 정책 배열
    public GameObject[] pickedPacist; // 보드판 위 파시스트 정책 배열

    private void Awake()
    {
        policyArray = new int[17];

        chanPickBtn.onClick.AddListener(SetChancellorAndSendPickEnd);
        pollCardsBtn[0].onClick.AddListener(() => { myJaNein = 0; pollWaringTextObj.SetActive(false); });
        pollCardsBtn[1].onClick.AddListener(() => { myJaNein = 1; pollWaringTextObj.SetActive(false); });
        pollFinBtn.onClick.AddListener(FinPoll);

        policyBtn[0].onClick.AddListener(() => LeavePolicy(0));
        policyBtn[1].onClick.AddListener(() => LeavePolicy(1));
        policyBtn[2].onClick.AddListener(() => LeavePolicy(2));
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

        // 플레이어 커스텀 프로퍼티 초기화
        PhotonHashtable playerProperties = new PhotonHashtable();
        playerProperties["beforeChan"] = false; // 이전 수상 초기화
        playerProperties["beforePre"] = false; // 이전 대통령 초기화
        playerProperties["ready"] = false; // 커스텀 플레이어 프로퍼티 속성 설정 (레디 상태)

        if(PhotonNetwork.IsMasterClient)
        {
            playerProperties["ready"] = true; // 마스터 플레이어라면 레디를 true로
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
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
            nameActorDictionary[player.Value.NickName] = player.Value.ActorNumber; // 플레이어 이름에 따른 액터넘버

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
            PhotonHashtable existPlayerProperties = player.CustomProperties;

            existPlayerProperties["position"] = (Position)totalPosition[index];
            player.SetCustomProperties(existPlayerProperties);

            index++;
        }
    }

    public void ShowPickChancellorInfo(Action pickChancellor) // 수상 뽑으라고 안내
    {
        // 0: 내각구성, 1: 대통령, 2: 대통령이름, 3: 다음 대통령, 4: 대통령은 수상을 선정~
        Text[] texts = infoPanel.GetComponentsInChildren<Text>();

        texts[0].text = "내각 구성을 시작합니다"; 
        texts[1].text = "대통령"; 
        texts[2].text = PhotonNetwork.CurrentRoom.Players[playerOrder[(int)PhotonNetwork.CurrentRoom.CustomProperties["currentOrder"]]].NickName; // TODO: currentOrder 증가할 때 0으로 초기화
        texts[3].text = $"다음 대통령은 {PhotonNetwork.CurrentRoom.Players[playerOrder[(int)PhotonNetwork.CurrentRoom.CustomProperties["currentOrder"] + 1]].NickName} 입니다."; 
        texts[4].text = "대통령은 수상을 선정하십시오.";

        infoPanel.SetActive(true);

        // 애니메이션 올라갈 때까지 대기 후 수상 뽑기 호출
        StartCoroutine(WaitPanelSeconds(4f, pickChancellor));
    }

    public void PickChanellorInfo() // 수상 뽑기
    {
        infoPanel.SetActive(false);

        // 대통령이라면 수상 선택
        if((Player)PhotonNetwork.CurrentRoom.CustomProperties["president"] == PhotonNetwork.LocalPlayer)
        {
            roomNameText.text = "수상을 선택해주십시오.";

            ShowChanCandidate();
        }
        else // 아니라면 대기
        {
            roomNameText.text = $"{PhotonNetwork.CurrentRoom.Players[playerOrder[(int)PhotonNetwork.CurrentRoom.CustomProperties["currentOrder"]]].NickName}이(가) 수상을 선정 중입니다.";
        }
    }

    public void ShowChanCandidate() // 토글 그룹에 수상 후보 목록 출력
    {
        ToggleListInit();

        int index = 0;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if(player != PhotonNetwork.LocalPlayer)
            {
                // 현재 생존 인원이 6명 이상일 경우
                if ((int)PhotonNetwork.CurrentRoom.CustomProperties["liveCnt"] > minPlayer)
                {
                    if (!(bool)player.CustomProperties["beforePre"] && !(bool)player.CustomProperties["beforeChan"])
                    {
                        chanToggles[index].GetComponentInChildren<Text>().text = player.NickName;
                        chanToggles[index].name = player.NickName;
                        chanToggles[index].gameObject.SetActive(true);

                        index++;
                    }
                }
                else
                {
                    if (!(bool)player.CustomProperties["beforeChan"])
                    {
                        chanToggles[index].GetComponentInChildren<Text>().text = player.NickName;
                        chanToggles[index].name = player.NickName;
                        chanToggles[index].gameObject.SetActive(true);

                        index++;
                    }
                }
            }
        }

        chanPanel.SetActive(true);
    }

    public void SetChancellorAndSendPickEnd()
    {
        Toggle selectedToggle = null; // 수상으로 선정된 토글

        // 누굴 선정했는지 가져오기
        int cnt = 0;
        foreach(Toggle t in chanToggleGroup.ActiveToggles())
        {
            selectedToggle = t;
            cnt++;
            break;
        }

        if (cnt == 0) return; // 수상이 선택되지 않았음

        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        // 수상이 누군지 선정 (내각 구성 무산 시 -1로 지정)
        existRoomProperties["chancellor"] = PhotonNetwork.CurrentRoom.Players[nameActorDictionary[selectedToggle.GetComponentInChildren<Text>().text]];
        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

        // 뽑기 완료 -> 투표하기 진행
        ShowPollInfoRPC();
    }

    private void ToggleListInit() // 수상 후보 토글 목록 비활성화
    {
        foreach (Toggle toggle in chanToggles)
        {
            toggle.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void ShowPollInfo()
    {
        chanPanel.SetActive(false);

        roomNameText.text = "";

        Player president = (Player)PhotonNetwork.CurrentRoom.CustomProperties["president"];
        Player chancellor = (Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"];

        // 0: 내각구성 찬반 투표, 1: 대통령, 2: 대통령이름, 3: 다음 대통령, 4: 대통령은 수상을 선정~
        Text[] texts = infoPanel.GetComponentsInChildren<Text>();
        
        texts[0].text = "내각 구성 찬반 투표를 시작합니다";
        texts[1].text = "대통령, 수상";
        texts[2].text = $"{president.NickName}, {chancellor.NickName}";
        texts[3].text = "과반수 동의 시, 내각이 구성됩니다.";
        texts[4].text = "투표를 시작해주십시오.";

        infoPanel.SetActive(true);

        // 판넬 애니메이션 대기 후 찬반 투표 시작
        StartCoroutine(WaitPanelSeconds(4f, Poll));
    }

    public void ShowPollInfoRPC()
    {
        view.RPC("ShowPollInfo", RpcTarget.All);
    }

    public void Poll() // 찬반 투표
    {
        if(PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["president"] ||
            PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["chancellor"])
        {
            roomNameText.text = "의원들의 투표를 기다리는 중 입니다...";
            return;
        }

        // 초기화
        roomNameText.text = "투표를 진행해주십시오.";
        myJaNein = -1;
        pollResultHash.Clear();

        infoPanel.SetActive(false);
        pollPanel.SetActive(true);
    }

    public void FinPoll() // 투표 완료 버튼
    {
        if(myJaNein == -1) // 투표를 하지 않았다면 완료 되지 않음
        {
            pollWaringTextObj.SetActive(true);
            return;
        }

        if(pollCardsBtn[0].IsInteractable()) // 투표 미완료 -> 완료
        {
            pollCardsBtn[0].interactable = false;
            pollCardsBtn[1].interactable = false;

            pollFinBtn.gameObject.GetComponentInChildren<Text>().text = "투표 변경";

            view.RPC("SendPollResultToMaster", RpcTarget.MasterClient, 
                PhotonNetwork.NickName, myJaNein); // 마스터 클라이언트에게 투표 결과 전송
        }
        else  // 투표 완료 -> 미완료
        {
            pollCardsBtn[0].interactable = true;
            pollCardsBtn[1].interactable = true;

            pollFinBtn.gameObject.GetComponentInChildren<Text>().text = "투표 완료";
        }
    }

    [PunRPC]
    public void SendPollResultToMaster(string name, int result) // 마스터 클라이언트가 투표 결과를 확인
    {
        pollResultHash[name] = result;

        if(pollResultHash.Count == (PhotonNetwork.CurrentRoom.MaxPlayers - 2)) // 모든 사람이 투표를 완료 했다면
        {
            string resultS = ""; // 투표 결과를 문자열로 저장하여 전송
            // "NickName 0 NickName 1 ..." (" "으로 구분)

            int jaCnt = 0;
            int neinCnt = 0;
            foreach(DictionaryEntry d in pollResultHash)
            {
                resultS += $"{d.Key} {d.Value} ";

                if ((int)d.Value == 0) jaCnt++;
                if ((int)d.Value == 1) neinCnt++;
            }

            if(jaCnt > neinCnt) // 내각 구성 성공
            {
                view.RPC("ShowPollResult", RpcTarget.All, resultS, 0);
            }
            else // 내각 구성 실패
            {
                view.RPC("ShowPollResult", RpcTarget.All, resultS, 1);
            }
        }
    }

    [PunRPC]
    public void ShowPollResult(string resultS, int result)
    {
        pollPanel.SetActive(false);
        pollResultPanel.SetActive(true);
        InitPollResultArray();

        string[] results = resultS.Split(" "); // 결과 문자열을 split

        int jaIdx = 0;
        int neinIdx = 0;
        for(int i = 0; i < results.Length-1; i+=2) // 결과 리스트 세팅
        {
            if(results[i + 1] == "0") // 찬성
            {
                jaPollResultTexts[jaIdx].text = results[i];
                jaIdx++;
            }
            else // 반대
            {
                neinPollResultTexts[neinIdx].text = results[i];
                neinIdx++;
            }
        }

        StartCoroutine(WaitPanelSeconds(4f, () => ShowPolicyPick(result))); // 투표 패널 비활성화
    }

    public void ShowPolicyPick(int result)
    {
        pollResultPanel.SetActive(false);

        if (result == 0) // 내각 구성에 성공했다면 투표 진행
        {
            // 파시즘 정책이 3개 이상 발의되었고 수상이 히틀러이라면 종료
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] >= 3
                && (Position)((Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"]).CustomProperties["position"] == Position.hitler)
            {
                baseImg.sprite = endingBase[0]; // 파시즘 배경으로 교환

                Text[] texts = endingPanel.GetComponentsInChildren<Text>();
                texts[0].text = "파시스트가 승리하였습니다";
                texts[1].text = "파시즘 정책 3개 이상 발의 후 히틀러 수상";

                ShowAllPositionCard();

                endingPanel.SetActive(true);

                StartCoroutine(WaitPanelSeconds(5f, InitPos));
            }
            else // 정책 뽑기
            {
                infoPanel.SetActive(false);
                roomNameText.text = "";

                // 정책 뽑는다 info panel 나오기
                Text[] texts = infoPanel.GetComponentsInChildren<Text>();

                Player president = (Player)PhotonNetwork.CurrentRoom.CustomProperties["president"];
                Player chancellor = (Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"];

                texts[0].text = "신성한 의회 단계입니다";
                texts[1].text = "대통령, 수상";
                texts[2].text = $"{president.NickName}, {chancellor.NickName}";
                texts[3].text = "모두의 채팅과 보이스가 중지됩니다.";
                texts[4].text = "대통령과 수상은 정책을 선택해주십시오.";

                // 채팅 보이스 막아야 함
                chatInputField.interactable = false;

                infoPanel.SetActive(true);

                // president한테 투표하라고 함수 호출
                if(PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["president"])
                {
                    StartCoroutine(WaitPanelSeconds(4f, PickPolicyByPresident));
                }
                else // 아닐 시 대기
                {
                    StartCoroutine(WaitPanelSeconds(4f, () => { infoPanel.SetActive(false); }));
                }
            }
        }
        else // 내각 구성에 실패했다면 추적용 마커 1칸 전진
        {
            print("내각 구성 실패~");
        }
    }

    public void ShowAllPositionCard()
    {
        foreach(GameObject obj in cardDictionary.Values)
        {
            obj.SetActive(true);
        }
    }

    public void InitPos()
    {
        endingPanel.SetActive(false); // 엔딩 패널 비활성화

        baseImg.sprite = endingBase[2]; // 배경화면 초기화

        foreach (GameObject obj in cardDictionary.Values) // 신분 카드 비활성화
        {
            obj.SetActive(false);
        }

        // 게임 종료 상태 설정
        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        existRoomProperties["ing"] = false;
        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);
        
        // 레디 상태 설정
        PhotonHashtable existPlayerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            existPlayerProperties["ready"] = false;
            player.SetCustomProperties(existPlayerProperties);
        }
    }

    public void PickPolicyByPresident() // 대통령 정책 뽑기
    {
        infoPanel.SetActive(false);

        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        PhotonHashtable existPlayerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

        // 이전 대통령, 수상 여부 초기화
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // TODO: 이 새끼가 범인
            //PhotonHashtable h = player.CustomProperties; // TODO: 여기 참조 오류나는지 확인
            //h["beforePre"] = false;
            //h["beforeChan"] = false;

            //player.SetCustomProperties(h);
        }

        PolicyActiveInit();

        roomNameText.text = "버릴 정책을 선택해주세요";

        int policyIdx = (int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"];

        Image[] images = policyPanel.GetComponentsInChildren<Image>();

        if(policyIdx > totalPolicyNum - 3)
        {
            // 정책 섞기
            SufflePolicy();
            PassSufflePolicyRPC(policyArray);

            // 정책 인덱스 초기화
            existRoomProperties["policyIdx"] = 0;
            PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);
        }

        // 정책 카드 이미지 설정
        images[0].sprite = policyImg[policyArray[policyIdx]];
        images[1].sprite = policyImg[policyArray[policyIdx+1]];
        images[2].sprite = policyImg[policyArray[policyIdx+2]];

        existRoomProperties["policyIdx"] = (int)existRoomProperties["policyIdx"] + 3;
        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

        // 이전 대통령 여부 설정
        existPlayerProperties["beforePre"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(existPlayerProperties);

        policyPanel.SetActive(true);
    }

    public void LeavePolicy(int n)
    {
        // 버려진 정책 개수 카운팅
        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        existRoomProperties["leavePolicyCnt"] = (int)existRoomProperties["leavePolicyCnt"] + 1;

        // 대통령이라면 수상에게 넘기기
        if(PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["president"])
        {
            policyPanel.SetActive(false);

            roomNameText.text = "수상께서 정책 선정 중입니다";

            // 수상에게 넘기기
            view.RPC("PickPolicyByChancellor", (Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"], n);
        }
        else if (PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["chancellor"]) // 수상이라면 정책 발의
        {
            policyPanel.SetActive(false);

            // 정책 발의
            view.RPC("ShowPickedPolicy", RpcTarget.All, n);
        }
    }

    [PunRPC]
    public void PickPolicyByChancellor(int n)
    {
        // 이전 수상 여부 설정
        PhotonHashtable existPlayerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        existPlayerProperties["beforeChan"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(existPlayerProperties);

        roomNameText.text = "버릴 정책을 선택하여 주십시오";

        PolicyActiveInit();

        policyBtn[n].gameObject.SetActive(false); // 버려진 카드 비활성화

        policyPanel.SetActive(true);
    }

    [PunRPC]
    public void ShowPickedPolicy(int n)
    {
        roomNameText.text = "";

        int curPickedPolicy = policyArray[(int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"] - 3 + n];

        if(curPickedPolicy == 0) // 뽑힌 정책이 리버럴이라면
        {
            int idx = (int)PhotonNetwork.CurrentRoom.CustomProperties["liberalPolicy"]; // 보드판에 깔기
            pickedLiberal[idx].SetActive(true);

            // 리버럴 정책 개수 증가
            PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            existRoomProperties["liberalPolicy"] = (int)existRoomProperties["liberalPolicy"] + 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

            // 다음 내각 의회 구성
            print("새로운 내각을 구성합니다.");
        }
        else // 뽑힌 정책이 파시즘이라면
        {
            int idx = (int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"]; // 보드판에 깔기
            pickedPacist[idx].SetActive(true);

            // 파시즘 정책 개수 증가
            PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            existRoomProperties["pacismPolicy"] = (int)existRoomProperties["pacismPolicy"] + 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

            // TODO: 대통령 특수 권한 실행
            print("대통령 특수 권한을 실행합니다.");
        }
    }

    public void PolicyActiveInit()
    {
        policyBtn[0].gameObject.SetActive(true);
        policyBtn[1].gameObject.SetActive(true);
        policyBtn[2].gameObject.SetActive(true);
    }

    public void InitPollResultArray()
    {
        for(int i = 0; i < jaPollResultTexts.Length; i++)
        {
            jaPollResultTexts[i].text = "";
            neinPollResultTexts[i].text = "";
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

    private IEnumerator WaitPanelSeconds(float s, Action callback)
    {
        yield return new WaitForSeconds(s);

        callback?.Invoke();
    }
}
