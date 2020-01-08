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
    [Header("인디케이터가 움직이는 속도")]
    public float speed;
    [Header("인디케이터에 따라 회전할 트랜스폼")]
    public Transform shotPos;
    [Header("던지는 궤적을 나타내는가?")]
    [SerializeField] private bool isThrowable;


    public void Start()
    {
        if(transform.root.GetComponent<PhotonView>().IsMine)
        {
            stats = transform.GetComponentInParent<CharacterStats>();
            attackJoystick = GameObject.Find("Canvas").transform.Find("Attack Joystick").GetComponent<Joystick>();
            attackJoystick.onPointerDown += OnPointerDown;
            attackJoystick.onPointerUp += OnPointerUp;
        }

        //처음엔 비활성화 상태여야함.
        gameObject.SetActive(false);


    }


    private void OnPointerDown()
    {
        //이벤트가 실행됬는데 인디케이터는 파괴되어있을 수 있다.
        if(gameObject != null)
            gameObject.SetActive(true);
    }

    private void OnPointerUp()
    {
        if (!isThrowable) { transform.root.rotation = transform.rotation; }
        if(gameObject != null)
            gameObject.SetActive(false);
    }



    private void FixedUpdate()
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

