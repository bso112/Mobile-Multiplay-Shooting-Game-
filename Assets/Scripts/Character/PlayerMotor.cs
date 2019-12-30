using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    public Joystick moveJoystick;
    public Joystick attackJoystick;
    public Animator animator;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveJoystick != null)
        {
            Vector3 direction = (Vector3.forward * moveJoystick.Vertical + Vector3.right * moveJoystick.Horizontal).normalized;
            rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
            if(Mathf.Abs(moveJoystick.Vertical) > 0.1f || Mathf.Abs(moveJoystick.Horizontal) > 0.1f)
            {
                if(animator != null)
                {
                    animator.SetFloat("Speed", (Mathf.Abs(moveJoystick.Vertical) + Mathf.Abs(moveJoystick.Horizontal))/2 );
                }
            }
            else
                animator.SetFloat("Speed", 0);

            transform.LookAt(transform.position + direction);

        }


    }


}
