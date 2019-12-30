using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviour
{
    //public float flySpeed;
    //public float destroyDistance;
    //public GameObject destroyEffect;
    private PhotonView view;


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag.Equals("Player"))
        {

            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Destroy(gameObject);
            }
            else
            {
                if (view.Owner != PhotonNetwork.MasterClient)
                    view.TransferOwnership(PhotonNetwork.LocalPlayer);
                if (view.IsMine)
                    PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void Awake()
    {
        view = GetComponent<PhotonView>();


    }






}
