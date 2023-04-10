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

    private const float m_RepairPartsTimeLimit = .35f;

    public void RepairBomb(float repairStatusValue)
    {
        m_BombCase?.PlayFixBombPartsAnimation(repairStatusValue >= m_RepairPartsTimeLimit);
        m_BombCase?.RotateTopCase(repairStatusValue);

        m_BombManager?.TurnOnLightSmooth(repairStatusValue >= .5f);
        m_BombManager?.TurnOnAllCircuitLights(repairStatusValue >= .5f);
    }

    public void SetSmokeAlpha(float val)
    {
        m_Smoke.SetSmokeAlpha(val);
    }
}
