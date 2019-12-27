using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 오프라인 테스트를 위해 셋업해주는 스크립트
/// </summary>
public class Offline_setup : MonoBehaviour
{
    public GameObject characterPrefab;
    private GameObject character;
    public Transform spawnPos;
    public Button attackBtn, ultiBtn;
    public GameObject[] toDisable;
    public Joystick joystick;
    public FollowCam followCam;


    // Start is called before the first frame update
    void Awake()
    {
        character = Instantiate(characterPrefab, spawnPos.position, characterPrefab.transform.rotation);
        followCam.target = character;
        character.GetComponent<PlayerSetup>().enabled = false;
        attackBtn.onClick.AddListener(character.GetComponent<Shooter>().OnShotButtonClicked);
        ultiBtn.onClick.AddListener(character.GetComponent<Shooter>().OnUltiButtonClicked);
        character.GetComponent<PlayerMotor>().joyStick = joystick;
        foreach(var obj in toDisable)
        {
            obj.SetActive(false);
        }
        
        
    }


}
