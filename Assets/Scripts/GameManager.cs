using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{

    #region Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("more than one gameManager");
            return;
        }
        Instance = this;

    }
    #endregion

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
    private Text[] A_TeamNames;
    private Image[] A_TeamPortraits;

    [SerializeField]
    private GameObject[] B_TeamProfiles;
    private Image[] B_TeamPortraits;
    private Text[] B_TeamNames;

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
    private bool Lock;

    [Header("카운트다운")]
    [SerializeField]
    private float countDownMax = 10;
    private float cached_countDownMax;

    //모든 플레이어가 셋팅되고 게임이 시작되었나?
    private bool isGameStart;


    


    private void Start()
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
        if(!PhotonNetwork.IsConnectedAndReady)
        {
            GameObject character = GameObject.FindWithTag("Player");
            localPlayer = character;
        }

      

    }

    private void Update()
    {


        if (!Lock && ObjectPooler.instance.IsPoolReady)
        {
            InitGame();
            Lock = true;
        }

        CurrentGameTime += Time.deltaTime;

        if(CurrentGameTime >= 3.0f && isGameStart == false)
        {
            //게임 시작
            isGameStart = true;
        }

        if (CurrentGameTime >= GameTimeLimit)
        {
            isGameEnd = true;
        }

        //테스트용으로 바로 게임클리어하게 하는 장치
        if(Input.GetKeyDown(KeyCode.K))
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
        //프로필을 셋팅한다. (순서대로 들어가지 않음)
        foreach (GameObject A_Profile in A_TeamProfiles)
        {
            A_TeamPortraits = A_Profile.GetComponentsInChildren<Image>();
            A_TeamNames = A_Profile.GetComponentsInChildren<Text>();
        }

        foreach (GameObject B_Profile in B_TeamProfiles)
        {
            B_TeamPortraits = B_Profile.GetComponentsInChildren<Image>();
            B_TeamNames = B_Profile.GetComponentsInChildren<Text>();
        }

        foreach (var portrait in portraits)
        {
            portraitDic.Add(portrait.name, portrait);
        }

        //네트워크가 준비되면
        if (PhotonNetwork.IsConnectedAndReady)
        {

            photonView = GetComponent<PhotonView>();

            //캐릭터 스폰
            int A_index = 0;
            int B_index = 0;


            ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.LocalPlayer.CustomProperties;


            //A팀은 A팀 포지션에서 스폰, B팀은 B팀 포지션에서 스폰.
            if ((int)properties["team"] == 0)
            {
                A_index = Mathf.Clamp(A_index, 0, A_spawnPoints.Length);
                localPlayer = PhotonNetwork.Instantiate((string)properties["character"], A_spawnPoints[A_index++].position, Quaternion.identity);
                //각 플레이어마다 팀을 정해준다.
                localPlayer.GetComponent<PlayerSetup>().SetTeamRPC(0);

            }
            else
            {
                B_index = Mathf.Clamp(B_index, 0, B_spawnPoints.Length);
                localPlayer = PhotonNetwork.Instantiate((string)properties["character"], B_spawnPoints[B_index++].position, Quaternion.identity);
                localPlayer.GetComponent<PlayerSetup>().SetTeamRPC(1);
            }




            //매치테이블 셋팅
            StartCoroutine(SetProfileCorutine());


        }
        else
        {
            Debug.Log("not ready - offline");
        }

        //3초후 게임시작하는 걸 가정함.
        Destroy(matchTablePanel, 3f);



    }





    //매치테이블을 셋팅
    private IEnumerator SetProfileCorutine()
    {
        //커스텀 프로퍼티를 가져오는데 값이 null이나 ""이면 충분히 기다리지 않았다는 뜻.
        yield return new WaitForSeconds(0.1f);

        Debug.Log("setProfile");
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("in room");
        }
        else
            Debug.Log("out room");

        int A_TeamProfileIndex = 0;
        int B_TeamProfileIndex = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log("참가한 플레이어 이름" + player.NickName);
            ExitGames.Client.Photon.Hashtable properties = player.CustomProperties;

            if ((int)properties["team"] == 0)
            {
                A_TeamProfileIndex = Mathf.Clamp(A_TeamProfileIndex, 0, A_TeamProfiles.Length);
                A_TeamNames[A_TeamProfileIndex].text = player.NickName;
                A_TeamPortraits[A_TeamProfileIndex++].sprite = portraitDic[(string)properties["character"]];


            }
            else
            {
                B_TeamProfileIndex = Mathf.Clamp(B_TeamProfileIndex, 0, B_TeamProfiles.Length);
                B_TeamNames[B_TeamProfileIndex].text = player.NickName;
                Debug.Log(player.NickName + "의 캐릭터: " + (string)properties["character"]);
                B_TeamPortraits[B_TeamProfileIndex++].sprite = portraitDic[(string)properties["character"]];
            }
        }

    }


}



