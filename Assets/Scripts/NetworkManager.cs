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

    [Range(0, 1)]
    private int team;
    //게임신에서 스폰할 인덱스
    private int spawnIndex;



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

        //팀은 마스터클라이언트(방장)에서만 관리한다.
        if(PhotonNetwork.IsMasterClient)
        {
            //SetCustomProperties는 동기화가 된다.
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "team", team++ } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "spawnIndex", spawnIndex++ } });
        }

        //플레이어의 캐릭터 선택은 로컬에서 관리한다.
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "character", currentPlayerPrefab } });

        ShowOnlyOnePanel(loadingPanel.name);


    }

    //마스터 클라이언트에서만 실행
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //방장에서만 실행
        //Debug.Log("OnPlayerEnteredRoom");
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        if (team > 1)
        {
            team = 0;
        }

        newPlayer.SetCustomProperties(new Hashtable() { { "team", team++ } });
        newPlayer.SetCustomProperties(new Hashtable() { { "spawnIndex", spawnIndex } });

        // team        0   1   0   1   0   1   0   1
        // spawnIndex  0   0   1   1   2   2   3   3
        if (team == 1)
            spawnIndex++;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
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
        roomOptions.MaxPlayers = 2;

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
