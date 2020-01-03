using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    protected bool isAI;

     void Start()
    {
        if (isAI)
        {
            Debug.Log("AI입니다");
            return;
        }
        Debug.Log("A");
    }
}
