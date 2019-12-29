﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(AudioSource))]
public abstract class Shooter : MonoBehaviour
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
    protected ObjectPooler pooler;
    protected CharacterStats ownerStats;

    private float timeStampForAttack;
    private float timeStampForUlti;


    private void Start()
    {
        fx = GetComponent<AudioSource>();
        pooler = ObjectPooler.instance;
        ownerStats = GetComponent<CharacterStats>();
    }

    //자식에서 Start를 쓰면 부모의 Start가 실행이 안됨..
    protected void BaseStart()
    {
        fx = GetComponent<AudioSource>();
        pooler = ObjectPooler.instance;
        ownerStats = GetComponent<CharacterStats>();
    }

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
            StartCoroutine(Shoot(projectilePrefab));
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
            StartCoroutine(Ultimate(specialProjectilePrefab));
            timeStampForUlti = Time.time + UltiCoolDown;
        }
    }

    protected abstract IEnumerator Shoot(GameObject projectilePrefab);

    protected abstract IEnumerator Ultimate(GameObject projectilePrefab);

   






}
