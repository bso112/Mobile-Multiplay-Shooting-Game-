using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{

    protected CharacterStats ownerStats;
    protected Transform owner;
    private GameManager gm;
    protected PhotonView view;


    [Header("기본공격에 붙는 추가데미지")]
    [SerializeField]
    protected float damage;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    protected void Start()
    {

        gm = GameManager.Instance;
        if(gm.localPlayer == null)
        {
            Debug.Log("로컬플레이어 널!");
        }
        owner = gm.localPlayer.transform;
        ownerStats = gm.localPlayer.GetComponent<CharacterStats>();


    }






}
