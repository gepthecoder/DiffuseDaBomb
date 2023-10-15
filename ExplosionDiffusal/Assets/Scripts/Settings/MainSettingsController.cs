using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSettingsController : MonoBehaviour
{
    [SerializeField] private Animator m_SettingsAnimator;
    [SerializeField] private SnapToItem m_SnapToItemTeamEmblem;

    public void InitMainSettings()
    {
        m_SettingsAnimator.Play("SHOW");
        m_SnapToItemTeamEmblem.InitSnap();
    }

    public void DeinitMainSettings()
    {
        m_SettingsAnimator.Play("HIDE");
        m_SnapToItemTeamEmblem.DeinitSnap();
    }
}
