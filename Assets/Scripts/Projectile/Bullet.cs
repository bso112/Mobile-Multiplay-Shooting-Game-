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


    private new void Start()
    {
        base.Start();
        // instantiate bullet and muzzel flash
        projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
        projectileParticle.transform.parent = transform;
        if (muzzleParticle)
        {
            muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
            Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
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
            if(view.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        CharacterStats target = other.GetComponent<CharacterStats>();
        if (target != null && ownerStats != null && target != ownerStats)
        {
            target.TakeDamageRPC(ownerStats.attack.GetValue());
        }

        impactParticle = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)) as GameObject;



        ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();
        for (int i = 1; i < trails.Length; i++)
        {
            ParticleSystem trail = trails[i];
            if (!trail.gameObject.name.Contains("Trail"))
                continue;
            trail.transform.SetParent(null);
            Effect e_trail = trail.GetComponent<Effect>();
            e_trail.Photon_Destroy(2f);
        }


        if (view.IsMine)
        {
            Effect e_projectileParticle = projectileParticle.GetComponent<Effect>();
            Effect e_impactParticle = impactParticle.GetComponent<Effect>();
            e_projectileParticle.Photon_Destroy(3f);
            e_impactParticle.Photon_Destroy(5f);
            PhotonNetwork.Destroy(gameObject);
        }

    }
}
