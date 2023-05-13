using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class FinalScore
{
    public TextMeshProUGUI m_TextScoreWinner;
    public TextMeshProUGUI m_TextScoreLoser;

    public void SetScore(int winnerScore, int loserScore)
    {
        m_TextScoreWinner.text = $"{winnerScore}";
        m_TextScoreLoser.text = $"{loserScore}";
    }
}

public class EndMatchManager : MonoBehaviour
{
    [SerializeField] private Animator m_EndMatchAnimeSeq;
    [SerializeField] private Animator m_EndMatchAnimePopUp;
    [Space(5)]
    [SerializeField] private FinalScore m_FinalScore;
    [Space(5)]
    [SerializeField] private DuelObject m_WinningTeamObj;
    [SerializeField] private DuelObject m_LosingTeamObj;
    [Space(5)]
    [SerializeField] private EndMatchObject m_EndMatchObject;

    public void InitEndMatch(EndMatchObjectData DATA)
    {
        // Init Data
        m_EndMatchObject?.SetEndMatchObjectData(DATA);
        m_FinalScore?.SetScore(DATA.m_WinningTeamScore, DATA.m_LosingTeamScore);

        m_WinningTeamObj?.OnSettingsChanged(DATA.m_SettingsItemDataWinner, () => { });
        m_LosingTeamObj?.OnSettingsChanged(DATA.m_SettingsItemDataLoser, () => { });

        // Play Sequence - POP UP
        m_EndMatchAnimePopUp?.Play("POPUP");

        // Play Sequence - END MATCH (ili Someone WON or its a DRAW)
        m_EndMatchAnimeSeq?.Play(!DATA.m_IsDraw ? "endMatchAnime" : "endMatchAnimeDraw");
    }
}
