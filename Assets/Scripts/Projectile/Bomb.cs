using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Bomb : Projectile
{
    [Header("폭탄이 밀어내는 힘")]
    public float power;
    [Header("피해반경")]
    public float radius;
    [Header("대상이 위로 뜨는 정도")]
    public float upForce;





    private ParabolaController controller;
    private Vector3 startPos;
    public GameObject effectPrefab;
    private Renderer[] renderers;

    private void OnEnable()
    {
        if (renderers != null)
        {
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
            }
        }

        if (controller != null)
            controller.FollowParabola();


    }

    private new void Start()
    {
        base.Start();
        startPos = transform.position;
        controller = GetComponent<ParabolaController>();
        renderers = GetComponentsInChildren<Renderer>();
    }


    private void OnDisable()
    {
        transform.position = startPos;
        //멈춰줘야됨..
        if (controller != null)
        {
            controller.StopFollow();
        }

    }


    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("bomb Collided with" + collision.transform.name);

        Explosion();

    }

    private void Explosion()
    {



        //부딪힌 곳을 시작으로 radius만큼 주변에 있는 콜라이더들을 가져움
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var col in colliders)
        {
            PhotonView view = col.transform.GetComponent<PhotonView>();
            //로컬플레이어면 패스
            if (view != null)
            {
                if (view.Owner == PhotonNetwork.LocalPlayer)
                    continue;
            }

            //밀어내고 데미지 주기
            Rigidbody rib = col.GetComponent<Rigidbody>();
            if (rib != null)
            {
                rib.AddExplosionForce(power, transform.position, radius, upForce, ForceMode.Force);
                CharacterStats targetStats = col.GetComponent<CharacterStats>();

                if (targetStats != null)
                {
                    Debug.Log(targetStats.gameObject.name);
                    targetStats.TakeDamageRPC(ownerStats.attack.GetValue() + damage);

                }

            }
        }

        //이펙트 생성, 파괴, 오브젝트 파괴
        GameObject effectObj;

        if (PhotonNetwork.IsConnectedAndReady)
        { effectObj = PhotonNetwork.Instantiate(effectPrefab.name, transform.position, Quaternion.identity); }
        else //오프라인 테스트용 코드
        { effectObj = Instantiate(effectPrefab, transform.position, Quaternion.identity); }


        float delay = effectObj.GetComponent<ParticleSystem>().main.duration;
        if (view.IsMine)
        {
            effectObj.GetComponent<Effect>().Photon_Destroy(delay);
            //오브젝트 파괴
            PhotonNetwork.Destroy(this.gameObject);
            
        }

        //오프라인 테스트용 코드
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            effectObj.GetComponent<Effect>().Delayed_Destroy(delay);
            Destroy(gameObject);

        }




    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
