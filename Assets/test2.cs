using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    private void Start()
    {
        test t = FindObjectOfType<test>();
        t.print();
    }
}
