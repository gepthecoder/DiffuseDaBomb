using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairHelper : MonoBehaviour
{
    [SerializeField] private Smoke m_Smoke;
    [SerializeField] private BombCase m_BombCase;

    /// <summary>
    /// NOOOOO ONLY TIME I PROMISE <3
    /// </summary>
    [SerializeField] private BombManager m_BombManager; // ugly.. wrote that blindly af xaxa dont do that kids

    private const float m_RepairPartsTimeLimitShort = .35f;
    private const float m_RepairPartsTimeLimitLong = .6f;

    private VictoryType m_vType;
    private bool m_IsInited = false;

    public void RepairBomb(float repairStatusValue) // 0 - 1 
    {
        if (!m_IsInited)
            return;

        m_BombCase?.RotateTopCase(repairStatusValue, m_vType);

        if (m_vType == VictoryType.BombDefused || m_vType == VictoryType.RoundTimeEnded)
        {
            m_BombCase?.PlayFixBombPartsAnimation(repairStatusValue >= m_RepairPartsTimeLimitLong);
            m_BombManager?.SetSparkSpeed(repairStatusValue);
        }
        else if(m_vType == VictoryType.BombExploded)
        {
            m_BombCase?.PlayFixBombPartsAnimation(repairStatusValue >= m_RepairPartsTimeLimitShort);

            m_BombManager?.TurnOnLightSmooth(repairStatusValue >= .5f);
            m_BombManager?.TurnOnAllCircuitLights(repairStatusValue >= .5f);
        }
    }

    public void SetSmokeAlpha(float val)
    {
        if (m_vType != VictoryType.BombExploded)
            return;

        m_Smoke?.SetSmokeAlpha(val);
    }

    public void Init(VictoryType vType)
    {
        this.m_IsInited = true;
        this.m_vType = vType;
    }
}
