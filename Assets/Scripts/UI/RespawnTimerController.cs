using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnTimerController : MonoBehaviour
{   
    [SerializeField] Image Fill;
    [Header("리스폰 대기시간(초)")]
    [SerializeField] float maxTime;
    private float currentTime = 0f;

    private void OnEnable()
    {
        Fill.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        Fill.fillAmount = currentTime / maxTime;
        if(Fill.fillAmount >= 1)
        {
            GameManager.Instance.Respawn();
            currentTime = 0f;
            Fill.fillAmount = 0;
            gameObject.SetActive(false);
        }


    }
}
