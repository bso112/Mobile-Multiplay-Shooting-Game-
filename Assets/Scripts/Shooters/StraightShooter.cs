using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class StraightShooter : Shooter
{

    protected override IEnumerator ShootCorutine()
    {
        //캐릭터가 총구방향으로 몸을 돌릴때까지 대기
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < firePerClick; i++)
        {
            //발사체 스폰
            GameObject go = ObjectPooler.Instance.Instantiate(projectilePrefab.name, shotPos[0].position, Quaternion.Euler(transform.forward));
            //발사
            Rigidbody projectileRB = go.GetComponent<Rigidbody>();
            projectileRB.AddForce(transform.forward * shotPower, ForceMode.Force);
            //발사체에 주인 알려주기
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.owner = transform;
            //연속 발사 사이에 텀을 둔다. 
            yield return new WaitForSeconds(shootDelay);
        }

    }

    protected override IEnumerator UltimateCorutine()
    {
        //캐릭터가 총구방향으로 몸을 돌릴때까지 대기
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < firePerClick + 5; i++)
        {
            //발사체 스폰
            GameObject go = ObjectPooler.Instance.Instantiate(projectilePrefab.name, shotPos[0].position, Quaternion.Euler(transform.forward));
            //발사
            Rigidbody projectileRB = go.GetComponent<Rigidbody>();
            projectileRB.AddForce(transform.forward * shotPower, ForceMode.Force);
            //발사체에 주인 알려주기
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.owner = transform;
            //연속 발사 사이에 텀을 둔다. 
            yield return new WaitForSeconds(shootDelay);
        }

        //궁극기 게이지 초기화
        characterCon.InitUltiCharge();
    }
}
