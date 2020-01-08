using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterStats))]
public abstract class Shooter : MonoBehaviour, IPunInstantiateMagicCallback
{
    public GameObject projectilePrefab;
    public GameObject specialProjectilePrefab;
    [Header("")]
    public Transform[] shotPos;
    [Header("한번 공격에 몇번 발사하는가")]
    public float firePerClick = 1f;
    [Header("공격간의 딜레이")]
    public float fireCoolDown;
    [Header("특수공격간의 딜레이")]
    public float UltiCoolDown;
    [Header("발사간의 딜레이")]
    public float shootDelay = 0.2f;
    [Header("발사체를 쏘는 힘")]
    public float shotPower;

    //발사 효과음을 트는 오디오소스
    private AudioSource fx;
    
    protected GameObject currentProjectile;
    protected CharacterStats ownerStats;

    private float timeStampForAttack;
    private float timeStampForUlti;

    /// <summary>
    /// 궁극기 사용후 게이지 초기화를 위해 사용
    /// </summary>
    protected CharacterController characterCon;



    public void OnShotButtonClicked()
    {

        if(projectilePrefab == null)
        {
            Debug.Log("발사체를 할당하세요!");
            return;
        }

        if(timeStampForAttack <= Time.time)
        {
            if(fx!=null)
            {
                fx.Play();
                Debug.Log("효과음 실행");
            }

            PhotonView view = GetComponent<PhotonView>();
            if (view != null)
                view.RPC("Shoot", RpcTarget.AllBuffered);
            else
                Debug.Log("view is null");

            timeStampForAttack = Time.time + fireCoolDown;
        }
        
    }

    public void OnUltiButtonClicked()
    {

        if (specialProjectilePrefab == null)
        {
            Debug.Log("궁극기 발사체를 할당하세요!");
            return;
        }


        if (timeStampForUlti <= Time.time)
        {
            Debug.Log("궁극기!");
            PhotonView view = GetComponent<PhotonView>();
            if (view != null)
                view.RPC("Ultimate", RpcTarget.All);
            else
                Debug.Log("view is null");
            timeStampForUlti = Time.time + UltiCoolDown;
        }
    }

    [PunRPC]
    protected void Shoot()
    {
        StartCoroutine(ShootCorutine());
    }

    [PunRPC]
    protected void Ultimate()
    {
        StartCoroutine(UltimateCorutine());
    }

    protected abstract IEnumerator ShootCorutine();

    protected abstract IEnumerator UltimateCorutine();

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        fx = GetComponent<AudioSource>();
        ownerStats = GetComponent<CharacterStats>();
        characterCon = GetComponent<CharacterController>();
    }
}
