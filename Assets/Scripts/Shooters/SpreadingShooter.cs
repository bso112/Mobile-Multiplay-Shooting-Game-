using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SpreadingShooter : Shooter
{
    [Header("한번 발사에 얼마나 많은 투사체가 날아가는가")]
    public float shotPerFire;
    [Header("탄막 각도")]
    public float spreadAngle;
    [Header("궁극기 범위")]
    public float radius;
    [Header("궁극기 지속시간")]
    public float UltiDuration;

    public Animator UltimateAni;

    private int shotPosCount;
    private float timeCounter;
    private float timeStamp;


    protected override IEnumerator Shoot(GameObject projectilePrefab)
    {
        //캐릭터가 총구방향으로 몸을 돌릴때까지 대기
        yield return new WaitForSeconds(0.1f);

        float angle = -spreadAngle / 2;

        for (int i = 0; i < shotPerFire; i++)
        {
            //발사
            Vector3 bulletAngle = new Vector3(0, transform.rotation.eulerAngles.y + angle, 0);
            angle += spreadAngle / shotPerFire;
            //발사체 스폰
            GameObject go = ObjectPooler.Instance.Instantiate(projectilePrefab.name, shotPos[shotPosCount].position, Quaternion.Euler(bulletAngle));
            Rigidbody projectileRB = go.GetComponent<Rigidbody>();
            //발사체에 주인 알려주기
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.owner = transform;
            projectileRB.AddForce(go.transform.forward * shotPower);
        }
        shotPosCount++;
        shotPosCount %= shotPos.Length;


        yield return null;
    }

    protected override IEnumerator Ultimate(GameObject projectilePrefab)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        //UltiDuration 동안 범위 내의 적들을 0.2초마다 타격한다.
        while (UltiDuration >= timeCounter)
        {
            timeCounter += Time.deltaTime;

            if (timeStamp <= Time.time)
            {
                foreach (var collider in colliders)
                {
                    CharacterStats targetStats = collider.GetComponent<CharacterStats>();
                    if (targetStats != null && targetStats != ownerStats)
                    {
                        targetStats.TakeDamage(ownerStats.attack.GetValue());
                        timeStamp = Time.time + 0.2f;
                    }
                }
            }
            yield return null;
        }
        timeCounter = 0;

        characterCon.InitUltiCharge();
    }





    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
