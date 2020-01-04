using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;

public class Indecator : MonoBehaviour
{

    private PlayerMotor motor;
    private Joystick attackJoystick;
    private CharacterStats stats;
    public float speed;
    [Header("인디케이터에 따라 회전할 트랜스폼")]
    public Transform shotPos;
    [Header("던지는 궤적을 나타내는가?")]
    [SerializeField] private bool isThrowable;
    private PhotonView view;


    public void Start()
    {
        view = transform.GetComponentInParent<PhotonView>();
        
        if(view !=null)
        {
            if (view.IsMine)
            {
                stats = transform.GetComponentInParent<CharacterStats>();
                attackJoystick = GameObject.Find("Canvas").transform.Find("Attack Joystick").GetComponent<Joystick>();
                attackJoystick.onPointerDown += () => { view.RPC("SetActive", RpcTarget.All, true);  };
                attackJoystick.onPointerUp += () => { if (!isThrowable) { transform.root.rotation = transform.rotation; } view.RPC("SetActive", RpcTarget.All, false); };
                

            }else if(!PhotonNetwork.IsConnectedAndReady)
            {
                attackJoystick = GameObject.Find("Canvas").transform.Find("Attack Joystick").GetComponent<Joystick>();
                attackJoystick.onPointerDown += () => gameObject.SetActive(true);
                attackJoystick.onPointerUp += () => { if (!isThrowable) { transform.root.rotation = transform.rotation; } gameObject.SetActive(false); };

            }
        }

        //처음엔 비활성화 상태여야함.
        gameObject.SetActive(false);


    }


    //리모트 인스턴스에서도 인디케이터는 보여야하기 때문에.
    [PunRPC]
    private void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void FixedUpdate()
    {
        if(view.IsMine || !PhotonNetwork.IsConnectedAndReady)
        {
            float h = attackJoystick.Horizontal;
            float v = attackJoystick.Vertical;

            Vector3 dir = new Vector3(h, 0, v).normalized;

            if (isThrowable)
            {
                transform.position += dir * speed;
                transform.localPosition = Vector3.ClampMagnitude(transform.localPosition, stats.range.GetValue());

            }
            else
            {
                transform.LookAt(dir * 100);
                shotPos.rotation = transform.rotation;
            }
        }



    }



}

