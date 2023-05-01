using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single Source Of Truth For Rounds
/// </summary>
public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;

    private int m_CurrentRound = 1;
    private int m_MaxRounds;

    private void Awake() => instance = this;

    public void NewRound() { m_CurrentRound++; }
    public int GetCurrentRound() { return m_CurrentRound; }

    public void SetMaxRounds(int maxRounds) { m_MaxRounds = maxRounds; }

    public bool IsHalfTime()
    {
        return m_CurrentRound == m_MaxRounds / 2;
    }

}
