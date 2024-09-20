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

public enum PresidentSpecial
{
    Policy, // 정책 훔쳐보기
    NextPresident, // 다음 대통령 지정
    Identity, // 신분 훔쳐보기
    Kill, // 죽이기
}

public class PlayManager : MonoBehaviourPunCallbacks // 싱글톤으로 올릴려다가 든 게 많아서 내림
{ 
    public PhotonView view;

    public TextMeshProUGUI readyOrStartTMP;

    private Color masterColor = new Color(1f, 1f, 144 / 255f);
    private Color nonReadyColor = new Color(188 / 255f, 188 / 255f, 188 / 255f);
    private Color liberalColor = new Color(120f / 255f, 253f / 255f, 249f / 255f);
    private Color pacistColor = new Color(253f / 255f, 120f / 255f, 138f / 255f);
    private Color hitlerColor = new Color(255f / 255f, 52f / 255f, 78f / 255f);

    public Text roomNameText;
    public Text stateText;
    public Text chatText;
    public GameObject chatObj;
    public GameObject myPosObj;
    public GameObject myCard;

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
    public int[] policyArray = new int[17]; // 정책 배열 (0: liberal, 1: pacist) liberal 6장, pacist 11장

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

    public GameObject[] markers; // 추적용 마커 오브젝트 배열

    public bool isStopCountdown = true; // 카운트다운 조기마감용 bool 변수
    public Text countDownText; // 카운트다운 텍스트
    public GameObject countDownObj; // 카운트다운 오브젝트

    public GameObject policyPeekPanel; // 정책 미리 보기 패널
    public GameObject identityPanel; // 신분 확인 패널

    public GameObject identitySelectPanel; // 신분 확인 선택 패널
    public GameObject showIdentityPanel; // 신분 공개 패널
    public GameObject[] identityBtns; // 신분 선택 버튼 배열
    public List<string> identityNamesList = new List<string>(); // 신분 버튼 매개변수용 리스트

    public Image[] policyImages; // policy Info의 Images 

    public GameObject nextPrePickPanel; // 다음 대통령 지정 패널
    public ToggleGroup nextPrePickToggleGroup; // 다음 대통령 지정 토글 그룹
    public Toggle[] nextPrePickToggles; // 다음 대통령 지정 토글 배열

    public Button prePickBtn; // 다음 대통령 강제 선정 버튼

    public Sprite deadImage; // 죽은 사람의 card image
    public Sprite cardImage; // 기본 card image

    public GameObject killPanel; // 처형 패널
    public ToggleGroup killToggleGroup; // 처형 토글 그룹
    public Toggle[] killToggles; // 처형 토글 배열

    public Button killBtn; // 처형 버튼

    public Image[] policyPeekImages; // 정책 미리보기의 Images

    public Sprite presidentImage; // 대통령 팻말 이미지
    public Sprite chancellorImage; // 수상 팻말 이미지

    private void Awake()
    {
        chanPickBtn.onClick.AddListener(SetChancellorAndSendPickEnd);
        prePickBtn.onClick.AddListener(SetNextPresident);
        killBtn.onClick.AddListener(KillSomeone);

        pollCardsBtn[0].onClick.AddListener(() => { myJaNein = 0; pollWaringTextObj.SetActive(false); });
        pollCardsBtn[1].onClick.AddListener(() => { myJaNein = 1; pollWaringTextObj.SetActive(false); });
        pollFinBtn.onClick.AddListener(FinPoll);

        policyBtn[0].onClick.AddListener(() => LeavePolicy(0));
        policyBtn[1].onClick.AddListener(() => LeavePolicy(1));
        policyBtn[2].onClick.AddListener(() => LeavePolicy(2));

        readyOrStartTMP = readyButton.GetComponentInChildren<TextMeshProUGUI>();

        policyImages = policyPanel.GetComponentsInChildren<Image>();

        int index = 0;
        foreach (GameObject obj in identityBtns)
        {
            identityNamesList.Add("");

            Button b = obj.GetComponent<Button>();

            int captureIdx = index;
            b.onClick.AddListener(() => ShowIdentity(identityNamesList[captureIdx]));

            index++;
        }
    }

    public void InitWhenJoinedRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name; // 방 이름 설정

        baseImg.sprite = endingBase[2]; // 배경화면 초기화

        // 보드판 인원수에 맞게 등장
        foreach (GameObject board in pacistBoads)
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
            obj.GetComponent<Image>().sprite = cardImage;
        }

        // info panel 비활성화
        infoPanel.SetActive(false);

        // 플레이어 커스텀 프로퍼티 초기화
        PhotonHashtable playerProperties = new PhotonHashtable();
        playerProperties["beforeChan"] = false; // 이전 수상 초기화
        playerProperties["beforePre"] = false; // 이전 대통령 초기화
        playerProperties["ready"] = false; // 커스텀 플레이어 프로퍼티 속성 설정 (레디 상태)
        playerProperties["dead"] = false; // 사망여부 초기화

        if (PhotonNetwork.IsMasterClient)
        {
            playerProperties["ready"] = true; // 마스터 플레이어라면 레디를 true로

            PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            existRoomProperties["ing"] = false;
            PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

            readyOrStartTMP.text = "게임 시작";

            stateText.color = masterColor;
            stateText.text = "회의 위원장";

            readyButton.interactable = false;
        }
        else
        {
            playerProperties["ready"] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            readyOrStartTMP.text = "게임 준비";

            stateText.color = nonReadyColor;
            stateText.text = "회의장으로 가는 중...";


            readyButton.interactable = true;
        }


        // 보드판 위 정책 카드 비활성화
        for (int i = 0; i < pickedLiberal.Length; i++)
        {
            pickedLiberal[i].SetActive(false);
        }

        for (int i = 0; i < pickedPacist.Length; i++)
        {
            pickedPacist[i].SetActive(false);
        }

        posInfoTextObj.SetActive(false); // 신분 공개 비활성화
        chanPanel.SetActive(false); // 수상 선택 패널 비활성화
        pollPanel.SetActive(false); // 내각 구성 찬반 패널 비활성화
        pollResultPanel.SetActive(false); // 내각 구성 결과 패널 비활성화
        endingPanel.SetActive(false); // 엔딩 패널 비활성화
        policyPanel.SetActive(false); // 정책 안내 패널 비활성화
        policyPeekPanel.SetActive(false); // 정책 미리보기 오브젝트 비활성화

        countDownObj.SetActive(false); // 카운트다운 오브젝트 비활성화

        if(PhotonNetwork.IsMasterClient)
            InitMarker(); // 추적용 마커 초기화
        InitMarkerActive(); // 추적용 마커 오브젝트 초기화

        InitPreChanImage(); // 대통령, 수상 명패 초기화

        SoundManager.Instance.PlayBGM(SoundManager.Instance.waitBGM);
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.bellSF);
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
        SoundManager.Instance.bgmAS.Stop(); // 배경음악 멈추기
        policyArray = p;
    }

    [PunRPC]
    public void SetButtonNameInit()
    {
        // 게임 시작했으므로 버튼과 제목(안내 문구로 사용하므로) 초기화
        roomNameText.text = "";
        readyButton.gameObject.SetActive(false);
        outButton.gameObject.SetActive(false);
    }

    public void SetButtonNameInitRPC()
    {
        view.RPC("SetButtonNameInit", RpcTarget.All);
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

        for(int i = 0; i < p.Length; i++)
        {
            string name = PhotonNetwork.CurrentRoom.Players[playerOrder[i]].NickName;
            nameActorDictionary[name] = playerOrder[i];
        }
    }

    public void PickPosition() // 역할 뽑기
    {
        isSelected.Clear();

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

    [PunRPC]
    public void StateInitForGameStart() // 게임 시작 후 레디 상태 문구 지우기
    {
        stateText.text = "";

        foreach (var obj in cardDictionary)
        {
            Text state = obj.Value.GetComponentsInChildren<Text>()[1];
            state.text = "";
        }

        markers[0].SetActive(true); // 추적용 마커 활성화
    }

    public void StateInitForGameStartRPC()
    {
        view.RPC("StateInitForGameStart", RpcTarget.All);
    }

    public void SetPlayerCustomForPosition() // 포지션 뽑기
    {
        int index = 0;

        Player hitler = null;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // 플레이어 커스텀 프로퍼티(포지션 - 리버럴, 파시스트, 히틀러) 지정
            PhotonHashtable existPlayerProperties = player.CustomProperties;

            existPlayerProperties["position"] = (Position)totalPosition[index];

            if ((Position)totalPosition[index] == Position.hitler) // 만약 히틀러라면 room property에 저장
            {
                hitler = player;
            }

            player.SetCustomProperties(existPlayerProperties);

            view.RPC("SetPositionCard", RpcTarget.All, player);

            index++;
        }

        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        // 첫 번째 대통령 지정
        existRoomProperties["president"] = PhotonNetwork.CurrentRoom.Players[playerOrder[0]];

        // 현재 순서 저장 (currentOrder 저장)
        existRoomProperties["currentOrder"] = 0;

        existRoomProperties["hitler"] = hitler; // 히틀러 저장

        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

        ShowPositionRPC(); // 역할 보여주기
    }

    public void SetPresidentandCurrentOrder(int n) // 대통령 수상 선정
    {
        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        if (n >= (int)PhotonNetwork.CurrentRoom.CustomProperties["liveCnt"] - 1) // 구성원 수 보다 커지면 인덱스 초기화 (한 바퀴 돌음)
        {
            PhotonHashtable exsistRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            existRoomProperties["currentOrder"] = 0;
            PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

            n = 0;
        }

        for (int i = n; i <= (int)PhotonNetwork.CurrentRoom.MaxPlayers; i++) // 만약 n번째 사람이 죽었다면 그 다음 산 사람 찾기
        {
            if (i == (int)PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                i = 0;
            }

            if (!(bool)PhotonNetwork.CurrentRoom.Players[playerOrder[i]].CustomProperties["dead"]) // 제일 가까운 산 사람 찾기
            {
                n = i;
                break;
            }
        }
 
        // 첫 번째 대통령 지정
        existRoomProperties["president"] = PhotonNetwork.CurrentRoom.Players[playerOrder[n]];

        // 현재 순서 저장 (currentOrder 저장)
        existRoomProperties["currentOrder"] = n;
        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);
    }

    public void ShowPickChancellorInfo(Action pickChancellor) // 수상 뽑으라고 안내
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.bellSF);

        roomNameText.text = "";

        InitPreChanImage(); // 대통령 수상 명패 초기화

        infoPanel.SetActive(false);

        // 대통령 명패 활성화
        if(((Player)PhotonNetwork.CurrentRoom.CustomProperties["president"]) == PhotonNetwork.LocalPlayer)
        {
            myCard.GetComponentsInChildren<Image>(true)[3].sprite = presidentImage;
            myCard.GetComponentsInChildren<Image>(true)[3].gameObject.SetActive(true);
        }
        else
        {
            cardDictionary[((Player)PhotonNetwork.CurrentRoom.CustomProperties["president"]).NickName].GetComponentsInChildren<Image>(true)[3].sprite = presidentImage;
            cardDictionary[((Player)PhotonNetwork.CurrentRoom.CustomProperties["president"]).NickName].GetComponentsInChildren<Image>(true)[3].gameObject.SetActive(true);
        }

        // 0: 내각구성, 1: 대통령, 2: 대통령이름, 3: 다음 대통령, 4: 대통령은 수상을 선정~
        Text[] texts = infoPanel.GetComponentsInChildren<Text>();

        texts[0].text = "내각 구성을 시작합니다"; 
        texts[1].text = "대통령";
        texts[2].text = PhotonNetwork.CurrentRoom.Players[playerOrder[(int)PhotonNetwork.CurrentRoom.CustomProperties["currentOrder"]]].NickName;
        if((int)PhotonNetwork.CurrentRoom.CustomProperties["currentOrder"] == PhotonNetwork.CurrentRoom.MaxPlayers -1)
        {
            for(int i = 0; i < (int)PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            {
                if(!(bool)PhotonNetwork.CurrentRoom.Players[playerOrder[i]].CustomProperties["dead"]) // 제일 가까운 산 사람 찾기
                {
                    texts[3].text = $"다음 대통령은 {PhotonNetwork.CurrentRoom.Players[playerOrder[i]].NickName} 입니다.";
                    break;
                }
            }
        }
        else
        {
            for (int i = (int)PhotonNetwork.CurrentRoom.CustomProperties["currentOrder"] + 1; i <= (int)PhotonNetwork.CurrentRoom.MaxPlayers; i++)
            {
                if (i == (int)PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    i = 0;
                }

                if (!(bool)PhotonNetwork.CurrentRoom.Players[playerOrder[i]].CustomProperties["dead"]) // 제일 가까운 산 사람 찾기
                {
                    texts[3].text = $"다음 대통령은 {PhotonNetwork.CurrentRoom.Players[playerOrder[i]].NickName} 입니다.";
                    break;
                }
            }
        }
        
        texts[4].text = "대통령은 수상을 선정하십시오.";

        infoPanel.SetActive(true);

        // 애니메이션 올라갈 때까지 대기 후 수상 뽑기 호출
        StartCoroutine(WaitPanelSeconds(4f, pickChancellor));
    }

    public void PickChanellorInfo() // 수상 뽑기
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.paperSF);

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
        chanPanel.SetActive(false);
        ToggleListInit(chanToggles);

        int index = 0;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if(player != PhotonNetwork.LocalPlayer)
            {
                // 현재 생존 인원이 6명 이상일 경우
                if ((int)PhotonNetwork.CurrentRoom.CustomProperties["liveCnt"] > minPlayer)
                {
                    if (!(bool)player.CustomProperties["beforePre"] && !(bool)player.CustomProperties["beforeChan"] && !(bool)player.CustomProperties["dead"])
                    {
                        chanToggles[index].GetComponentInChildren<Text>().text = player.NickName;
                        chanToggles[index].name = player.NickName;
                        chanToggles[index].gameObject.SetActive(true);

                        index++;
                    }
                }
                else
                {
                    if (!(bool)player.CustomProperties["beforeChan"] && !(bool)player.CustomProperties["dead"])
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

        existRoomProperties["chancellor"] = PhotonNetwork.CurrentRoom.Players[nameActorDictionary[selectedToggle.GetComponentInChildren<Text>().text]];
        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

        // 뽑기 완료 -> 투표하기 진행
        ShowPollInfoRPC();
    }

    private void ToggleListInit(Toggle[] toggles) // 수상 후보 토글 목록 비활성화
    {
        foreach (Toggle toggle in toggles)
        {
            toggle.gameObject.SetActive(false);
        }
    }

    public void PickNextPresidentRPC()
    {
        if (PhotonNetwork.IsMasterClient)
            view.RPC("PickNextPresident", (Player)PhotonNetwork.CurrentRoom.CustomProperties["president"]);
    }

    [PunRPC]
    public void PickNextPresident() // 다음 대통령으로 선정할 대상 지정
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.paperSF);

        roomNameText.text = "다음 대통령을 지정하십시오";
        
        nextPrePickPanel.SetActive(false);
        ToggleListInit(nextPrePickToggles);

        int index = 0;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                if (!(bool)player.CustomProperties["dead"])
                {
                    nextPrePickToggles[index].GetComponentInChildren<Text>().text = player.NickName;
                    nextPrePickToggles[index].name = player.NickName;
                    nextPrePickToggles[index].gameObject.SetActive(true);

                    index++;
                }
            }
        }

        nextPrePickPanel.SetActive(true);
    }

    public void SetNextPresident()
    {
        Toggle selectedToggle = null; // 다음 대통령으로 선정된 토글

        // 누굴 선정했는지 가져오기
        int cnt = 0;
        foreach (Toggle t in nextPrePickToggleGroup.ActiveToggles())
        {
            selectedToggle = t;
            cnt++;
            break;
        }

        if (cnt == 0) return; // 다음 대통령이 선택되지 않았음

        // 선택된 유저의 순서 찾기
        int index = 0;
        for(int i = 0; i < playerOrder.Length; i++)
        {
            if(playerOrder[i] == nameActorDictionary[selectedToggle.GetComponentInChildren<Text>().text])
            {
                index = i;
                break;
            }
        }

        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        existRoomProperties["currentOrder"] = index;
        existRoomProperties["president"] = PhotonNetwork.CurrentRoom.Players[playerOrder[index]];

        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

        // 다음 라운드 진행
        StartCoroutine(WaitPanelSeconds(3f, PassNextTurnRPC));

    }

    [PunRPC]
    public void ShowPollInfo()
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.bellSF);

        // 수상 명패 활성화
        if (((Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"]) == PhotonNetwork.LocalPlayer)
        {
            myCard.GetComponentsInChildren<Image>(true)[3].sprite = chancellorImage;
            myCard.GetComponentsInChildren<Image>(true)[3].gameObject.SetActive(true);
        }
        else
        {
            cardDictionary[((Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"]).NickName].GetComponentsInChildren<Image>(true)[3].sprite = chancellorImage;
            cardDictionary[((Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"]).NickName].GetComponentsInChildren<Image>(true)[3].gameObject.SetActive(true);
        }

        chanPanel.SetActive(false);
        infoPanel.SetActive(false);

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
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["dead"]) return; // 죽은 사람은 투표하지 못한다.

        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.paperSF);

        pollResultHash.Clear();

        if (PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["president"] ||
            PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["chancellor"])
        {
            roomNameText.text = "의원들의 투표를 기다리는 중 입니다...";
            return;
        }

        // 초기화
        roomNameText.text = "투표를 진행해주십시오";
        pollFinBtn.gameObject.GetComponentInChildren<Text>().text = "투표 완료";
        myJaNein = -1;

        pollCardsBtn[0].interactable = true;
        pollCardsBtn[1].interactable = true;

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

            view.RPC("SetCountDown", RpcTarget.All, true);

            view.RPC("SendPollResultToMaster", RpcTarget.MasterClient, 
                PhotonNetwork.NickName, myJaNein, 0); // 마스터 클라이언트에게 투표 결과 전송
        }
        else  // 투표 완료 -> 미완료
        {
            pollCardsBtn[0].interactable = true;
            pollCardsBtn[1].interactable = true;

            pollFinBtn.gameObject.GetComponentInChildren<Text>().text = "투표 완료";

            view.RPC("SetCountDown", RpcTarget.All, false);

            view.RPC("SendPollResultToMaster", RpcTarget.MasterClient,
                PhotonNetwork.NickName, myJaNein, 1); // 마스터 클라이언트에게 투표 취소 전송
        }
    }

    [PunRPC]
    public void SendPollResultToMaster(string name, int result, int c) // 마스터 클라이언트가 투표 결과를 확인
    {
        if(c == 0) // 투표 했다면
        {
            pollResultHash[name] = result;
        }
        else // 투표 취소했다면
        {
            pollResultHash.Remove(name);
        }

        if(pollResultHash.Count == ((int)PhotonNetwork.CurrentRoom.CustomProperties["liveCnt"] - 2)) // 모든 사람이 투표를 완료 했다면
        {
            view.RPC("StartCountDown", RpcTarget.All); // 모두가 카운트다운 시작
        }
    }

    [PunRPC]
    public void StartCountDown()
    {
        StartCoroutine(WaitCountDown(5f, CalculPollResult));
    }

    [PunRPC]
    public void SetCountDown(bool b) // 카운트다운을 멈추거나 동작하기 위한 함수
    {
        isStopCountdown = b;

        if(!b) // 만약 카운트다운을 멈춘다면 카운트다운 오브젝트도 활성화
        {
            countDownObj.SetActive(false);
        }
    }

    public void CalculPollResult()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            string resultS = ""; // 투표 결과를 문자열로 저장하여 전송
                                 // "NickName 0 NickName 1 ..." (" "으로 구분)

            int jaCnt = 0;
            int neinCnt = 0;
            foreach (DictionaryEntry d in pollResultHash)
            {
                resultS += $"{d.Key} {d.Value} ";

                if ((int)d.Value == 0) jaCnt++;
                if ((int)d.Value == 1) neinCnt++;
            }

            if (jaCnt > neinCnt) // 내각 구성 성공
            {
                InitMarker(); // 추적용 마커 초기화

                view.RPC("ShowPollResult", RpcTarget.All, resultS, 0);
            }
            else // 내각 구성 실패
            {
                PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                existRoomProperties["marker"] = (int)existRoomProperties["marker"] + 1; // 마커 한 칸 전진

                PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

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
        SoundManager.Instance.bgmAS.Stop();

        pollResultPanel.SetActive(false);
        roomNameText.text = "";

        if (result == 0) // 내각 구성에 성공했다면 투표 진행
        {
            InitMarkerActive(); // 마커 오브젝트 초기화

            // 파시즘 정책이 3개 이상 발의되었고 수상이 히틀러이라면 종료
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] >= 3
                && (Position)((Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"]).CustomProperties["position"] == Position.hitler)
            {
                StartCoroutine(WaitPanelSeconds(3f, () => { PacistWin("파시즘 정책 3개 이상 발의 후 히틀러 수상"); }));
            }
            else // 정책 뽑기
            {
                SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.charmBellSF);

                infoPanel.SetActive(false);

                // 정책 뽑는다 info panel 나오기
                Text[] texts = infoPanel.GetComponentsInChildren<Text>();

                Player president = (Player)PhotonNetwork.CurrentRoom.CustomProperties["president"];
                Player chancellor = (Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"];

                texts[0].text = "신성한 의회 단계입니다";
                texts[1].text = "대통령, 수상";
                texts[2].text = $"{president.NickName}, {chancellor.NickName}";
                texts[3].text = "모두의 채팅이 중지됩니다.";
                texts[4].text = "대통령과 수상은 정책을 선택해주십시오.";

                chatInputField.interactable = false;

                infoPanel.SetActive(true);

                // president한테 투표하라고 함수 호출
                if(PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["president"])
                {
                    StartCoroutine(WaitPanelSeconds(4f, PickPolicyByPresident));
                }
                else // 아닐 시 대기
                {
                    roomNameText.text = "신성한 의회 단계입니다 정숙해주세요";
                    StartCoroutine(WaitPanelSeconds(4f, () => { infoPanel.SetActive(false); }));
                }
            }
        }
        else // 내각 구성에 실패했다면 추적용 마커 1칸 전진
        {
            SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.bellSF);

            PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            int m = (int)existRoomProperties["marker"];

            for (int i = 0; i < markers.Length; i++) // 마커 활성화 초기화
            {
                markers[i].SetActive(false);
            }

            markers[m].SetActive(true); //마커 한 칸 전진한 이미지 활성화

            if (m == 3) // 연속 3번 무산이라면
            {
                infoPanel.SetActive(false);
                // panel 보여주기
                Text[] texts = infoPanel.GetComponentsInChildren<Text>();

                texts[0].text = "내각이 무산 되었습니다";
                texts[1].text = "";
                texts[2].text = $"추적용 마커 연속 3칸 이동";
                texts[3].text = "";
                texts[4].text = "가장 상위 더미 정책이 강제로 발의됩니다.";

                infoPanel.SetActive(true);

                if(PhotonNetwork.IsMasterClient) // 마스터 클라이언트가 바로 위 정책이 발의
                {
                    StartCoroutine(WaitPanelSeconds(5f, ForceMotion));
                }
            }
            else
            {
                infoPanel.SetActive(false);

                // panel 보여주기
                Text[] texts = infoPanel.GetComponentsInChildren<Text>();

                texts[0].text = "내각이 무산 되었습니다";
                texts[1].text = "";
                texts[2].text = $"추적용 마커 1 전진";
                texts[3].text = $"현재 연속 무산 횟수 {m}번";
                texts[4].text = "다음 내각을 구성합니다.";

                infoPanel.SetActive(true);

                StartCoroutine(WaitPanelSeconds(5f, StartNewTurn)); // 다음 내각 구성
            }
        }
    }

    public void PacistWin(string reason)
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.bellSF);

        baseImg.sprite = endingBase[0]; // 파시즘 배경으로 교환

        Text[] texts = endingPanel.GetComponentsInChildren<Text>();
        texts[0].text = "파시스트가 승리하였습니다";
        texts[1].text = reason;

        ShowAllPositionCard();

        endingPanel.SetActive(true);

        StartCoroutine(WaitPanelSeconds(8f, InitWhenJoinedRoom));
    }

    public void ForceMotion()
    {
        // 인덱스 설정
        int m = (int)PhotonNetwork.CurrentRoom.CustomProperties["marker"];

        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        InitPre(); // 이전 대통령, 수상 여부 초기화

        int policyIdx = (int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"];

        if (policyIdx > totalPolicyNum - 1) // 남은 정책 개수가 부족한지 확인
        {
            // 정책 섞기
            SufflePolicy();
            PassSufflePolicyRPC(policyArray);

            // 정책 인덱스 초기화
            policyIdx = 0;
        }

        existRoomProperties["policyIdx"] = policyIdx + 1;
        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

        InitMarker(); // 마커 초기화

        view.RPC("ShowPickedPolicy", RpcTarget.All, 0, 1); // 정책 보여주기
    }

    public void InitMarker() // 추적용 마커 초기화
    {
        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        existRoomProperties["marker"] = 0;

        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);
    }

    public void InitMarkerActive() // 추적용 마커 오브젝트 초기화
    {
        for (int i = 0; i < markers.Length; i++) // 마커 활성화 초기화
        {
            markers[i].SetActive(false);
        }

        markers[0].SetActive(true); //마커 이미지 초기화
    }

    public void ShowAllPositionCard()
    {
        foreach(GameObject obj in cardDictionary.Values)
        {
            obj.SetActive(true);
        }
    }

    public void PickPolicyByPresident() // 대통령 정책 뽑기
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.paperSF);
        roomNameText.text = "버릴 정책을 선택해주세요";

        infoPanel.SetActive(false);

        InitPre(); // 이전 대통령, 수상 여부 초기화
        PolicyActiveInit(); // 정책카드 활성화

        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        PhotonHashtable existPlayerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

        int policyIdx = (int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"];

        if (policyIdx > totalPolicyNum - 3) // 남은 정책 개수가 부족한지 확인
        {
            // 정책 섞기
            SufflePolicy();
            PassSufflePolicyRPC(policyArray);

            // 정책 인덱스 초기화
            policyIdx = 0;
        }

        // 정책 카드 이미지 설정
        policyImages[0].sprite = policyImg[policyArray[policyIdx]];
        policyImages[1].sprite = policyImg[policyArray[policyIdx + 1]];
        policyImages[2].sprite = policyImg[policyArray[policyIdx + 2]];

        existRoomProperties["policyIdx"] = policyIdx + 3;
        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

        // 이전 대통령 여부 설정
        existPlayerProperties["beforePre"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(existPlayerProperties);

        policyPanel.SetActive(true);

    }

    public void InitPre()
    {
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            PhotonHashtable h = player.CustomProperties;
            h["beforePre"] = false;
            h["beforeChan"] = false;

            player.SetCustomProperties(h);
        }
    }

    public void LeavePolicy(int n)
    {
        // 대통령이라면 수상에게 넘기기
        if(PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["president"])
        {
            policyPanel.SetActive(false);

            roomNameText.text = $"{((Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"]).NickName}이(가) 정책 선정 중입니다";

            // 수상에게 넘기기
            view.RPC("PickPolicyByChancellor", (Player)PhotonNetwork.CurrentRoom.CustomProperties["chancellor"], n);
        }
        else if (PhotonNetwork.LocalPlayer == PhotonNetwork.CurrentRoom.CustomProperties["chancellor"]) // 수상이라면 정책 발의
        {
            policyPanel.SetActive(false);

            // 정책 발의
            view.RPC("ShowPickedPolicy", RpcTarget.All, n, 0);
        }
    }

    [PunRPC]
    public void PickPolicyByChancellor(int n)
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.paperSF);

        policyPanel.SetActive(false);

        // 이전 수상 여부 설정
        PhotonHashtable existPlayerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        existPlayerProperties["beforeChan"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(existPlayerProperties);

        roomNameText.text = "발의할 정책을 선택하여 주십시오";

        PolicyActiveInit();

        int policyIdx = (int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"];

        // 정책 카드 이미지 설정
        policyImages[0].sprite = policyImg[policyArray[policyIdx - 3]];
        policyImages[1].sprite = policyImg[policyArray[policyIdx - 2]];
        policyImages[2].sprite = policyImg[policyArray[policyIdx - 1]];

        policyBtn[n].gameObject.SetActive(false); // 버려진 카드 비활성화

        policyPanel.SetActive(true);
    }

    /// <summary>
    /// 정책을 보여주는 PunRPC 메서드 입니다.
    /// </summary>
    /// <param name="n">왼쪽부터 0, 1, 2라고 했을 때 뽑힌 정책의 인덱스</param>
    /// <param name="c">c == 0: 일반 정책 발의, c == 1: 강제 정책 발의</param>
    [PunRPC]
    public void ShowPickedPolicy(int n, int c) // 정책 보여주기
    {
        InitMarkerActive(); // 마커 초기화
        SoundManager.Instance.bgmAS.Play();

        roomNameText.text = "";
        chatInputField.interactable = true;

        int curPickedPolicy;
        if (c == 0)
            curPickedPolicy = policyArray[(int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"] - 3 + n];
        else
            curPickedPolicy = policyArray[(int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"] - 1];

        // 결과 안내
        infoPanel.SetActive(false);
        roomNameText.text = "정책 발의가 완료되었습니다";

        // 정책 결과 안내
        Text[] textInfo = infoPanel.GetComponentsInChildren<Text>();

        if (curPickedPolicy == 0) // 뽑힌 정책이 리버럴이라면
        {
            int idx = (int)PhotonNetwork.CurrentRoom.CustomProperties["liberalPolicy"]; // 보드판에 깔기
            pickedLiberal[idx].SetActive(true);

            // 리버럴 정책 개수 증가
            PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            existRoomProperties["liberalPolicy"] = (int)existRoomProperties["liberalPolicy"] + 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

            if((int)existRoomProperties["liberalPolicy"] > 3)
            {
                SoundManager.Instance.bgmAS.Stop();
                SoundManager.Instance.PlayBGM(SoundManager.Instance.liberalBGM);
            }

            SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.bellSF);

            // 정책 결과 안내
            textInfo[0].text = "이번 의회 결과는 리버럴입니다";
            textInfo[1].text = "";
            textInfo[2].text = $"사용된 정책 수 {(int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"]}"; // 버려진 정책 수
            textInfo[3].text = "";
            textInfo[4].text = "";

            infoPanel.SetActive(true);

            StartCoroutine(WaitPanelSeconds(4f, () => { LiberalPolicyResult(); }));
            
        }
        else // 뽑힌 정책이 파시즘이라면
        {
            int idx = (int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"]; // 보드판에 깔기
            pickedPacist[idx].SetActive(true);

            // 파시즘 정책 개수 증가
            PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            existRoomProperties["pacismPolicy"] = (int)existRoomProperties["pacismPolicy"] + 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

            if((int)existRoomProperties["pacismPolicy"] > 2)
            {
                SoundManager.Instance.bgmAS.Stop();
                SoundManager.Instance.PlayBGM(SoundManager.Instance.pacistBGM);
            }

            SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.bellSF);

            // 정책 결과 안내
            textInfo[0].text = "이번 의회 결과는 파시즘입니다";
            textInfo[1].text = "";
            textInfo[2].text = $"사용된 정책 수 {(int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"]}"; // 남은 정책
            textInfo[3].text = "";
            textInfo[4].text = "";

            infoPanel.SetActive(true);

            StartCoroutine(WaitPanelSeconds(4f, PacismPolicyResult));
            
        }
    }

    public void PacismPolicyResult() // 파시즘 정책에 따른 결과
    {
        infoPanel.SetActive(false);

        Text[] textInfo = infoPanel.GetComponentsInChildren<Text>();

        int special = ReturnPresidentSpecial();
        if((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 6) // 파시즘 정책 6개 발의로 승리
        {
            StartCoroutine(WaitPanelSeconds(3f, () => { PacistWin("파시즘 정책 6개 발의"); }));
        }
        else if (special == -1) // 대통령 특수 권한 없음
        {
            StartCoroutine(WaitPanelSeconds(4f, StartNewTurn));
        }
        else if(special == (int)PresidentSpecial.Policy)// 대통령 특수 권한 실행 - 정책 확인
        {
            SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.typeBellSF);
            roomNameText.text = "대통령 특수 권한 실행 - 정책 미리 보기";
           
            textInfo[0].text = "대통령 특수 권한 실행";
            textInfo[1].text = "";
            textInfo[2].text = "정책 미리보기";
            textInfo[3].text = "";
            textInfo[4].text = "다음 3개의 정책을 본인만 미리 확인합니다.";

            infoPanel.SetActive(true);

            StartCoroutine(WaitPanelSeconds(4f, CheckPolicyRPC));
        }
        else if (special == (int)PresidentSpecial.Identity)// 대통령 특수 권한 실행 - 신분 확인
        {
            SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.typeBellSF);
            roomNameText.text = "대통령 특수 권한 실행 - 신분 확인";

            textInfo[0].text = "대통령 특수 권한 실행";
            textInfo[1].text = "";
            textInfo[2].text = "신분 확인";
            textInfo[3].text = "";
            textInfo[4].text = "다른 한 사람의 신분을 본인만 확인합니다.";

            infoPanel.SetActive(true);

            StartCoroutine(WaitPanelSeconds(4f, CheckIdentityRPC));
        }
        else if (special == (int)PresidentSpecial.NextPresident)// 대통령 특수 권한 실행 - 다음 대통령 지정
        {
            SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.typeBellSF);
            roomNameText.text = "대통령 특수 권한 실행 - 다음 대통령 지정";

            textInfo[0].text = "대통령 특수 권한 실행";
            textInfo[1].text = "";
            textInfo[2].text = "다음 대통령 지정";
            textInfo[3].text = "";
            textInfo[4].text = "다음 내각의 대통령을 지정합니다.";

            infoPanel.SetActive(true);

            StartCoroutine(WaitPanelSeconds(4f, PickNextPresidentRPC));

        }
        else if (special == (int)PresidentSpecial.Kill)// 대통령 특수 권한 실행 - 처형
        {
            SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.typeBellSF);
            roomNameText.text = "대통령 특수 권한 실행 - 처형";

            textInfo[0].text = "대통령 특수 권한 실행";
            textInfo[1].text = "";
            textInfo[2].text = "처형";
            textInfo[3].text = "";
            textInfo[4].text = "다른 한 사람을 처형합니다.";

            infoPanel.SetActive(true);

            StartCoroutine(WaitPanelSeconds(4f, KillRPC));
        }

    }

    public void KillRPC()
    {
        if (PhotonNetwork.IsMasterClient)
            view.RPC("ShowKillCandidate", (Player)PhotonNetwork.CurrentRoom.CustomProperties["president"]);
    }

    [PunRPC]
    public void ShowKillCandidate() // 토글 그룹에 처형 대상 목록 출력
    {
        killPanel.SetActive(false);
        ToggleListInit(killToggles);

        int index = 0;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player != PhotonNetwork.LocalPlayer && !(bool)player.CustomProperties["dead"])
            {
                killToggles[index].GetComponentInChildren<Text>().text = player.NickName;
                killToggles[index].name = player.NickName;
                killToggles[index].gameObject.SetActive(true);

                index++;
            }
        }

        killToggles[0].isOn = true;

        killPanel.SetActive(true);
    }

    public void KillSomeone()
    {
        Toggle selectedToggle = null;

        // 누굴 선정했는지 가져오기
        int cnt = 0;
        foreach (Toggle t in killToggleGroup.ActiveToggles())
        {
            selectedToggle = t;
            cnt++;
            break;
        }

        if (cnt == 0) return; // 처형 대상이 선택되지 않았음

        PhotonHashtable existPlayerProperties = PhotonNetwork.CurrentRoom.Players[nameActorDictionary[selectedToggle.GetComponentInChildren<Text>().text]].CustomProperties;

        existPlayerProperties["dead"] = true;
        PhotonNetwork.CurrentRoom.Players[nameActorDictionary[selectedToggle.GetComponentInChildren<Text>().text]].SetCustomProperties(existPlayerProperties);

        // LiveCnt 변경
        PhotonHashtable existRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        existRoomProperties["liveCnt"] = (int)existRoomProperties["liveCnt"] - 1;
        PhotonNetwork.CurrentRoom.SetCustomProperties(existRoomProperties);

        killPanel.SetActive(false);

        // 모든 사람의 프로필에 죽은 사람의 네임 카드 변경
        view.RPC("ChangeNameCardForDead", RpcTarget.All, selectedToggle.GetComponentInChildren<Text>().text);
    }

    [PunRPC]
    public void ChangeNameCardForDead(string name) // 모든 사람의 프로필에 죽은 사람의 네임 카드 변경
    {
        // 총소리
        SoundManager.Instance.bgmAS.Pause();
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.killBellSF);

        roomNameText.text = $"{name}이(가) 사망하였습니다";

        if (PhotonNetwork.LocalPlayer.NickName == name)
        {
            myCard.GetComponent<Image>().sprite = deadImage;
        }
        else
        {
            cardDictionary[name].GetComponent<Image>().sprite = deadImage;
        }

        // 만약 죽은 사람이 히틀러라면 리버럴의 승리
        if(PhotonNetwork.CurrentRoom.Players[nameActorDictionary[name]] == PhotonNetwork.CurrentRoom.CustomProperties["hitler"])
        {
            StartCoroutine(WaitPanelSeconds(4f, () => { LiberalWin("히틀러 처형"); }));
        }
        else
        {
            StartCoroutine(WaitPanelSeconds(4f, () => { SoundManager.Instance.bgmAS.Play(); StartNewTurn(); }));
        }
    }

    public void CheckIdentityRPC()
    {
        if (PhotonNetwork.IsMasterClient)
            view.RPC("CheckIdentity", (Player)PhotonNetwork.CurrentRoom.CustomProperties["president"]);
    }

    [PunRPC]
    public void CheckIdentity()
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.paperSF);

        InitIdentityCards();
        identitySelectPanel.SetActive(false);
        
        roomNameText.text = "신분을 확인할 사람을 선택하십시오";

        int index = 0;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // 확인하려는 사람은 자신이 아니며 살아있어야 한다.
            if (player != PhotonNetwork.LocalPlayer && !(bool)player.CustomProperties["dead"])
            {
                identityNamesList[index] = player.NickName;
                identityBtns[index].GetComponentInChildren<Text>().text = player.NickName;
                identityBtns[index].SetActive(true);

                index++;
            }
        }

        identitySelectPanel.SetActive(true);
    }

    public void ShowIdentity(string name)
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.typeBellSF);

        view.RPC("ShowPresidentCheckWho", RpcTarget.All, name);

        identitySelectPanel.SetActive(false);
        showIdentityPanel.SetActive(false);

        Image identityImage = showIdentityPanel.GetComponentsInChildren<Image>()[1]; // 신분 이미지
        Text[] texts = showIdentityPanel.GetComponentsInChildren<Text>(); // 0: 이름, 1: 신분

        int actorNumber = nameActorDictionary[name];

        cardDictionary[name].transform.GetChild(3).gameObject.SetActive(true);

        identityImage.sprite = cardDictionary[name].transform.GetChild(3).gameObject.GetComponent<Image>().sprite; // 신분 이미지 설정
        texts[0].text = $"{name}는(은)";
        if((Position)PhotonNetwork.CurrentRoom.Players[actorNumber].CustomProperties["position"] == Position.liberal)
        {
            texts[1].color = liberalColor;
            texts[1].text = "리버럴입니다.";
        }
        else if ((Position)PhotonNetwork.CurrentRoom.Players[actorNumber].CustomProperties["position"] == Position.pacist)
        {
            texts[1].color = pacistColor;
            texts[1].text = "파시스트입니다.";
        }
        else if ((Position)PhotonNetwork.CurrentRoom.Players[actorNumber].CustomProperties["position"] == Position.hitler)
        {
            texts[1].color = hitlerColor;
            texts[1].text = "히틀러입니다.";
        }

        showIdentityPanel.SetActive(true);

        StartCoroutine(WaitPanelSeconds(4f, StartNewTurnRPC));
    }

    [PunRPC]
    public void ShowPresidentCheckWho(string s)
    {
        roomNameText.text = $"대통령이 {s}의 신분을 확인했습니다";
    }

    public void CheckPolicyRPC() // 대통령 특수 권한 - 신분 확인
    {
        roomNameText.text = "대통령이 다음 정책을 미리 확인하고 있습니다";

        if(PhotonNetwork.IsMasterClient)
            view.RPC("CheckPolicy", (Player)PhotonNetwork.CurrentRoom.CustomProperties["president"]);
    }

    public void InitIdentityCards() // 신분 버튼 비활성화
    {
        foreach(GameObject obj in identityBtns)
        {
            obj.SetActive(false);
        }
    }

    [PunRPC]
    public void CheckPolicy() // 대통령 특수 권한 - 정책 확인하기
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.paperSF);

        policyPeekPanel.SetActive(false);

        roomNameText.text = "다음 정책을 미리 확인하십시오";

        int policyIdx = (int)PhotonNetwork.CurrentRoom.CustomProperties["policyIdx"];

        if (policyIdx > totalPolicyNum - 3) // 남은 정책 개수가 부족한지 확인
        {
            // 정책 섞기
            SufflePolicy();
            PassSufflePolicyRPC(policyArray);

            // 정책 인덱스 초기화
            policyIdx = 0;
        }

        // 정책 카드 이미지 설정
        policyPeekImages[0].sprite = policyImg[policyArray[policyIdx]];
        policyPeekImages[1].sprite = policyImg[policyArray[policyIdx + 1]];
        policyPeekImages[2].sprite = policyImg[policyArray[policyIdx + 2]];

        policyPeekPanel.SetActive(true);

        StartCoroutine(WaitPanelSeconds(4f, StartNewTurnRPC));
    }

    public void LiberalPolicyResult() // 리버럴 정책에 따른 결과
    {
        infoPanel.SetActive(false);

        // 리버럴 엔딩 판정
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["liberalPolicy"] == 5)
        {
            StartCoroutine(WaitPanelSeconds(4f, () => { LiberalWin("리버럴 정책 5개 발의"); }));
        }
        else // 결과 안내 후 다음 내각 의회 구성
        {
            // 다음 대통령 지정
            StartNewTurn();
        }
    }

    public void LiberalWin(string reason)
    {
        SoundManager.Instance.PlaySoundEffect2(SoundManager.Instance.bellSF);

        roomNameText.text = $"리버럴 승리";

        infoPanel.SetActive(false);

        baseImg.sprite = endingBase[1]; // 리버럴 배경으로 교환

        Text[] texts = endingPanel.GetComponentsInChildren<Text>();
        texts[0].text = "리버럴이 승리하였습니다";
        texts[1].text = reason;

        ShowAllPositionCard();

        endingPanel.SetActive(true);

        StartCoroutine(WaitPanelSeconds(8f, InitWhenJoinedRoom));
    }

    public void StartNewTurnRPC()
    {
        showIdentityPanel.SetActive(false); // 신분 보기 비활성화
        policyPeekPanel.SetActive(false); // 정책 미리보기 비활성화

        view.RPC("StartNewTurn", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void StartNewTurn()
    {
        // 다음 대통령 지정은 마스터 클라이언트가 진행
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            int co = (int)PhotonNetwork.CurrentRoom.CustomProperties["currentOrder"];

            SetPresidentandCurrentOrder(co + 1);

            PassNextTurnRPC();
        }
    }

    [PunRPC]
    public void PassNextTurn()
    {
        ShowPickChancellorInfo(PickChanellorInfo);
    }

    public void PassNextTurnRPC()
    {
        nextPrePickPanel.SetActive(false); // 다음 대통령 지정 패널 비활성화

        view.RPC("PassNextTurn", RpcTarget.All);
    }

    public int ReturnPresidentSpecial() // 대통령 특별 권한 동작 반환
    {
        if(PhotonNetwork.CurrentRoom.MaxPlayers < 7)
        {
            if((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 3) // 정책 미리보기
            {
                return (int)PresidentSpecial.Policy;
            }
            else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 4 || (int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 5) // 처형
            {
                return (int)PresidentSpecial.Kill;
            }
        }
        else if(PhotonNetwork.CurrentRoom.MaxPlayers < 9)
        {
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 2) // 신분 확인
            {
                return (int)PresidentSpecial.Identity;
            }
            else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 3) // 다음 대통령 지정
            {
                return (int)PresidentSpecial.NextPresident;
            }
            else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 4 || (int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 5) // 처형
            {
                return (int)PresidentSpecial.Kill;
            }
        }
        else
        {
            if ((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 1 || (int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 2) // 신분 확인
            {
                return (int)PresidentSpecial.Identity;
            }
            else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 3) // 다음 대통령 지정
            {
                return (int)PresidentSpecial.NextPresident;
            }
            else if ((int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 4 || (int)PhotonNetwork.CurrentRoom.CustomProperties["pacismPolicy"] == 5) // 처형
            {
                return (int)PresidentSpecial.Kill;
            }
        }

        return -1;
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


    [PunRPC]
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

    public void InitPreChanImage() // 대통령, 수상 이미지 초기화
    {
        foreach(GameObject nameCard in cardDictionary.Values)
        {
            nameCard.GetComponentsInChildren<Image>(true)[3].gameObject.SetActive(false);
        }

        myCard.GetComponentsInChildren<Image>(true)[3].gameObject.SetActive(false);
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

        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.charmBellSF);

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

        SoundManager.Instance.PlayBGM(SoundManager.Instance.defaultBGM);

        ShowPickChancellorInfo(PickChanellorInfo);

        yield break;
    }

    private IEnumerator WaitPanelSeconds(float s, Action callback)
    {
        yield return new WaitForSeconds(s);

        callback?.Invoke();
    }

    private IEnumerator WaitCountDown(float s, Action callback)
    {
        countDownObj.SetActive(true);

        float sumTime = 0f;
        while(isStopCountdown && sumTime <= s)
        {
            sumTime += Time.deltaTime;
            countDownText.text = (s - (int)sumTime).ToString();

            yield return null;
        }

        if(isStopCountdown)
        {
            countDownObj.SetActive(false);
            callback?.Invoke();
        }
    }
}
