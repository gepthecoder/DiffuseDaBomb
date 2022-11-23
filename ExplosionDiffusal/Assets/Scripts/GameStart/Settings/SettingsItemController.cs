using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsItemController : MonoBehaviour
{
    [SerializeField] private SettingsItem m_SettingItems_Axis;
    [SerializeField] private SettingsItem m_SettingItems_Allies;

    [SerializeField] private DuelController m_DuelController;

    private void Awake()
    {
        Sub();
    }
    private void OnDestroy()
    {
        DeSub();
    }

    private void Sub()
    {
        m_SettingItems_Axis.OnSettingsItemChanged.AddListener((data) =>
        {
            m_DuelController.OnSettingsChanged(data);
        });

        m_SettingItems_Allies.OnSettingsItemChanged.AddListener((data) =>
        {
            m_DuelController.OnSettingsChanged(data);
        });
    }

    private void DeSub()
    {
        m_SettingItems_Axis.OnSettingsItemChanged.RemoveAllListeners();
        m_SettingItems_Allies.OnSettingsItemChanged.RemoveAllListeners();
    }
}

