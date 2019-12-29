using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Indecator : MonoBehaviour
{

    private PlayerMotor motor;
    private Joystick attackJoystick;
    private CharacterStats stats;
    public float speed;
    [Header("던지는 궤적을 나타내는가?")]
    [SerializeField] private bool isThrowable;

    private void Start()
    {
        motor = transform.root.GetComponent<PlayerMotor>();
        stats = transform.root.GetComponent<CharacterStats>();
        attackJoystick = motor.attackJoystick;
        attackJoystick.onPointerDown += () => gameObject.SetActive(true); 
        attackJoystick.onPointerUp += () => gameObject.SetActive(false);
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
            transform.root.LookAt(dir * 100);
        }

    }

}

