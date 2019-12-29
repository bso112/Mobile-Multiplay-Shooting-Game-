using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 이펙트 스스로가 자기자신에 대해 행하는 메소드를 모아놓은 클래스
/// </summary>
public class Effect : MonoBehaviour
{
    /// <summary>
    /// 파티클의 지속시간만큼 기다리고 파괴한다.
    /// </summary>
    public void Delayed_Destroy(float delay)
    {
        Destroy(gameObject, delay);
    }
    /// <summary>
    /// 파티클의 지속시간만큼 기다리고 파괴한다.(포톤 디스트로이)
    /// </summary>
    public void Photon_Destroy(float delay)
    {
        //이미 비활성화 상태라면 코드를 실행하지 않는다(오류남)
        if(gameObject.activeSelf)
            StartCoroutine(Photon_Destroy_corutine(delay));
    }
    private IEnumerator Photon_Destroy_corutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(gameObject);
    }
}
