using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //풀어 들어갈때만 숨기고 나올때만 원래대로 하려고 쓰는 카운트 변수
    int grasscount;		
    public PlayerSetup setup;
    MeshRenderer meshRenderer;
    private ScoreManager scoreMgr;

    private void Start()
    {
        setup = GetComponent<PlayerSetup>();
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
