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

    [SerializeField] private Animator m_SwitchingSidesAnime;
    [SerializeField] private Animator m_NewRoundAnime;
    private TextMeshProUGUI m_RoundText;

    private int m_CurrentRound = 1;
    private int m_MaxRounds;

    private void Awake() => instance = this;

    private void Start()
    {
        m_RoundText = m_NewRoundAnime.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void NewRound() { 
        m_CurrentRound++;

        if (IsHalfTime()) // TODO
        {
            m_SwitchingSidesAnime.Play("POPUP");
        }

        m_RoundText.text = string.Format("ROUND {0}", m_CurrentRound);
        m_NewRoundAnime.Play("POPUP");
    }
    public int GetCurrentRound() { return m_CurrentRound; }

    public void SetMaxRounds(int maxRounds) { m_MaxRounds = maxRounds; }

    private bool IsHalfTime()
    {
        return m_CurrentRound == m_MaxRounds / 2;
    }


}
