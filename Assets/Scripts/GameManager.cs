using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.IO;




public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }


    /// <summary>
    /// 로컬 플레이어의 캐릭터 오브젝트
    /// </summary>
    public GameObject localPlayer { get; private set; }
    /// <summary>
    /// 로컬플레이어의 팀을 반환한다. 실패하면 -1을 반환한다.
    /// </summary>
    public int homeTeam = -1;


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
    public bool isGameStart;



    [Header("AI로 스폰할 캐릭터들")]
    public GameObject[] AICharacters;


    public List<ExitGames.Client.Photon.Hashtable> playerInfos { get; private set; }
    //playerinfo의 기본키
    private int playerID = 0;



    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("more than one gameManager");
            return;
        }
        Instance = this;

        playerInfos = new List<ExitGames.Client.Photon.Hashtable>(); 
    }





    void Start()
    {

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
        for (int i = 0; i < 3; i++)
        {
            A_TeamPortraits[i] = A_TeamProfiles[i].GetComponentInChildren<Image>();
            A_TeamNames[i] = A_TeamProfiles[i].GetComponentInChildren<Text>();
            B_TeamPortraits[i] = B_TeamProfiles[i].GetComponentInChildren<Image>();
            B_TeamNames[i] = B_TeamProfiles[i].GetComponentInChildren<Text>();
        }





    }

    [PunRPC]
    private void AddPlayerInfoRPC(ExitGames.Client.Photon.Hashtable info)
    {
        playerInfos.Add(info);
    }



    private void Update()
    {


        if (!Lock && ObjectPooler.instance.IsPoolReady)
        {
            InitGame();
            Lock = true;
        }

        CurrentGameTime += Time.deltaTime;

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
            //플레이어 정보를 셋팅
            foreach (var player in PhotonNetwork.PlayerList)
            {
                ExitGames.Client.Photon.Hashtable props = player.CustomProperties;
                int team = (int)props["team"];
                string character = (string)props["character"];
                int spawnIndex = (int)props["spawnIndex"];

                Vector3 spawnPos = team == 0 ? A_spawnPoints[spawnIndex].position : B_spawnPoints[spawnIndex].position;

                ExitGames.Client.Photon.Hashtable info = new ExitGames.Client.Photon.Hashtable() { { "playerID", playerID++ }, { "team", team }, { "character", character },
                                                                                                   { "spawnPos", spawnPos }, { "nickname", player.NickName } };

                photonView.RPC("AddPlayerInfoRPC", RpcTarget.AllBuffered, info);

                //로컬 캐릭터 인스턴스화
                if (player.IsLocal)
                {
                    GameObject go = PhotonNetwork.Instantiate(character, spawnPos, Quaternion.identity);
                    go.GetComponent<CharacterSetup>().playerID = playerID;
                    go.GetComponent<CharacterSetup>().SetTeamRPC(team);
                    localPlayer = go;
                    homeTeam = team;
                }
            }

            //마스터클라이언트에서 AI를 인스턴스화한다.
            if (PhotonNetwork.IsMasterClient)
            {
                //AI를 인스턴스화한다.

                //AI가 스폰할 트랜스폼을 정할 인덱스
                int AI_A_spawnIndex = 0;
                int AI_B_spawnIndex = 0;

                //AI의 스폰포인트를 잡기 위해 현재 사용된 스폰포인트를 뛰어넘는다.
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    ExitGames.Client.Photon.Hashtable prop = player.CustomProperties;
                    if ((int)prop["team"] == 0)
                        AI_A_spawnIndex++;
                    else
                        AI_B_spawnIndex++;
                }


                //인스턴스화할 AI 개수
                int AICount = PhotonNetwork.CurrentRoom.MaxPlayers - PhotonNetwork.CurrentRoom.PlayerCount;
                //처음으로 인스턴스할 AI의 팀
                int AITeam = (PhotonNetwork.CurrentRoom.PlayerCount % 2) == 0 ? 0 : 1;


                for (int i = 0; i < AICount; i++)
                {
                    if (AITeam > 1)
                        AITeam = 0;

                    Vector3 AISpawnPos = AITeam == 0 ? A_spawnPoints[AI_A_spawnIndex++].position : B_spawnPoints[AI_B_spawnIndex++].position;

                    int random = Random.Range(0, AICharacters.Length);
                    GameObject AIgo = PhotonNetwork.Instantiate(AICharacters[random].name, AISpawnPos, Quaternion.identity);
                    AIgo.GetComponent<CharacterSetup>().playerID = playerID;
                    AIgo.GetComponent<CharacterSetup>().SetTeamRPC(AITeam);
                    ExitGames.Client.Photon.Hashtable info = new ExitGames.Client.Photon.Hashtable() { { "playerID", playerID++ }, { "team", AITeam }, { "character", AICharacters[random].name },
                                                                                                   { "spawnPos", AISpawnPos }, { "nickname", "AI" + i } };

                    photonView.RPC("AddPlayerInfoRPC", RpcTarget.AllBuffered, info);


                    AITeam++;
                }
            }


            SetProfile();
        }
        else
        {
            Debug.Log("not ready - offline");
        }

        isGameStart = true;
        //3초후 게임시작하는 걸 가정함.
        Destroy(matchTablePanel, 3f);



    }


    //매치테이블을 셋팅
    private void SetProfile()
    {

        //프로필의 텍스트와 이미지를 지정할 인덱스
        int index = 0;

        foreach (var info in playerInfos)
        {

            string character = (string)info["character"];
            int team = (int)info["team"];

            //프로필 슬롯이 팀당 3개므로 인덱스가 2를 넘어가면 안됨
            if (index > 2)
            {
                index = 0;
            }

            Text[] nameTexts = team == 0 ? A_TeamNames : B_TeamNames;
            Image[] portraits = team == 0 ? A_TeamPortraits : B_TeamPortraits;

            nameTexts[index].text = (string)info["nicknam"];
            portraits[index].sprite = portraitDic[character.Replace("AI", "")];
            index++;
        }






    }

    public void Respawn(int playerID)
    {
        foreach (var info in playerInfos)
        {
            int ID = (int)info["playerID"];
            string character = (string)info["character"];
            Vector3 spawnPos = (Vector3)info["spawnPos"];

            if (ID == playerID)
                PhotonNetwork.Instantiate(character, spawnPos, Quaternion.identity);
        }



    }

}



