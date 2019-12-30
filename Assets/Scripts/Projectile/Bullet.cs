using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 총알을 구성하는 이펙트의 제어, 닿은 대상에 대해 데미지 부여하는 스크립트
/// </summary>
public class Bullet : Projectile
{

    public GameObject impactParticle;//played on collision
    public GameObject projectileParticle;//bullet particle
    public GameObject muzzleParticle;
    public Vector3 impactNormal;
    private bool isConnectedAndReady;
    private bool isInitialized;


    //처음 한번
    private new void Start()
    {
        isConnectedAndReady = PhotonNetwork.IsConnectedAndReady;
        base.Start();

        if (muzzleParticle)
        {
            muzzleParticle = PhotonNetwork.Instantiate(muzzleParticle.name.Replace("(Clone)", ""), transform.position, transform.rotation) as GameObject;
            Effect effect = muzzleParticle.GetComponent<Effect>();
            if (view.IsMine || !isConnectedAndReady)
                effect.Photon_Destroy(1.5f);
        }

        isInitialized = true;


    }

    private void OnEnable()
    {
        Collider[] cols = GetComponents<Collider>();
        foreach (var col in cols)
        {
            col.enabled = true;
        }

        //오브젝트 풀에 있는 오브젝트가 모두 start를 거친 뒤, 껐다가 킬때마다 실행
        if (isInitialized)
        {

            if (muzzleParticle)
            {
                muzzleParticle = PhotonNetwork.Instantiate(muzzleParticle.name.Replace("(Clone)", ""), transform.position, transform.rotation) as GameObject;
                Effect effect = muzzleParticle.GetComponent<Effect>();
                if (view.IsMine || !isConnectedAndReady)
                    effect.Photon_Destroy(1.5f);
            }
        }
    }

    private void OnDisable()
    {
        Collider[] cols = GetComponents<Collider>();
        foreach (var col in cols)
        {
            col.enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (ownerStats == null)
            return;
        //사거리 벗어나면 사라짐 (y축 방향 제외).. 발사체의 크기를 고려해 공격범위로부터 +2 만큼을 최대로 둠.
        float range = ownerStats.range.GetValue();
        if (Mathf.Abs(owner.position.x - transform.position.x) > range + 2 || Mathf.Abs(owner.position.z - transform.position.z) > range + 2)
        {
            if (view.IsMine || !isConnectedAndReady)
                PhotonNetwork.Destroy(gameObject);
        }


    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("발사체가 트리거 된 것: " + other.name);
        //같은 발사체끼리 부딪치면 무시
        if (other.gameObject.name == gameObject.name || other.tag == "Grass" || other.tag == "Coin")
        {
            return;
        }

        //부딪힌게 캐릭터면 데미지를 준다.
        CharacterStats target = other.gameObject.GetComponent<CharacterStats>();
        if (target != null && ownerStats != null && target != ownerStats)
        {
            target.TakeDamageRPC(ownerStats.attack.GetValue());
        }

        //터질때 나타나는 이펙트
        impactParticle = PhotonNetwork.Instantiate(impactParticle.name.Replace("(Clone)", ""), transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));


        //이펙트를 하나하나 끈다.
        if (view.IsMine || !isConnectedAndReady)
        {
            Effect e_impactParticle = impactParticle.GetComponent<Effect>();
            e_impactParticle.Photon_Destroy(5f);
            PhotonNetwork.Destroy(gameObject);


        }
    }

}
