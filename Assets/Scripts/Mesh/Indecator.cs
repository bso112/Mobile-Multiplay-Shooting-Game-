using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Indecator : MonoBehaviour
{
    
    GraphicRaycaster gr;
    PointerEventData ped;
    // 공격 범위를 알려주는 이미지
    public GameObject indecator;

    

    // Start is called before the first frame update
    void Start()
    {
        gr = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        ped = new PointerEventData(null);
    }

    // Update is called once per frame
    void Update()
    {
 
        if (Input.GetMouseButtonDown(0))
        {
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            gr.Raycast(ped, results);
            //레이캐스트를 맞은 객체가 있으면
            if (results.Count > 0)
            {
                //객체를 순회하며 체크
                foreach (var result in results)
                {
                    if (result.gameObject.name.Equals("Movement Joystick"))
                    {
                        if(indecator != null)
                        indecator.SetActive(true);
                    }
                    
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(indecator != null)
                indecator.SetActive(false);
        }
    }

}

