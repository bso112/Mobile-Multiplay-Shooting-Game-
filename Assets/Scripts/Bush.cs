//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Bush : MonoBehaviour
//{

//    MeshRenderer meshRenderer;
//    private ScoreManager scoreMgr;

//    private void Start()
//    {
//        meshRenderer = GetComponent<MeshRenderer>();
//        scoreMgr = ScoreManager.Instance;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        Debug.Log("trigger");
//        //닿은게 플레이어면 부쉬는 투명하게 변하고, 플레이어 체력바를 숨긴다.
//        if (other.transform.CompareTag("Player"))
//        {
//            other.transform.Find("PlayerUI").gameObject.SetActive(false);
//            PlayerSetup setup = other.GetComponent<PlayerSetup>();
//            //아군이면 부쉬 속에서 투명하게 보인다.              
//            if (scoreMgr != null && scoreMgr.HomeTeam == setup.Team)
//            {
//                meshRenderer.material.color = ModifyAlpha(meshRenderer, 0.3f);
//            }

//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        //플레이어가 부쉬를 벗어날 때는 원상태로 되돌린다.
//        if (other.transform.CompareTag("Player"))
//        {
//            other.transform.Find("PlayerUI").gameObject.SetActive(true);
//            PlayerSetup setup = other.GetComponent<PlayerSetup>();
//            //다시 원래 투명도로 바꿔줌
//            if (scoreMgr.HomeTeam == setup.Team)
//            {
//                meshRenderer.material.color = ModifyAlpha(meshRenderer, 1f);
//            }
//        }
//    }


//    Color ModifyAlpha(Renderer renderer, float alpha)
//    {
//        Color newColor = renderer.material.color;
//        newColor.a = alpha;
//        return newColor;

//    }

//}
