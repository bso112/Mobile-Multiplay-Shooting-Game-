using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 인공지능 컨트롤러
/// </summary>
public class AIController : CharacterController
{

    /*
     * 1. 인공지능과 각 플레이어의 거리를 모두 구해 최단거리에 있는 플레이어를 알아낸다. (풀숲에 있는 플레이어는 생략한다)
     * 2. 그 플레이어에게 다가간다. 사정거리 안까지 다가가면 공격한다.(공격은 0.2초마다 시도한다)
     * 3. 궁이 차있다면 궁을 쓴다 
     * 4. 만약 자신의 체력이 일정체력 이하면 세이프포인트로 도망간다.
     * 5. 체력이 일정이상 채워지면 1로 돌아간다.
     */



    //풀어 들어갈때만 숨기고 나올때만 원래대로 하려고 쓰는 카운트 변수
    private int grasscount;
    private AISetup setup;
    private Shooter shooter;

    [Header("캐릭터의 렌더러와 상황별 메테리얼")]
    public Renderer playerRenderer;
    public Material playerInvis, playerNormal, playerSemiInvis;

    [Header("체력이 이 수치 이하면 도망감")]
    public float runThreshHold;


    private NavMeshAgent agent;
    private bool isAllplayerSetted;
    private List<GameObject> players = new List<GameObject>();
    private float ultiCharage;
    private bool AIProcessExed;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        setup = GetComponent<AISetup>();
        shooter = GetComponent<Shooter>();
    }



    protected override void Update()
    {
        base.Update();

        if (GameManager.Instance.isGameStart && !isAllplayerSetted)
        {
            players = GameManager.Instance.playerCharacters;
            isAllplayerSetted = true;
        }

        //모든 플레이어에 대한 레퍼런스가 준비되면
        if (isAllplayerSetted)
        {
            //AI프로세스 코루틴을 한번가동한다.
            if (!AIProcessExed)
            {
                StartCoroutine(AIProcess());
                AIProcessExed = true;
            }
        }

    }

    private IEnumerator AIProcess()
    {

        //세이프 포인트 가져오기
        Transform safePoints = GameObject.Find("SafePoints").transform;
        Transform red_parent = safePoints.GetChild(0);
        List<Transform> red_safePoints = new List<Transform>();
        List<Transform> blue_safePoints = new List<Transform>();

        for (int i = 0; i < red_parent.childCount; i++)
        {
            red_safePoints.Add(red_parent.GetChild(i));
        }
        Transform blue_parent = safePoints.GetChild(1);

        for (int i = 0; i < blue_parent.childCount; i++)
        {
            blue_safePoints.Add(blue_parent.GetChild(i));
        }

        Vector3 safePoint = Vector3.zero;
        //세이프포인트 선택에 사용할 랜덤인덱스
        int random = Random.Range(0, red_safePoints.Count);

        while (true)
        {

            //체력이 낮아지면 도망간다.
            if (stats.currentHP < runThreshHold)
            {
                
                //세이프포인트가 아직 셋팅안되었으면 셋팅
                if (safePoint == Vector3.zero)
                {
                    //자신의 팀에 따라 맞는 세이프포인트로 가기
                    safePoint = setup.Team == 0 ? red_safePoints[random].position : blue_safePoints[random].position;
                    agent.SetDestination(safePoint);
                    yield return new WaitForSeconds(1);
                    //체력이 찰때까지 가만히
                    continue;
                }
            }



            //플레이어 갱신
            players = GameManager.Instance.playerCharacters;
            //최단거리에 있는 캐릭터 구하기

            //타겟 캐릭터
            GameObject target = null;

            float minDistance = float.MaxValue;

            foreach (var player in players)
            {
                //플레이어가 파괴된 상태면 스킵
                if (player == null)
                    continue;
                //자기자신 제외, 같은 팀 제외
                if (player != gameObject && player.GetComponent<CharacterSetup>().Team != setup.Team)
                {
                    //상대 캐릭터와 자기자신간의 거리
                    float currDistance = Vector3.Distance(player.transform.position, transform.position);

                    //상대 캐릭터가 숨어있나?
                    bool isHiding = player.GetComponent<CharacterController>().isHiding;

                    //거리가 최소인 상대 캐릭터 구해서 target에 넣기
                    if (minDistance > currDistance && isHiding == false)
                    {
                        minDistance = currDistance;
                        target = player;

                    }
                }
            }

            if (agent != null && target != null)
            {
                //타겟으로 가기
                agent.SetDestination(target.transform.position);
                transform.LookAt(target.transform);


            }

            else if (agent == null)
            {
                Debug.Log("agent is null");
            }
            else
            {
                Debug.Log("target is null");
                yield return new WaitForSeconds(1);
                continue;
            }

            //만약 사정거리만큼 거리가 될만큼 가까워지면
            if (Vector3.Distance(target.transform.position, transform.position) < stats.range.GetValue())
            {

                //파라볼라슈터는 타겟을 정해줘야한다.
                ParabolaShooter parabolaShooter = GetComponent<ParabolaShooter>();
                if (parabolaShooter != null)
                {
                    parabolaShooter.target = target.transform;
                }


                //궁극기 게이치가 차면 궁극기 사용
                if (ultiCharage >= 1)
                {
                    shooter.OnUltiButtonClicked();
                }
                else
                {
                    //보통공격
                    shooter.OnShotButtonClicked();
                }

            }



            //이 모든 것을 1초에 한번만 체크한다.
            yield return new WaitForSeconds(1);
        }

    }

    public override void AddUltiCharge(float val)
    {
        val = Mathf.Clamp(val, 0, 1);
        ultiCharage += val;

    }

    public override void InitUltiCharge()
    {
        ultiCharage = 0;

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

}
