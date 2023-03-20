using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairHelper : MonoBehaviour
{
    [SerializeField] private Smoke m_Smoke;
    [SerializeField] private BombCase m_BombCase;

    private const float m_RepairPartsTimeLimit = .35f;

    public void RepairBomb(float repairStatusValue)
    {
        m_BombCase?.PlayFixBombPartsAnimation(repairStatusValue >= m_RepairPartsTimeLimit);
        m_BombCase?.RotateTopCase(repairStatusValue);
    }

    public void SetSmokeAlpha(float val)
    {
        m_Smoke.SetSmokeAlpha(val);
    }
}
