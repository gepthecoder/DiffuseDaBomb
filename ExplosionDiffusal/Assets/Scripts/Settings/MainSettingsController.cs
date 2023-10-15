using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSettingsController : MonoBehaviour
{
    [SerializeField] private Animator m_SettingsAnimator;
    [Space(5)]
    [SerializeField] private SnapToItem m_SnapToItemTeamEmblem;
    [Space(5)]
    [SerializeField] private FilePicker m_ImagePicker;

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

    #region Team Emblem Settings

    public void OnAddCustomEmblemButtonClicked()
    {
        m_ImagePicker.LoadFile();
    }

    #endregion

    #region Team Name Settings

    #endregion

    #region Audio Settings

    #endregion
}
