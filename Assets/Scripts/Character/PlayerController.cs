using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// 상황에 따라 캐릭터의 상태를 변화시키는 스크립트
/// </summary>
public class PlayerController : MonoBehaviour
{

    //풀어 들어갈때만 숨기고 나올때만 원래대로 하려고 쓰는 카운트 변수
    int grasscount;		
    public PlayerSetup setup;
    MeshRenderer meshRenderer;
    private ScoreManager scoreMgr;
    private float ultiCharage;

    private GameObject respawnTimer;
    private Button ultiBtn;
    private Animator ultiAnim;
    private Image ultiFill;
    private PhotonView view;



    private void Start()
    {   
       
        setup = GetComponent<PlayerSetup>();
        scoreMgr = ScoreManager.Instance;
        view = GetComponent<PhotonView>();
        if (view != null)
        {
            if (view.IsMine)
            {
                //UI셋팅
                Transform canvas = GameObject.Find("Canvas").transform;
                ultiBtn = canvas.Find("UltimateButton").GetComponent<Button>();
                ultiBtn.enabled = false;
                ultiAnim = ultiBtn.transform.Find("Skull").GetComponent<Animator>();
                ultiFill = ultiBtn.transform.Find("ChargeAmount").GetComponent<Image>();

                respawnTimer = canvas.Find("RespawnTimerHolder").transform.GetChild(0).gameObject;
                GetComponent<CharacterStats>().onPlayerDie += () => { respawnTimer.SetActive(true); };
            }
        }
        else
            Debug.Log("포톤 뷰가 없습니다!");

    }


    /// <summary>
    /// 궁극기 게이지를 val 만큼 채운다.
    /// </summary>
    /// <param name="val"> max = 1 </param>
    public void AddUltiCharge(float val)
    {
        val = Mathf.Clamp(val, 0, 1);
        ultiCharage += val;
        ultiFill.fillAmount = ultiCharage;

        //궁극기가 준비되면
        if(ultiCharage >= 1)
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
    public void InitUltiCharge()
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
            meshRenderer = col.transform.GetChild(0).GetComponent<MeshRenderer>();

            if (setup != null)
            {
                if (scoreMgr != null && scoreMgr.HomeTeam == setup.Team)
                {
                    meshRenderer.material.color = ModifyAlpha(meshRenderer, 0.3f);
                }
            }

            //오프라인 테스트용 코드
            if (!Photon.Pun.PhotonNetwork.IsConnectedAndReady)
            {
                meshRenderer.material.color = ModifyAlpha(meshRenderer, 0.3f);
            }

            //풀숲에 들어갈때 UI 가리기
            if (grasscount == 1)
            {
                transform.Find("PlayerUI").gameObject.SetActive(false);
            }

        }
    }

    //make player invisible when not colliding with grass
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Grass")
        {
            grasscount--;
            meshRenderer = col.transform.GetChild(0).GetComponent<MeshRenderer>();

            if (setup != null)
            {
                if (scoreMgr != null && scoreMgr.HomeTeam == setup.Team)
                {
                    meshRenderer.material.color = ModifyAlpha(meshRenderer, 1f);
                }
            }

            //오프라인 테스트용 코드
            if (!Photon.Pun.PhotonNetwork.IsConnectedAndReady)
            {
                meshRenderer.material.color = ModifyAlpha(meshRenderer, 1f);
            }

            //풀숲을 나올때 UI 표시하기
            if (grasscount == 0)
            {
                transform.Find("PlayerUI").gameObject.SetActive(true);
            }

        }
    }

    Color ModifyAlpha(Renderer renderer, float alpha)
    {
        Color newColor = renderer.material.color;
        newColor.a = alpha;
        return newColor;

    }



}
