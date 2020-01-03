using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class AISetup : CharacterSetup
{

    private PhotonView photonView;
    public TextMeshProUGUI nameText;

    private void Start()
    {
        transform.parent = null;
        SetPlayerName();
    }

    private void SetPlayerName()
    {
        nameText.text = "AI";
    }

    public override void SetTeamRPC(int _team)
    {
        if (photonView == null)
            Debug.LogError("포톤 뷰가 없습니다");
        photonView.RPC("SetTeam", RpcTarget.AllBuffered, _team);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + "의 팀은" + Team);
    }

    [PunRPC]
    private void SetTeam(int _team)
    {
        Team = _team;
    }

}
