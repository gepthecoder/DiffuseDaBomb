using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum HighlightType
{
    Default, Normal, Intense, 
}

public class Highlighter : MonoBehaviour
{
    public bool CanHiglight = true;
    public HighlightType Type = HighlightType.Default;

    private float m_WobbleIntensity;

    private void Start()
    {
        m_WobbleIntensity = GetWobbleIntensityByType(Type);
    }

    public void HighlightMe()
    {
        if (!CanHiglight)
            return;
        transform.DOShakeScale(2f, m_WobbleIntensity, 5, 20).OnComplete(() => { HighlightMe(); });
    }

    public void StopHighlightEffect()
    {
        CanHiglight = false;
    }

    private float GetWobbleIntensityByType(HighlightType type)
    {
        switch (type)
        {
            case HighlightType.Normal:
                return .2f;
            case HighlightType.Intense:
                return .35f;
        case HighlightType.Default:
            default:
                return .15f;
        }
    }
}
