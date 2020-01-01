using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 점수를 UI에 표시함. 모든 게임인스턴스에 하나씩 존재함.
/// </summary>
public class ScoreTextManager : MonoBehaviour
{
    
    #region Singleton
    public static ScoreTextManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("more than one gameManager");
            return;
        }
        Instance = this;

    }
    #endregion



    //점수관련 필드
    [SerializeField]
    private Text ATeamScoreText;
    [SerializeField]
    private Text BTeamScoreText;


    /// <summary>
    /// 모든 인스턴스의 스코어 텍스트를 업데이트함
    /// </summary>
    /// <param name="ATeamScore"></param>
    /// <param name="BTeamScore"></param>
    public void UpdateScore(int ATeamScore, int BTeamScore)
    {
        ATeamScoreText.text = ATeamScore.ToString();
        BTeamScoreText.text = BTeamScore.ToString();
    }




}
