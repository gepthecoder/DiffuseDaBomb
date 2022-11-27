using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System;

public class GlobalConfig
{
    public GameModeType __GAME_MODE_TYPE__;
    public MatchSettingsConfigData __MATCH_SETTINGS__;
    public SettingsItemData __DUEL_SETTINGS__;

    public GlobalConfig(GameModeType type) {
        this.__GAME_MODE_TYPE__ = type;
    }

    public GlobalConfig(SettingsItemData data)
    {
        this.__DUEL_SETTINGS__ = data;
    }

    public GlobalConfig(MatchSettingsConfigData data)
    {
        this.__MATCH_SETTINGS__ = data;
    }

    public GlobalConfig() { }
}


public enum StartMatchState { Initial, PlayMatchMain, SettingsMain, MatchSettings, Duel, TeamAConfig, TeamBConfig, }

public class StartMatchManager : MonoBehaviour
{
    [SerializeField] private StartMatchManagerUI m_StartMatchManagerUI;
    [SerializeField] private DuelController m_DuelController;
    [Space(5)]
    [SerializeField] private Transform t_MainCamera;
    [Space(5)]
    [SerializeField] private Transform t_CameraStartPosition;
    [SerializeField] private Transform t_CameraEndPosition;

    private GlobalConfig m_GlobalConfigData = new GlobalConfig();

    [HideInInspector] public UnityEvent OnStartMatchEvent = new UnityEvent();

    private void Awake()
    {
        Sub();
    }

    private void OnDestroy()
    {
        DeSub();
    }

    private void DeSub()
    {
        m_StartMatchManagerUI.OnStartMatchButtonClickedEvent.RemoveListener(StartMatch);
        m_StartMatchManagerUI.OnGameModeSelectedEvent.RemoveAllListeners();
        m_DuelController.OnDuelObjectSelectedEvent.RemoveAllListeners();
    }

    private void Sub()
    {
        m_StartMatchManagerUI.OnStartMatchButtonClickedEvent.AddListener(StartMatch);
        m_DuelController.OnDuelObjectSelectedEvent.AddListener((TYPE) => {
            m_StartMatchManagerUI.TriggerBehaviour(TYPE == DuelObjectType.Attacker ? 
                StartMatchState.TeamAConfig : StartMatchState.TeamBConfig);
        });

        m_StartMatchManagerUI.OnGameModeSelectedEvent.AddListener((mode) => {
            m_GlobalConfigData.__GAME_MODE_TYPE__ = mode;
        });

        m_StartMatchManagerUI.OnMatchSettingsSetEvent.AddListener((DATA) => {
            m_GlobalConfigData.__MATCH_SETTINGS__ = DATA;
        });
    }

    internal void Init()
    {
        t_MainCamera.position = t_CameraStartPosition.position;
        t_MainCamera.localEulerAngles = new Vector3(t_CameraStartPosition.rotation.x, t_CameraStartPosition.rotation.y, t_CameraStartPosition.rotation.z);

        m_StartMatchManagerUI.TriggerBehaviour(StartMatchState.Initial);
    }

    public void StartMatch()
    {
        ZoomInToCaseSeq();
    }

    private void ZoomInToCaseSeq()
    {
        t_MainCamera.DOMove(t_CameraEndPosition.position, 3f)
            .SetEase(Ease.InOutBack)
            .OnComplete(() => {
                OnStartMatchEvent?.Invoke();
            });
    }

   
}
