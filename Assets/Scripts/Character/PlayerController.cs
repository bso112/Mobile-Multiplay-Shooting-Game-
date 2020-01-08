using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// 상황에 따라 캐릭터의 상태를 변화시키는 스크립트
/// </summary>
public class PlayerController : CharacterController, IPunInstantiateMagicCallback
{

    //풀어 들어갈때만 숨기고 나올때만 원래대로 하려고 쓰는 카운트 변수
    int grasscount;
    private PlayerSetup setup;

    [Header("캐릭터의 렌더러와 상황별 메테리얼")]
    public Renderer playerRenderer;
    public Material playerInvis, playerNormal, playerSemiInvis;
    private float ultiCharage;

    private GameObject respawnTimer;
    private Button ultiBtn;
    private Animator ultiAnim;
    private Image ultiFill;
    private PhotonView view;
    //AI냐?
    protected bool isAI;



    /// <summary>
    /// 궁극기 게이지를 val 만큼 채운다.
    /// </summary>
    /// <param name="val"> max = 1 </param>
    public override void AddUltiCharge(float val)
    {
        val = Mathf.Clamp(val, 0, 1);
        ultiCharage += val;
        if(ultiFill == null)
        {
            Debug.Log("ultiFill이 없어!");
            ultiFill = ultiBtn.transform.Find("ChargeAmount").GetComponent<Image>();
        }
        ultiFill.fillAmount = ultiCharage;

        //궁극기가 준비되면
        if (ultiCharage >= 1)
        {
            //궁극기 버튼 활성화
            ultiBtn.enabled = true;
            ultiAnim.enabled = true;
            //궁극기 활성화
        }
        else
        {
            ultiBtn.enabled = false;
            ultiAnim.enabled = false;
        }
    }

    /// <summary>
    /// 궁극기를 쓴 뒤 궁극기에 대한 설정 초기화
    /// </summary>
    public override void InitUltiCharge()
    {
        ultiCharage = 0;
        ultiFill.fillAmount = 0;
        ultiBtn.enabled = false;
        ultiAnim.enabled = false;
    }


    //keep track of grass collisions and keep player invisible if still colliding with grass
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Grass")
        {
            grasscount++;
            MeshRenderer grassRenderer = col.transform.GetChild(0).GetComponent<MeshRenderer>();


            if (setup != null)
            {
                //아군일 경우
                if (GameManager.Instance.homeTeam == setup.Team)
                {
                    //풀숲을 반투명하게
                    grassRenderer.material.color = ModifyAlpha(grassRenderer, 0.3f);
                    //캐릭터 반투명하게
                    playerRenderer.material = playerSemiInvis;
                }
                else
                {
                    //적군일 경우 안보이게
                    playerRenderer.material = playerInvis;
                }

            }

            //풀숲에 들어갈때 UI 가리기
            if (grasscount == 1)
            {
                transform.Find("PlayerUI").gameObject.SetActive(false);
                isHiding = true;

            }

        }
    }

    //make player invisible when not colliding with grass
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Grass")
        {
            grasscount--;
            MeshRenderer grassRenderer = col.transform.GetChild(0).GetComponent<MeshRenderer>();


            //풀숲을 불투명하게
            grassRenderer.material.color = ModifyAlpha(grassRenderer, 1f);


            //풀숲을 나올때
            if (grasscount == 0)
            {
                //UI표시
                transform.Find("PlayerUI").gameObject.SetActive(true);
                //아군이건 적군이건 정상으로
                playerRenderer.material = playerNormal;
                isHiding = false;
            }

        }
    }

    Color ModifyAlpha(Renderer renderer, float alpha)
    {
        Color newColor = renderer.material.color;
        newColor.a = alpha;
        return newColor;

    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {

        
        Debug.Log("포톤 메시지 : " + info);
        setup = GetComponent<PlayerSetup>();
        view = GetComponent<PhotonView>();

        if (isAI)
            return;

        if (view != null)
        {
            if (view.IsMine)
            {
                //UI셋팅
                Transform canvas = GameObject.Find("Canvas").transform;
                ultiBtn = canvas.Find("UltimateButton").GetComponent<Button>();
                ultiAnim = ultiBtn.transform.Find("Skull").GetComponent<Animator>();
                ultiFill = ultiBtn.transform.Find("ChargeAmount").GetComponent<Image>();
               


                respawnTimer = canvas.Find("RespawnTimerHolder").transform.GetChild(0).gameObject;
                GetComponent<CharacterStats>().onPlayerDie += () => { respawnTimer.SetActive(true); };
            }
        }
        else
            Debug.Log("포톤 뷰가 없습니다!");   

    }
}
