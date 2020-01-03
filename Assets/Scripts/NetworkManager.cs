using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static NetworkManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("NetworkManager intance is already exist");
            return;
        }
        instance = this;
    }

    #endregion

    public Text connectionStatusText;

    //플레이어가 선택한 캐릭터
    private string currentPlayerPrefab;
    //게임 시작했을때 바로 나오는 캐릭터
    public GameObject defaultCharacterModel;

    [Header("Login Panel")]
    public GameObject loginPanel;
    public InputField nameField;

    [Header("GameOptionPanel")]
    public GameObject gameOptionPanel;
    public Button PlayButton;
    public Button CharacterListButton;
    public CharacterPreview characterPreview;

    [Header("CharacterListPanel")]
    public GameObject characterListPanel;
    public Button characterSelectButton;

    [Header("LoadingPanel")]
    public GameObject loadingPanel;
    public Text playerCount;

    [Header("CharacterStatsPanel")]
    public GameObject characterStatsPanel;

    [Header("Room capacity of player")]
    [SerializeField] private byte maxPlayerCount;
    [Range(0, 1)]
    private int team;
    //게임신에서 스폰할 인덱스
    private int spawnIndex;

    /// <summary>
    /// 매치메이킹 대기시간
    /// </summary>
    private float waitTime;
    [Header("매치메이킹 최대 대기시간")]
    public float maxWait;


    //룸에 모든 인원이 모였나?
    private bool isRoomFull;
    //로컬 플레이어가 방에 들어왔나?
    private bool isLocalEnteredRoom;
    //최대 대기시간이 초과되었나?
    private bool isWaitMax;

    // Start is called before the first frame update
    void Start()
    {
        ShowOnlyOnePanel(loginPanel.name);
        currentPlayerPrefab = "Soldier";
        characterPreview.currentModel = defaultCharacterModel;

    }

    // Update is called once per frame
    void Update()
    {
        connectionStatusText.text = PhotonNetwork.NetworkClientState.ToString();

        if (isLocalEnteredRoom)
        {

            waitTime += Time.deltaTime;

            if (waitTime > maxWait && !isWaitMax)
            {
                //최대 대기시간이 지나면 그냥 게임실행
                PhotonNetwork.LoadLevel("GameScene");
                isWaitMax = true;
            }

            //테스트용 코드
            if (Input.GetKeyDown(KeyCode.K))
            {
                waitTime = maxWait;
            }
        }


        

    }

    public void SetCharacter(CharacterInfo _characterInfo)
    {
        //플레이어의 캐릭터 선택을 저장한다.
        currentPlayerPrefab = _characterInfo.name;
    }




    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        ShowOnlyOnePanel(gameOptionPanel.name);
    }

    //로컬플레이어만 실행
    public override void OnJoinedRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //로컬에서만 실행
        Debug.Log("OnJoinedRoom");
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "character", currentPlayerPrefab } });

        isLocalEnteredRoom = true;

        ShowOnlyOnePanel(loadingPanel.name);


    }

    //마스터 클라이언트에서만 실행
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        isRoomFull = PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;

    }



    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
    {
        Debug.Log(target.NickName + "의 커스텀 프로퍼티가 " + changedProps + "으로 업데이트 되었습니다.");


        //모든 플레이어가 방에 들어왔고, 커스텀프로퍼티가 모두 셋팅되었다면 게임실행
        if (isRoomFull && changedProps["character"] != null)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }


    }



    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        //룸이 없으면 만든다.
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayerCount;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

   

    #endregion

    #region UI Methods


    public void OnLoginBtnClicked()
    {
        PhotonNetwork.NickName = nameField.text;
        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.Disconnect();
        }
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.ConnectUsingSettings();


    }

    public void OnPlayBtnClicked()
    {
        PhotonNetwork.JoinRandomRoom();

    }

    public void OnCharacterListBtnClicked()
    {
        ShowOnlyOnePanel(characterListPanel.name);

    }

    public void OnMapSelectBtnClicked()
    {

    }

    public void ShowOnlyOnePanel(string _panelName)
    {

        loginPanel.SetActive(_panelName.Equals(loginPanel.name));
        gameOptionPanel.SetActive(_panelName.Equals(gameOptionPanel.name));
        characterListPanel.SetActive(_panelName.Equals(characterListPanel.name));
        loadingPanel.SetActive(_panelName.Equals(loadingPanel.name));
        characterStatsPanel.SetActive(_panelName.Equals(characterStatsPanel.name));

        
    }

    #endregion

}
