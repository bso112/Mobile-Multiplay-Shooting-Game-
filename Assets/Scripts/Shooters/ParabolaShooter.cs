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

    protected override IEnumerator Shoot(GameObject projectilePrefab)
    {
        //캐릭터가 총구방향으로 몸을 돌릴때까지 대기
        yield return new WaitForSeconds(0.3f);

        GameObject projectile;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            projectile = PhotonNetwork.Instantiate(projectilePrefab.name, shotPos[0].position, Quaternion.identity);
        }
        else
        {
            projectile = Instantiate(projectilePrefab, shotPos[0].position, Quaternion.identity);

        }

        Vector3 dir = target.position - transform.position;
        projectile.GetComponent<Rigidbody>().AddForce(dir.x * shotPower, upPower, dir.z * shotPower);


        yield return new WaitForSeconds(shootDelay);

    }

    protected override IEnumerator Ultimate(GameObject _projectilePrefab)
    {
        StartCoroutine(Shoot(_projectilePrefab));
        GetComponent<PlayerController>().InitUltiCharge();
        yield return null;
    }

}
