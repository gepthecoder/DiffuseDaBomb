using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RematchManager : MonoBehaviour
{
    [Header("Match Settings")]
    [SerializeField] private MatchSettingsItem m_MSGameTime;
    [SerializeField] private MatchSettingsItem m_MSBombTime;
    [SerializeField] private MatchSettingsItem m_MSStartDelay;
    [SerializeField] private MatchSettingsItem m_MSRoundLimit;
    [Header("Duel")]
    [SerializeField] private RematchDuelObject m_Allies;
    [SerializeField] private RematchDuelObject m_Axis;
    [Space(10)]
    [SerializeField] private GameObject m_Parent;

    protected GlobalConfig ___Global_Config___;

    [HideInInspector] public UnityEvent<GlobalConfig> OnReadyEvent = new UnityEvent<GlobalConfig>();

    private bool m_Ready = false;

    public void InitRematchModule(GlobalConfig cfg)
    {
        if (cfg == null)
            return;

        m_Ready = false;

        ___Global_Config___ = cfg;

        // Set Module Data: Match Settings / Duel
        m_MSGameTime.InitSettingsItemValue(cfg.__MATCH_SETTINGS__.GameTimeInMinutes);
        m_MSBombTime.InitSettingsItemValue(cfg.__MATCH_SETTINGS__.BombTimeInMinutes);
        m_MSStartDelay.InitSettingsItemValue(cfg.__MATCH_SETTINGS__.MatchStartTimeInSeconds);
        m_MSRoundLimit.InitSettingsItemValue(cfg.__MATCH_SETTINGS__.ScoreLimit);

        var axisData = cfg.__DUEL_SETTINGS__.AxisConfigData;
        var alliesData = cfg.__DUEL_SETTINGS__.AlliesConfigData;

        m_Axis.InitRematchDuelObject(axisData);
        m_Allies.InitRematchDuelObject(alliesData);

        // Show Module
        m_Parent.SetActive(true);

        // Add Listeners and Adjust CFG

        // Match Settings
        m_MSGameTime.OnMatchSettingsItemValueChangedEvent.AddListener((val) => {
            ___Global_Config___.__MATCH_SETTINGS__.GameTimeInMinutes = val;
        });
        m_MSBombTime.OnMatchSettingsItemValueChangedEvent.AddListener((val) => {
            ___Global_Config___.__MATCH_SETTINGS__.BombTimeInMinutes = val;
        });
        m_MSStartDelay.OnMatchSettingsItemValueChangedEvent.AddListener((val) => {
            ___Global_Config___.__MATCH_SETTINGS__.MatchStartTimeInSeconds = val;
        });
        m_MSRoundLimit.OnMatchSettingsItemValueChangedEvent.AddListener((val) => {
            ___Global_Config___.__MATCH_SETTINGS__.ScoreLimit = val;
        });

        // Duel

        // AXIS
        m_Axis.OnTeamAvatarChangedEvent.AddListener((mapper) => {
            ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamEmblem = mapper;
        });

        m_Axis.OnTeamNameChangedEvent.AddListener((name) => {
            ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamName = name;
        });

        m_Axis.OnTeamCountChangedEvent.AddListener((count) => {
            ___Global_Config___.__DUEL_SETTINGS__.AxisConfigData.TeamCount = count;
        });

        // ALLIES
        m_Allies.OnTeamAvatarChangedEvent.AddListener((mapper) => {
            ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamEmblem = mapper;
        });

        m_Allies.OnTeamNameChangedEvent.AddListener((name) => {
            ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamName = name;
        });

        m_Allies.OnTeamCountChangedEvent.AddListener((count) => {
            ___Global_Config___.__DUEL_SETTINGS__.AlliesConfigData.TeamCount = count;
        });
    }

    public void DeinitRematchModule()
    {
        // Hide Module
        m_Parent.SetActive(false);

        // Remove Listeners and Adjust CFG
        m_MSGameTime.OnMatchSettingsItemValueChangedEvent.RemoveAllListeners();
        m_MSBombTime.OnMatchSettingsItemValueChangedEvent.RemoveAllListeners();
        m_MSStartDelay.OnMatchSettingsItemValueChangedEvent.RemoveAllListeners();
        m_MSRoundLimit.OnMatchSettingsItemValueChangedEvent.RemoveAllListeners();
    }

    #region Button Events

    public void OnReadyButtonClicked()
    {
        if(!m_Ready)
        {
            OnReadyEvent?.Invoke(___Global_Config___);

            DeinitRematchModule();

            m_Ready = true;
        }
    }

    public void CloseRematchButtonClicked()
    {
        DeinitRematchModule();
    }

    #endregion
}
