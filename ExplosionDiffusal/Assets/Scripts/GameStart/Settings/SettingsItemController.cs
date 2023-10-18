using System;
using UnityEngine;

public class SettingsItemController : MonoBehaviour
{
    [SerializeField] private SettingsItem m_SettingItems_Axis;
    [SerializeField] private SettingsItem m_SettingItems_Allies;
    [Space(5)]
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
            if(data.TeamName != string.Empty)
            {
                TryDisableTeamNameOption(SettingsItemType.Axis, data.TeamName);
            }

            if(data.TeamEmblem != null)
            {
                TryDisableTeamEmblemOption(SettingsItemType.Axis, data.TeamEmblem.hasMaterial ?  data.TeamEmblem.MATERIAL.mainTexture.name : data.TeamEmblem.SPRITE.name);
            }

            m_DuelController.OnSettingsChanged(data);
        });

        m_SettingItems_Allies.OnSettingsItemChanged.AddListener((data) =>
        {
            if (data.TeamName != string.Empty)
            {
                TryDisableTeamNameOption(SettingsItemType.Allies, data.TeamName);
            }

            if (data.TeamEmblem != null)
            {
                TryDisableTeamEmblemOption(SettingsItemType.Allies, data.TeamEmblem.hasMaterial ? data.TeamEmblem.MATERIAL.mainTexture.name : data.TeamEmblem.SPRITE.name);
            }

            m_DuelController.OnSettingsChanged(data);
        });
    }

    private void TryDisableTeamNameOption(SettingsItemType team, string tNameString)
    {
        switch (team)
        {
            case SettingsItemType.Axis:
                {
                    m_SettingItems_Allies.DistinctTeamNameValuesPlease(tNameString);
                }
                break;
            case SettingsItemType.Allies:
                {
                    m_SettingItems_Axis.DistinctTeamNameValuesPlease(tNameString);
                }
                break;
            default:
                break;
        }
    }

    private void TryDisableTeamEmblemOption(SettingsItemType team, string tEmblemID)
    {
        switch (team)
        {
            case SettingsItemType.Axis:
                {
                    m_SettingItems_Allies.DistinctTeamEmblemsPlease(tEmblemID);
                }
                break;
            case SettingsItemType.Allies:
                {
                    m_SettingItems_Axis.DistinctTeamEmblemsPlease(tEmblemID);
                }
                break;
            default:
                break;
        }
    }

    private void DeSub()
    {
        m_SettingItems_Axis.OnSettingsItemChanged.RemoveAllListeners();
        m_SettingItems_Allies.OnSettingsItemChanged.RemoveAllListeners();
    }
}

