using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using System;

public enum StartMatchState { Initial, PlayMatchMain, SettingsMain, }

public class StartMatchManager : MonoBehaviour
{
    [SerializeField] private StartMatchManagerUI m_StartMatchManagerUI;
    [SerializeField] private Transform t_MainCamera;
    [Space(5)]
    [SerializeField] private Transform t_CameraStartPosition;
    [SerializeField] private Transform t_CameraEndPosition;

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
    }

    private void Sub()
    {
        m_StartMatchManagerUI.OnStartMatchButtonClickedEvent.AddListener(StartMatch);
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
