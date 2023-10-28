using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Single Source Of Truth For Rounds
/// </summary>
public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;
    [Header("POP UPS")]
    [SerializeField] private Animator m_SwitchingSidesAnime;
    [SerializeField] private Animator m_NewRoundAnime;
    private TextMeshProUGUI m_RoundText;

    [Header("SWITCHING SIDES")]
    [SerializeField] private Animator m_SwitchSidesAnime;
    [SerializeField] private MainCanvas m_MainCanvas;

    private int m_CurrentRound = 1;
    private int m_MaxRounds;

    private DuelObjectType m_AxisSideType = DuelObjectType.Defender;
    private DuelObjectType m_AlliesSideType = DuelObjectType.Attacker;

    private void Awake() => instance = this;

    private void Start()
    {
        m_RoundText = m_NewRoundAnime.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void NewRound(Action roundTimeStartAction) { 
        m_CurrentRound++;

        m_RoundText.text = string.Format("ROUND {0}", m_CurrentRound);
        m_NewRoundAnime.Play("POPUP");
        
        if(IsHalfTime()) {
            StartCoroutine(TrySwitchSides(roundTimeStartAction)); 
        }
        else {
            AudioManager.INSTANCE.PlayGameIntroAudio();
            roundTimeStartAction?.Invoke(); 
        }
    }
    public int GetCurrentRound() { return m_CurrentRound; }

    public void SetMaxRounds(int maxRounds) { m_MaxRounds = maxRounds; }

    private bool IsHalfTime()
    {
        return m_CurrentRound == (m_MaxRounds / 2) + 1;
    }

    // S W I T C I N G  S I D E S

    public IEnumerator TrySwitchSides(Action action)
    {
        m_AlliesSideType = DuelObjectType.Defender;
        m_AxisSideType = DuelObjectType.Attacker;

        AudioManager.INSTANCE.SwitchingSides();

        yield return new WaitForSeconds(2f);

        // Hide Teams - Defender / Attacker
        m_MainCanvas?.ShowTeamPlaceholder(false);

        m_SwitchingSidesAnime.Play("POPUP");

        // SWITCH
        m_SwitchSidesAnime.Play("SWITCH");

        m_MainCanvas?.SwapAttackerDefender();

        yield return new WaitForSeconds(4.5f); // lenght of m_SwitchSidesAnime => 5s

        // Show Teams - Attacker / Defender
        m_MainCanvas?.ShowTeamPlaceholder(true);

        AudioManager.INSTANCE.PlayGameIntroAudio(false);

        action?.Invoke();
    }

    public Team GetWinningTeamByVictoryType(VictoryType type)
    {
        Team team = Team.None;

        switch (type)
        {
            case VictoryType.BombExploded:
                {
                    return GetAttackerTeam();
                }
            case VictoryType.BombDefused:
                {
                    return GetDefenderTeam();
                }
            case VictoryType.RoundTimeEnded:
                {
                    return GetDefenderTeam();
                }
            default:
                break;
        }

        return team;
    }

    private Team GetDefenderTeam()
    {
        if(m_AxisSideType == DuelObjectType.Defender)
        {
            return Team.Axis;
        }

        if (m_AlliesSideType == DuelObjectType.Defender)
        {
            return Team.Allies;
        }

        else return Team.None;
    }

    private Team GetAttackerTeam()
    {
        if (m_AxisSideType == DuelObjectType.Attacker)
        {
            return Team.Axis;
        }

        if (m_AlliesSideType == DuelObjectType.Attacker)
        {
            return Team.Allies;
        }

        else return Team.None;
    }

}
