using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RespawnTimerController : MonoBehaviour
{   
    [SerializeField] Image Fill;
    [Header("리스폰 대기시간(초)")]
    private float maxTime;
    private float currentTime = 0f;

    private void OnEnable()
    {
        Fill.fillAmount = 0;
    }

    private void Start()
    {
        maxTime = GameManager.Instance.respawnWait;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        Fill.fillAmount = currentTime / maxTime;
        if(Fill.fillAmount >= 1)
        {
            currentTime = 0f;
            Fill.fillAmount = 0;
            gameObject.SetActive(false);
        }


    }
}
