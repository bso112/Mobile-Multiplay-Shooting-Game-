using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    private void Start()
    {
        this.enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("부딪힘");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거!!");
    }

}
