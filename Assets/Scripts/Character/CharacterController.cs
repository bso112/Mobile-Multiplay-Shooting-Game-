using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    public bool isHiding {get; protected set;}

    /// <summary>
    /// 궁극기 게이지를 val 만큼 채운다.
    /// </summary>
    /// <param name="val"> max = 1 </param>
    public abstract void AddUltiCharge(float val);

    /// <summary>
    /// 궁극기를 쓴 뒤 궁극기에 대한 설정 초기화
    /// </summary>
    public abstract void InitUltiCharge();
}
