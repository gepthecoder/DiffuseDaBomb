using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private MainCanvas m_MainCanvas;

    private int m_ScoreLimit;

    private int m_AxisScore = 0;
    private int m_AlliesScore = 0;

    internal void IncreaseScore(Team winningTeam_, out bool isScoreLimit, out Team victoriesTeam, out bool isDraw)
    {
        Team __TeamReachedScoreLimit__;
        int teamScore;
        bool isTeamDraw;
        IncreaseScoreByType(winningTeam_, out __TeamReachedScoreLimit__, out teamScore, out isTeamDraw);

        var teamHolder = GetWinningTeamByType(winningTeam_);

        teamHolder.IncreaseScore(teamScore);

        isScoreLimit = __TeamReachedScoreLimit__ != Team.None;
        isDraw = isScoreLimit && isTeamDraw;
        victoriesTeam = __TeamReachedScoreLimit__;
    }

    public void GetFinalScoreByTeamType(Team winTeam, out int winScore, out int loseScore)
    {
        switch (winTeam)
        {
            case Team.Axis:
                {
                    winScore = m_AxisScore;
                    loseScore = m_AlliesScore;
                } break;
            case Team.Allies:
                {
                    winScore = m_AlliesScore;
                    loseScore = m_AxisScore;
                }
                break;
            case Team.None:
            default:
                {
                    winScore = -1;
                    loseScore = -1;
                }
                break;
        }
    }

    private MainTeamHolder GetWinningTeamByType(Team type)
    {
        switch (type)
        {
            case Team.Axis:
                return m_MainCanvas.GetAxisTeamHolder();
            case Team.Allies:
                return m_MainCanvas.GetAlliesTeamHolder();
            case Team.None:
            default:
                return null;
        }
    }

    private void IncreaseScoreByType(Team type, out Team teamWon, out int teamScore, out bool isDraw)
    {
        switch (type)
        {
            case Team.Axis:
                m_AxisScore += 1;
                teamScore = m_AxisScore;
                break;
            case Team.Allies:
                m_AlliesScore += 1;
                teamScore = m_AlliesScore;
                break;
            case Team.None:
            default:
                teamScore = -1;
                break;
        }

        if((m_AxisScore + m_AlliesScore) >= m_ScoreLimit)
        {
            teamWon = m_AxisScore > m_AlliesScore ? Team.Axis : Team.Allies;
        } else { teamWon = Team.None; }

        isDraw = m_AlliesScore == m_AxisScore;
    }

    public void SetScoreLimit(int scoreLimit) { m_ScoreLimit = scoreLimit; }
}
