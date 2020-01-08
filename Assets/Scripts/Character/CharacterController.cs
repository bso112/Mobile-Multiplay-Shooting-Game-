using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CharacterController : MonoBehaviour
{
    /// <summary>
    /// 부쉬에 숨어 있는가?
    /// </summary>
    public bool isHiding { get; protected set; }

    protected CharacterStats stats;
    [Header("부쉬에서 초당 몇 체력을 회복하나?")]
    [SerializeField] protected float healAmount = 10f;
    
    //부쉬에 들어간 뒤 초를 세기 위한 변수
    private float currentTime;

    /// <summary>
    /// 궁극기 게이지를 val 만큼 채운다.
    /// </summary>
    /// <param name="val"> max = 1 </param>
    public abstract void AddUltiCharge(float val);

    /// <summary>
    /// 궁극기를 쓴 뒤 궁극기에 대한 설정 초기화
    /// </summary>
    public abstract void InitUltiCharge();




    protected virtual void Awake()
    {
        stats = GetComponent<CharacterStats>();
    }


    protected virtual void Update()
    {
        //부쉬에 숨어있으면 체력 채운다.

        if (isHiding)
        {
            currentTime += Time.deltaTime;

            if (currentTime > 1f)
            {
                stats.Heal(healAmount);
                currentTime = 0;
            }
        }
    }

}
