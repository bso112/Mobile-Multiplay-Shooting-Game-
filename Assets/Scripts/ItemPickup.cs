using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterStats))]
public class ItemPickup : MonoBehaviour
{
    //동전을 줍고, 게임매니저(마스터클라이언트)에게 스코어를 획득했음을 알린다.

    /// <summary>
    /// 플레이어가 먹은 동전 수(팀이 아님)
    /// </summary>
    public int score { get; private set; }
    private PlayerSetup setup;
    private PhotonView view;
    private CharacterStats stats;
    private ScoreManager scoreMgr;

    //죽을 때 드랍되는 코인에 가해지는 힘
    public float popPower { get; private set; }


    private void Start()
    {
        setup = GetComponent<PlayerSetup>();
        view = GetComponent<PhotonView>();
        stats = GetComponent<CharacterStats>();
        stats.onPlayerDie += DropCoins;
        scoreMgr = ScoreManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (view.IsMine)
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin != null)
            {
                score++;
                OnPickCoinRPC();
            }
        }

    }

    //플레이어가 죽을때 코인을 드랍한다.
    private void DropCoins()
    {
        for (int i = 0; i < score; i++)
        {
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            Vector3 randomDir = new Vector3(x, 1, z);
            GameObject coin = PhotonNetwork.Instantiate("Coin", transform.position, Quaternion.identity);
            coin.GetComponent<Rigidbody>().AddForce(randomDir * popPower, ForceMode.Impulse);
        }

        score = 0;
    }

    /// <summary>
    /// 마스터클라이언트의 스코어매니저에게 자기 팀 점수를 올려달라고 요청함.
    /// </summary>
    private void OnPickCoinRPC()
    {   
        if(view.IsMine)
            scoreMgr.AddScore(setup.Team);
        



    }

}
