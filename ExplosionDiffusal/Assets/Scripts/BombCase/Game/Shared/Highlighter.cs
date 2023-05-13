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
    public ClickableType ID;

    private float m_WobbleIntensity;
    private Vector3 m_InitalScale;

    private void Start()
    {
        m_WobbleIntensity = GetWobbleIntensityByType(Type);
        m_InitalScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void HighlightMe()
    {
        if (!CanHiglight)
        {
            transform.localScale = m_InitalScale;
            return;
        }

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
