using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 총알을 구성하는 이펙트의 제어, 닿은 대상에 대해 데미지 부여하는 스크립트
/// </summary>
public class Bullet : Projectile
{

    //부딪쳤을때 나타나는 파티클
    public GameObject impactParticle;//played on collision
    //발사시 나타나는 파티클
    public GameObject muzzleParticle;
    public Vector3 impactNormal;
    [Header("한 발 피격당 궁극기가 차는 정도(max = 1)")]
    public float ultiCharge;
    private bool isConnectedAndReady;
    private bool isInitialized;
    private ObjectPooler pool;

    //오너의 팀
    private int team;

    //처음 한번
    private new void Start()
    {
        pool = ObjectPooler.Instance;

        if (muzzleParticle)
        {
            GameObject particle = pool.Instantiate(muzzleParticle.name, transform.position, transform.rotation) as GameObject;
            pool.Destroy(particle, 1.5f);
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
                GameObject particle = pool.Instantiate(muzzleParticle.name, transform.position, transform.rotation) as GameObject;
                pool.Destroy(particle, 1.5f);
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

        //오너가 할당이 안됬거나, 파괴되었으면 아무것도 안함
        if (owner == null)
            return;

        //오너가 할당되면 오너스탯을 가져옴. 덤으로 팀도 가져옴
        if(owner != null && ownerStats == null)
        {
            ownerStats = owner.GetComponent<CharacterStats>();
            team = owner.GetComponent<CharacterSetup>().Team;
        }

        //사거리 벗어나면 사라짐 (y축 방향 제외).. 발사체의 크기를 고려해 공격범위로부터 +2 만큼을 최대로 둠.
        float range = ownerStats.range.GetValue();
        if (Mathf.Abs(owner.position.x - transform.position.x) > range + 2 || Mathf.Abs(owner.position.z - transform.position.z) > range + 2)
        {
            pool.Destroy(gameObject);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        //오너가 할당이 안됬거나, 파괴되었으면 아무것도 안함
        if (owner == null)
            return;

        //부딪힌게 상대 캐릭터면 데미지를 준다.
        CharacterStats target = other.gameObject.GetComponent<CharacterStats>();
        //자기자신이 아니고, 적군이면
        if (target != null && ownerStats != null && target != ownerStats && other.GetComponent<CharacterSetup>().Team != team)
        {
            //상대 캐릭터의 공격력만큼 데미지 준다.
            target.TakeDamageRPC(ownerStats.attack.GetValue());
            //캐릭터의 궁극기 게이지를 채운다.
            CharacterController characterCon = owner.GetComponent<CharacterController>();
            if (characterCon != null)
                characterCon.AddUltiCharge(ultiCharge);
            else
                Debug.Log("궁극기를 채우려는데 컨트롤러가 없음!");

            //터질때 나타나는 이펙트
            GameObject particle = pool.Instantiate(impactParticle.name, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));
            pool.Destroy(particle, 5f);
            pool.Destroy(gameObject);
        }


    }

}
