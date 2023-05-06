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

    private int m_CurrentRound = 1;
    private int m_MaxRounds;

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
            // TODO: Init Duel Objects
            StartCoroutine(TrySwitchSides(roundTimeStartAction)); 
        }
        else { roundTimeStartAction?.Invoke(); }
    }
    public int GetCurrentRound() { return m_CurrentRound; }

    public void SetMaxRounds(int maxRounds) { m_MaxRounds = maxRounds; }

    private bool IsHalfTime()
    {
        return m_CurrentRound == (m_MaxRounds / 2) + 1;
    }

    // S W I T C I N G  S I D E S

    public IEnumerator TrySwitchSides(Action action) // TODO: scene -> add indicator for ATTACKER / DEFENDER
    {
        yield return new WaitForSeconds(2f);

        // Hide Teams - Defender / Attacker

        m_SwitchingSidesAnime.Play("POPUP");

        m_SwitchSidesAnime.Play("SWITCH");

        yield return new WaitForSeconds(4.5f); // lenght of m_SwitchSidesAnime => 5s

        // Show Teams - Attacker / Defender


        action?.Invoke();
    }


}
