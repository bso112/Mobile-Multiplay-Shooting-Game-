using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AISetup : PlayerSetup
{

    private void Awake()
    {   
        //부모의 start메소드 실행금지. AI입니다.
        isAI = true;
    }



}
