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

    internal void IncreaseScore(Team winningTeam_, out bool isScoreLimit, out Team victoriesTeam)
    {
        Team __TeamReachedScoreLimit__;
        int teamScore;
        IncreaseScoreByType(winningTeam_, out __TeamReachedScoreLimit__, out teamScore);

        var teamHolder = GetWinningTeamByType(winningTeam_);

        teamHolder.IncreaseScore(teamScore);

        isScoreLimit = __TeamReachedScoreLimit__ != Team.None;
        victoriesTeam = __TeamReachedScoreLimit__;
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

    private void IncreaseScoreByType(Team type, out Team teamWon, out int teamScore)
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

        if (m_AlliesScore >= m_ScoreLimit) { teamWon = Team.Allies; }
        else if (m_AxisScore >= m_ScoreLimit) { teamWon = Team.Axis; }
        else teamWon = Team.None;
    }

    public void SetScoreLimit(int scoreLimit) { m_ScoreLimit = scoreLimit; }
}
