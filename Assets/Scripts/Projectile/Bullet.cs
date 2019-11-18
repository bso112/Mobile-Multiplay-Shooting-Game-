﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : Projectile
{
    

    // Update is called once per frame
    void Update()
    {
        if (ownerStats == null)
            return;
        //사거리 벗어나면 사라짐
        if(Vector3.Distance(owner.position, transform.position) > ownerStats.range.GetValue())
        {
            PhotonNetwork.Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        CharacterStats target = other.GetComponent<CharacterStats>();
        if(target!=null && ownerStats != null && target != ownerStats )
        {   
            target.TakeDamageRPC(ownerStats.attack.GetValue());
            PhotonNetwork.Destroy(gameObject);
        }
        
    }
}
