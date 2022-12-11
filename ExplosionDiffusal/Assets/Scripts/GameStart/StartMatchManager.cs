using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
public enum StartMatchState { Initial, ModeSelection, SettingsMain, MatchSettings, Duel, TeamAConfig, TeamBConfig, }

public class StartMatchManager : MonoBehaviour
{
    [SerializeField] private StartMatchManagerUI m_StartMatchManagerUI;
    [SerializeField] private DuelController m_DuelController;
    [Space(5)]
    [SerializeField] private Transform t_MainCamera;
    [Space(5)]
    [SerializeField] private Transform t_CameraStartPosition;

    [HideInInspector] public UnityEvent<GameState, GlobalConfig> OnStartMatchEvent = new UnityEvent<GameState , GlobalConfig>();

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
        m_StartMatchManagerUI.OnStartMatchButtonClickedEvent.RemoveAllListeners();
        m_DuelController.OnDuelObjectSelectedEvent.RemoveAllListeners();
    }

    private void Sub()
    {
        m_StartMatchManagerUI.OnStartMatchButtonClickedEvent.AddListener((configData) => {
            StartMatch(configData);
        });

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

    public void StartMatch(GlobalConfig data)
    {
        ZoomInToCaseSeq(data);
    }

    private void ZoomInToCaseSeq(GlobalConfig data)
    {
        Vector3 camEndPos, camEndRot;
        (camEndPos, camEndRot) = CameraManager.Instance.GetInitalCameraPositionAndRotation();

        t_MainCamera.DORotate(camEndRot, 1.5f);

        t_MainCamera.DOJump(camEndPos, 1f, 1, 3f)
            .SetEase(Ease.OutBack);

        StartCoroutine(InvokeCountdownStateOnDelay(1.5f, data));
    }

    private IEnumerator InvokeCountdownStateOnDelay(float delay, GlobalConfig data)
    {
        yield return new WaitForSeconds(delay);
        OnStartMatchEvent?.Invoke(GameState.Countdown, data);
        m_StartMatchManagerUI.DisableStartMatchCanvas();

    }
}
