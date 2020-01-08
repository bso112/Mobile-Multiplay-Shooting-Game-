using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class ParabolaShooter : Shooter
{

    [Header("낙하지점")]
    public Transform target;
    [Header("위로 뜨는 정도")]
    public float upPower;

    protected override IEnumerator ShootCorutine()
    {
        //캐릭터가 총구방향으로 몸을 돌릴때까지 대기
        yield return new WaitForSeconds(0.3f);

        GameObject go;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            go = ObjectPooler.Instance.Instantiate(projectilePrefab.name, shotPos[0].position, Quaternion.identity);
            //발사체에 주인 알려주기
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.owner = transform;

        }
        else
        {
            go = Instantiate(projectilePrefab, shotPos[0].position, Quaternion.identity);

        }

        //타겟은 AIController에서 넘어온다.
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            go.GetComponent<Rigidbody>().AddForce(dir.x * shotPower, upPower, dir.z * shotPower);

        }


        yield return new WaitForSeconds(shootDelay);

    }

    protected override IEnumerator UltimateCorutine()
    {
        StartCoroutine(ShootCorutine());
        characterCon.InitUltiCharge();
        yield return null;
    }

}
