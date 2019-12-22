﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 이펙트 스스로가 자기자신에 대해 행하는 메소드를 모아놓은 클래스
/// </summary>
public class Effect : MonoBehaviour
{
    private ParticleSystem ps;
    //파티클의 지속시간
    private float duration;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        duration = ps.main.duration;
    }
    /// <summary>
    /// 파티클의 지속시간만큼 기다리고 파괴한다.
    /// </summary>
    public void Delayed_Destroy()
    {
        Destroy(gameObject, duration);
    }
    /// <summary>
    /// 파티클의 지속시간만큼 기다리고 파괴한다.(포톤 디스트로이)
    /// </summary>
    public void Photon_Destroy()
    {
        StartCoroutine(Photon_Destroy_corutine());
    }
    private IEnumerator Photon_Destroy_corutine()
    {
        yield return new WaitForSeconds(duration);
        PhotonNetwork.Destroy(gameObject);
    }
}
