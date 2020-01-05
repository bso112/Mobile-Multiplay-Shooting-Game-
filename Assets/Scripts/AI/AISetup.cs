using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class AISetup : CharacterSetup
{

    private PhotonView view;
    public TextMeshProUGUI nameText;

    private void Start()
    {
        transform.parent = null;
        view = GetComponent<PhotonView>();
        SetPlayerName();
    }

    private void SetPlayerName()
    {
        nameText.text = "AI";
    }

    public override void SetTeamRPC(int _team)
    {
        if (view == null)
        {
            view = GetComponent<PhotonView>();
        }
        view.RPC("SetTeam", RpcTarget.AllBuffered, _team);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "의 팀은" + Team);
    }

    [PunRPC]
    private void SetTeam(int _team)
    {
        Team = _team;
    }

}
