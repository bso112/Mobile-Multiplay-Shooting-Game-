using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CoinGenerator : MonoBehaviour
{
    //코인이 생성되는 시간 간격
    public float interval = 1f;
    public float popPower = 1f;
    public Transform popPos;
    private float timeStamp;

    private bool Lock = false;
    private ObjectPooler pool;

    private void Start()
    {
        pool = ObjectPooler.Instance;

        if(!PhotonNetwork.IsMasterClient)
        {
            this.enabled = false;
        }
        else if(!PhotonNetwork.IsConnectedAndReady)
        {
            this.enabled = true;
        }

        pool.onObjectPoolReady += ()=> { if (PhotonNetwork.IsMasterClient) SpawnCoin(); };

    }


    void SpawnCoin()
    {
        StartCoroutine(SpawnCoinCorutine());
    }

    IEnumerator SpawnCoinCorutine()
    {

        while (true)
        {
            if (timeStamp <= Time.time)
            {
                float x = Random.Range(-1f, 1f);
                float z = Random.Range(-1f, 1f);
                Vector3 randomDir = new Vector3(x, 1, z);

                GameObject coin = PhotonNetwork.Instantiate("Coin", popPos.position, Quaternion.identity);   

                coin.GetComponent<Rigidbody>().AddForce(randomDir * popPower, ForceMode.Impulse);

                timeStamp = Time.time + interval;

            }
            yield return null;
        }
    }


}
