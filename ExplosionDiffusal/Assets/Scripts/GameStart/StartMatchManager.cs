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
    public DuelConfigData __DUEL_SETTINGS__;

    public GlobalConfig(GameModeType type) {
        this.__GAME_MODE_TYPE__ = type;
    }

    public GlobalConfig(DuelConfigData data)
    {
        this.__DUEL_SETTINGS__ = data;
    }

    public GlobalConfig(MatchSettingsConfigData data)
    {
        this.__MATCH_SETTINGS__ = data;
    }

    public GlobalConfig() { }
}


public enum StartMatchState { Initial, ModeSelection, SettingsMain, MatchSettings, Duel, TeamAConfig, TeamBConfig, }

public class StartMatchManager : MonoBehaviour
{
    [SerializeField] private StartMatchManagerUI m_StartMatchManagerUI;
    [SerializeField] private DuelController m_DuelController;
    [Space(5)]
    [SerializeField] private Transform t_MainCamera;
    [Space(5)]
    [SerializeField] private Transform t_CameraStartPosition;

    [HideInInspector] public UnityEvent<GameState> OnStartMatchEvent = new UnityEvent<GameState>();

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
        m_DuelController.OnDuelObjectSelectedEvent.RemoveAllListeners();
    }

    private void Sub()
    {
        m_StartMatchManagerUI.OnStartMatchButtonClickedEvent.AddListener(StartMatch);

        m_DuelController.OnDuelObjectSelectedEvent.AddListener((TYPE) => {
            m_StartMatchManagerUI.TriggerBehaviour(TYPE == DuelObjectType.Attacker ? 
                StartMatchState.TeamAConfig : StartMatchState.TeamBConfig);
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
        Vector3 camEndPos, camEndRot;
        (camEndPos, camEndRot) = CameraManager.Instance.GetInitalCameraPositionAndRotation();

        t_MainCamera.DORotate(camEndRot, 1.5f);

        t_MainCamera.DOJump(camEndPos, 1f, 1, 3f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                m_StartMatchManagerUI.DisableStartMatchCanvas();
                OnStartMatchEvent?.Invoke(GameState.Initial);
            });
    }

   
}
