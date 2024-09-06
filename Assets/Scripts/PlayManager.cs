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

public class PlayManager : MonoBehaviourPunCallbacks // �̱������� �ø����ٰ� �� �� ���Ƽ� ����
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

    public Sprite[] posCardImg; // �ź� ī�� (0: hitler, 1~2: pacist, 3~9: liberal)

    public GameObject posInfoTextObj; // ����� �ź���?
    public GameObject[] posObj; // �ź� ���� ������Ʈ (0: ������, 1: �Ľý�Ʈ, 2: ��Ʋ��)
    public Text[] pacistListTexts; // ���� ���� ��� �Ľý�Ʈ ����Ʈ

    public GameObject[] pacistBoads; // �Ľý�Ʈ ������ 0: 5~6, 1: 7~8, 2: 9~10

    public GameObject infoPanel; // ���� ����, ���� ��ǥ �� ���� ��Ȳ�� �˷��ִ� �ǳ�

    public int[] playerOrder; // �÷��̾� ���� - Actor Number�� ����

    public int currentOrder; // ���� ���� ��������� - playerOrder�� �ε����� ���

    private int totalPolicyNum = 17;
    public int[] policyArray = new int[17]; // ��å �迭 (0: liberal, 1: pacist) liberal 6��, pacist 11��

    public void InitWhenJoinedRoom()
    {
        // ������ �ο����� �°� ����
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

        // ���̵��ξƿ� �� �̹��� ���İ� 0���� �ʱ�ȭ
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;

        foreach (Text t in pacistListTexts)
        {
            t.text = "";
        }

        // ��ư�� Ȱ��ȭ
        readyButton.gameObject.SetActive(true);
        outButton.gameObject.SetActive(true);

        // �ڽ��� �ź� ī�� ��Ȱ��ȭ
        myPosObj.gameObject.SetActive(false);

        // �ٸ� ����� �ź� ī�� ��Ȱ��ȭ
        foreach(GameObject obj in cardDictionary.Values)
        {
            obj.transform.GetChild(3).gameObject.SetActive(false);
        }

        // info panel ��Ȱ��ȭ
        infoPanel.SetActive(false);
    }

    public int[] SufflePolicy() // ��å �迭 ����
    {
        int liberalCnt = 6;
        int pacistCnt = 11;

        for(int i = 0; i < totalPolicyNum;)
        {
            int policy = UnityEngine.Random.Range(0, 2); // �����ϰ� ��å �̱� (0: liberal, 1: pacist)
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

    public int[] SetPlayerOrder() // �÷��� ���� ���ϱ�
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

    public void PickPosition() // ���� �̱�
    {
        totalPosition = new int[PhotonNetwork.CurrentRoom.MaxPlayers];
        for(int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++) // ���� �湮 �迭 �ʱ�ȭ
        {
            isSelected.Add(false);
            totalPosition[i] = (int)Position.liberal;
        }

        if (!PhotonNetwork.IsMasterClient) return; // ������ Ŭ���̾�Ʈ(����)�� ����

        int liberalCnt = PhotonManager.Instance.cntDictionary[PhotonNetwork.CurrentRoom.MaxPlayers].Item1;
        int pacistCnt = PhotonManager.Instance.cntDictionary[PhotonNetwork.CurrentRoom.MaxPlayers].Item2;

        int totalCnt = liberalCnt + pacistCnt;

        for (int i = 0; i < totalCnt;) // List�� ������ �����ϰ� ����
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

    // TODO: ���� ���� �� ���� �̿Ϸ� ������ �־����
    [PunRPC]
    public void StateInitForGameStart() // ���� ���� �� ���� ���� ���� �����
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

    public void SendPickPosition() // ������ �̱�
    {
        int index = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // �÷��̾� Ŀ���� ������Ƽ(������ - ������, �Ľý�Ʈ, ��Ʋ��) ����
            PhotonManager.Instance.playerProperties["position"] = (Position)totalPosition[index];
            player.SetCustomProperties(PhotonManager.Instance.playerProperties);

            index++;
        }
    }

    public void ShowPickChancellorInfo(Action pickChancellor) // ���� ������� �ȳ�
    {
        // 0: ��������, 1: �����, 2: ������̸�, 3: ���� �����, 4: ������� ������ ����~
        Text[] texts = infoPanel.GetComponentsInChildren<Text>();

        texts[0].text = "���� ����"; 
        texts[1].text = "�����"; 
        texts[2].text = PhotonNetwork.CurrentRoom.Players[playerOrder[currentOrder]].NickName; // TODO: currentOrder ������ �� 0���� �ʱ�ȭ
        texts[3].text = $"���� ������� {PhotonNetwork.CurrentRoom.Players[playerOrder[currentOrder+1]].NickName} �Դϴ�."; 
        texts[4].text = "������� ������ �����Ͻʽÿ�.";

        infoPanel.SetActive(true);

        // ���� �̱� �Լ� ȣ��
        pickChancellor?.Invoke();
    }

    public void PickChanellorInfo() // ���� �̱�
    {
        // ������̶�� ���� ����
        if((Player)PhotonNetwork.CurrentRoom.CustomProperties["president"] == PhotonNetwork.LocalPlayer)
        {
            roomNameText.text = "������ �������ֽʽÿ�.";
        }
        else // �ƴ϶�� ���
        {
            roomNameText.text = $"{PhotonNetwork.CurrentRoom.Players[playerOrder[currentOrder]].NickName}�� ������ ���� ���Դϴ�.";
        }
    }

    public void SetPositionCard(Player player)
    {
        // �÷��̾� UI�� ���̴� ���� ī�� ��������Ʈ ��ü
        int spriteIdx = 0;

        if ((Position)player.CustomProperties["position"] == Position.liberal)
        {
            spriteIdx = UnityEngine.Random.Range(3, 10);
        }
        else if ((Position)player.CustomProperties["position"] == Position.pacist)
        {
            spriteIdx = UnityEngine.Random.Range(1, 3);
        }
        else
        {
            spriteIdx = 0;
        }

        if (player == PhotonNetwork.LocalPlayer)
        {
            myPosObj.gameObject.GetComponent<Image>().sprite = posCardImg[spriteIdx];
        }
        else
        {
            cardDictionary[player.NickName].transform.GetChild(3).gameObject.GetComponent<Image>().sprite
            = posCardImg[spriteIdx];
        }
    }

    public void ShowPositionRPC() // ���� ���� �� �ź� �����ֱ�
    {
        view.RPC("ShowPosition", RpcTarget.All);
    }

    [PunRPC]
    public void ShowPosition()
    {
        StartCoroutine(GameStartIntro());
    }

    public void KickAllPlayerRPC() // ���� �� �� ������ �� ���ֱ�
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

    private IEnumerator GameStartIntro() // ���� ���� �� �ź� ��Ʈ��
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

        // �ź� ���� ������Ʈ Ȱ��ȭ
        posInfoTextObj.SetActive(true);
        posObj[(int)PhotonNetwork.LocalPlayer.CustomProperties["position"]].SetActive(true);

        // �ڽ��� �ź� ī�� Ȱ��ȭ
        // TODO: ���� ���� �� �ٽ� ��Ȱ��ȭ �ؾߵ�
        myPosObj.gameObject.SetActive(true);

        if ((Position)PhotonNetwork.LocalPlayer.CustomProperties["position"] == Position.pacist) // �Ľý�Ʈ�� ���
        {
            int index = 0;
            foreach(Player player in PhotonNetwork.CurrentRoom.Players.Values) // �ź� ���� �� �Ľý�Ʈ ����Ʈ ���
            {
                if(player != PhotonNetwork.LocalPlayer && (Position)player.CustomProperties["position"] == Position.pacist)
                {
                    pacistListTexts[index].text = player.NickName;

                    // �Ľý�Ʈ���� �ź�ī�� Ȱ��ȭ
                    // TODO: ���� ���� �� �ٽ� ��Ȱ��ȭ �ؾߵ�
                    cardDictionary[player.NickName].transform.GetChild(3).gameObject.SetActive(true);

                    index++;
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
