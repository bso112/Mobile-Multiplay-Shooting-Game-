using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : Projectile
{

    public GameObject impactParticle;//played on collision
    public GameObject projectileParticle;//bullet particle
    public GameObject muzzleParticle;
    public Vector3 impactNormal;
    private bool isConnectedAndReady;
    private bool isInitialized;



    private new void Start()
    {
        isConnectedAndReady = PhotonNetwork.IsConnectedAndReady;
        base.Start();

        //bullet이 활성화 되면 얘도 동시에 활성화 되기 때문에 OnEnable에서 안해도 됨.
        projectileParticle = PhotonNetwork.Instantiate(projectileParticle.name.Replace("(Clone)", ""), transform.position, transform.rotation) as GameObject;
        projectileParticle.transform.parent = transform;


    }

    private void OnEnable()
    {
        Collider[] cols = GetComponents<Collider>();
        foreach (var col in cols)
        {
            col.enabled = true;
        }

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
        //사거리 벗어나면 사라짐
        if (Vector3.Distance(owner.position, transform.position) > ownerStats.range.GetValue())
        {
            if (view.IsMine || !isConnectedAndReady)
                PhotonNetwork.Destroy(gameObject);

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //같은 발사체끼리 부딪치면 무시
        if (other.gameObject.name == gameObject.name)
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
            Effect e_projectileParticle = projectileParticle.GetComponent<Effect>();
            Effect e_impactParticle = impactParticle.GetComponent<Effect>();
            e_projectileParticle.Photon_Destroy(0f);
            e_impactParticle.Photon_Destroy(5f);


        }
    }

}
