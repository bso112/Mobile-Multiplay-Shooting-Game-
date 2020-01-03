using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

/// <summary>
/// 스폰할 캐릭터들에 대한 정보
/// </summary>
public class PlayerInfo
{
    public byte Id { get; set; }

    public int team;
    public string character;
    public int? actorNum;
    public string nickname;
    /// <summary>
    /// 캐릭터가 스폰된 포지션. 리스폰에 쓰인다.
    /// </summary>
    public Vector3 spawnPos { get; set; }

    PlayerInfo() { }

    public PlayerInfo(int team, string character, string nickname, int? actorNum)
    {
        this.team = team;
        this.character = character;
        this.nickname = nickname;
        this.actorNum = actorNum;
    }

    public override string ToString()
    {
        return "team :" + team + "character :" + character + "actorNum:" + actorNum + "nickname" + nickname;
    }




    public static object Deserialize(byte[] data)
    {
        var result = new PlayerInfo();
        result.Id = data[0];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        var c = (PlayerInfo)customType;
        return new byte[] { c.Id };
    }



}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }


    /// <summary>
    /// 로컬 플레이어의 캐릭터 오브젝트
    /// </summary>
    public GameObject localPlayer { get; private set; }

    [SerializeField]
    private Text countDownText;

    [Header("mathTablePanel 관련")]
    public GameObject matchTablePanel;

    [SerializeField]
    private Sprite[] portraits;

    [SerializeField]
    private GameObject[] A_TeamProfiles;
    private Text[] A_TeamNames = new Text[3]; 
    private Image[] A_TeamPortraits = new Image[3];

    [SerializeField]
    private GameObject[] B_TeamProfiles;
    private Image[] B_TeamPortraits = new Image[3];
    private Text[] B_TeamNames = new Text[3]; 

    //A팀과 B팀의 스폰 포인트
    [SerializeField]
    private Transform[] A_spawnPoints;
    [SerializeField]
    private Transform[] B_spawnPoints;


    private PhotonView photonView;
    private ScoreManager scoreMgr;

    private Dictionary<string, Sprite> portraitDic = new Dictionary<string, Sprite>();


    [Header("게임 러닝타임(초)")]
    [SerializeField]
    private float GameTimeLimit;
    public float CurrentGameTime { get; private set; }

    //게임진행 관련 필드
    public int winner { get; private set; }
    public bool isGameEnd { get; private set; }
    public System.Action onGameEnd;
    private bool gotTenCoin;

    //한번만 실행하기 위한 단순한 락
    private bool Lock = false;

    [Header("카운트다운")]
    [SerializeField]
    private float countDownMax = 10;
    private float cached_countDownMax;

    //모든 플레이어가 셋팅되고 게임이 시작되었나?
    private bool isGameStart;

    //생성할 캐릭터들에 대한 정보
    private List<PlayerInfo> characterInfo = new List<PlayerInfo>();


    [Header("AI로 스폰할 캐릭터들")]
    public GameObject[] AICharacters;



    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("more than one gameManager");
            return;
        }
        Instance = this;


    }





    void Start()
    {

        //커스텀타입 등록
        PhotonPeer.RegisterType(typeof(PlayerInfo), 1, PlayerInfo.Serialize, PlayerInfo.Deserialize);

        photonView = GetComponent<PhotonView>();
        scoreMgr = ScoreManager.Instance;

        //점수가 바뀔때마다 일어나는 이벤트
        if (scoreMgr != null)
        {
            scoreMgr.onScoreChanged += GotTenCoin;
            scoreMgr.onScoreChanged += CountDown;
        }

        cached_countDownMax = countDownMax;

        //오프라인 테스트용 코드
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            GameObject character = GameObject.FindWithTag("Player");
            localPlayer = character;
        }


        //프로필을 셋팅한다.

        for(int i = 0; i < 3; i++)
        {
            A_TeamPortraits[i] = A_TeamProfiles[i].GetComponentInChildren<Image>();
            A_TeamNames[i] = A_TeamProfiles[i].GetComponentInChildren<Text>();
            B_TeamPortraits[i] = B_TeamProfiles[i].GetComponentInChildren<Image>();
            B_TeamNames[i] = B_TeamProfiles[i].GetComponentInChildren<Text>();
        }




        //마스터 클라이언트에서 캐릭터들에 대한 정보를 생성한다.
        if (PhotonNetwork.IsMasterClient)
        {
            int team = 0;

            //방에 있는 플레이어에들에 대한 정보를 playerInfo에 담는다.
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (team > 1)
                    team = 0;

                Hashtable properties = player.CustomProperties;

                Debug.Log("네트워크 매니저에서 넘어온 플레이어 " + player.NickName + "의 캐릭터:" + (string)properties["character"]);

                PlayerInfo info = new PlayerInfo(team++, (string)properties["character"], player.NickName, player.ActorNumber);
                photonView.RPC("AddcharacterInfoRPC", RpcTarget.AllBuffered, info);

            }

            //만들어낼 AI에 대한 정보를 palyerInfo에 담는다.
            for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers - PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                if (team > 1)
                    team = 0;
                //AI의 캐릭터 정하기
                int random = Random.Range(0, AICharacters.Length);
                PlayerInfo AIInfo = new PlayerInfo(team++, AICharacters[random].name, "AI" + i, null);
                photonView.RPC("AddcharacterInfoRPC", RpcTarget.AllBuffered, AIInfo);
            }
        }


    }


    /// <summary>
    /// 모든 게임 인스턴스에서 characterInfo를 동기화해서 생성한다.
    /// </summary>
    /// <param name="info"></param>
    [PunRPC]
    private void AddcharacterInfoRPC(PlayerInfo info)
    {
        characterInfo.Add(info);
    }





    private void Update()
    {


        if (!Lock && ObjectPooler.instance.IsPoolReady)
        {
            InitGame();
            Lock = true;
        }

        CurrentGameTime += Time.deltaTime;

        if (CurrentGameTime >= 3.0f && isGameStart == false)
        {
            //게임 시작
            isGameStart = true;
        }

        if (CurrentGameTime >= GameTimeLimit)
        {
            isGameEnd = true;
        }

        //테스트용으로 바로 게임클리어하게 하는 장치
        if (Input.GetKeyDown(KeyCode.K))
        {
            isGameEnd = true;
        }


        if (isGameEnd)
        {
            EndGame();
        }

    }

    private void EndGame()
    {
        if (isGameEnd)
        {
            Debug.Log("게임 끝");
            onGameEnd?.Invoke();
            PhotonNetwork.LoadLevel("GameResult");

        }
    }

    private void CountDown()
    {
        //한쪽이라도 10개 이상의 코인을 가지고 있으면 카운트다운.
        if (gotTenCoin)
        {
            StartCoroutine(CountDownCorutine());
            Debug.Log("카운트다운 시작");
        }
    }

    private IEnumerator CountDownCorutine()
    {

        //카운트다운 하는 동안 코인 뺏겨서 10개 이하되면 중단.
        while (countDownMax > 0 && gotTenCoin)
        {
            countDownMax -= Time.deltaTime;
            float minute = Mathf.Round(countDownMax);
            countDownText.text = minute.ToString();

            yield return null;
        }

        //카운트다운이 0보다 작아지면 게임 끝.
        if (countDownMax <= 0)
        {
            if (scoreMgr.ATeamScore > scoreMgr.BTeamScore)
            {
                winner = 0;
            }
            else
                winner = 1;

            Debug.Log("카운트다운 0");
            isGameEnd = true;
        }

        countDownMax = cached_countDownMax;

    }

    private void GotTenCoin()
    {
        if ((scoreMgr.ATeamScore >= 10 || scoreMgr.BTeamScore >= 10) && !(scoreMgr.ATeamScore == scoreMgr.BTeamScore))
        {
            Debug.Log("10개의 이상의 코인 겟");
            gotTenCoin = true;

        }
        else
            gotTenCoin = false;

    }

    private void InitGame()
    {

        Debug.Log("initGame");

        foreach (var portrait in portraits)
        {
            portraitDic.Add(portrait.name, portrait);
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            //마스터 클라이언트에서 모든 플레이어를 스폰하고 제어권을 각각의 플레이어에게 넘겨준다.
            if (PhotonNetwork.IsMasterClient)
            {
                int spawnIndex = 0;

                foreach (PlayerInfo info in characterInfo)
                {
                    Debug.Log(info.ToString());
                    Vector3 spawnPos = info.team == 0 ? A_spawnPoints[spawnIndex].position : B_spawnPoints[spawnIndex].position;

                    GameObject character = PhotonNetwork.Instantiate(info.character, spawnPos, Quaternion.identity);


                    //AI캐릭터에 대한 정보일 경우 넘긴다.
                    if (info.actorNum == null)
                    {
                        continue;
                    }
                    //로컬플레이어에 대한 정보일 경우 로컬플레이어에 전역변수에 할당한다.
                    else if (info.actorNum != null && info.actorNum.Value == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        localPlayer = character;

                    }
                    //로컬이 아닐경우
                    else
                    {
                        //캐릭터의 소유권을 해당 캐릭터 정보에 대한 로컬 플레이어에게 넘겨준다.
                        character.GetComponent<PhotonView>().TransferOwnership(info.actorNum.Value);
                    }
                    //해당 캐릭터의 팀을 정해준다(동기화)
                    character.GetComponent<CharacterSetup>().SetTeamRPC(info.team);
                    //캐릭터를 생성한 위치를 정보에 넘겨준다.
                    info.spawnPos = spawnPos;

                }

            }
            else
            {
                //마스터가 아닌 플레이어의 로컬플레이어 할당법 (마스터 클라이언트가 아직 생성안했는데 여기서 접근하는 문제가 생길수도..)
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    if (player.GetComponent<PhotonView>().IsMine)
                    {
                        localPlayer = player;
                    }
                }

            }

            SetProfile();
        }
        else
        {
            Debug.Log("not ready - offline");
        }

        //3초후 게임시작하는 걸 가정함.
        Destroy(matchTablePanel, 3f);



    }


    //매치테이블을 셋팅
    private void SetProfile()
    {

        int index = 0;

        foreach (PlayerInfo info in characterInfo)
        {
            //프로필 슬롯이 팀당 3개므로 인덱스가 2를 넘어가면 안됨
            if (index > 2)
            {
                index = 0;
            }

            Text[] nameTexts = info.team == 0 ? A_TeamNames : B_TeamNames;
            Image[] portraits = info.team == 0 ? A_TeamPortraits : B_TeamPortraits;


            nameTexts[index].text = info.nickname;
            string portraitName = info.actorNum == null ? info.character.Replace("AI", "") : info.character;
            portraits[index].sprite = portraitDic[portraitName];
            index++;
        }


    }

    public void Respawn(int actorNum)
    {

        foreach (var info in characterInfo)
        {
            if (info.actorNum == actorNum)
            {
                PhotonNetwork.Instantiate(info.character, info.spawnPos, Quaternion.identity);
            }
        }

    }

}



