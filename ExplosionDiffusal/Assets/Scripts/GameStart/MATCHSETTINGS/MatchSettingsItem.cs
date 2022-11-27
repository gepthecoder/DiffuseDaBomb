using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MatchSettingsItem : MonoBehaviour
{
    public MatchSettingsStateType Type = MatchSettingsStateType.None;

    [SerializeField] private Animator m_MatchSettingsItemAnimator;
    [SerializeField] private CanvasGroup m_MatchSettingsItemCGroup;

    public void OnShowItem()
    {
        m_MatchSettingsItemAnimator.Play("show");
    }

    public void OnHideItem()
    {
        m_MatchSettingsItemAnimator.Play("hide");
    }

    public CanvasGroup GetCanvasGroup()
    {
        return m_MatchSettingsItemCGroup;
    }

}
