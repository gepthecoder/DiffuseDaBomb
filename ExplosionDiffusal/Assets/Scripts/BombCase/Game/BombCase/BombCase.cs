using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BombCase : MonoBehaviour
{
    [SerializeField] private Animator m_FixPartsAnimator;
    [Space(5)]
    [SerializeField] private Transform m_TopCasePart;
    [SerializeField] private Transform m_BombTransform;

    private float m_CaseOpenedValue = 87f;

    private int m_ShakeIntensityNormal= 5;
    private int m_ShakeIntensityIntense= 9;
    private int m_ShakeIntensity;

    private float m_ShakeStrenght = .12f;

    private BombCaseState i_State;

    public void Init()
    {
        m_ShakeIntensity = m_ShakeIntensityNormal;
    }

    public void TriggerBehaviour(BombCaseState state, Action callback = null)
    {
        i_State = state;

        switch (state)
        {
            case BombCaseState.Close:
                Debug.Log("<color=yellow>BombCaseState</color><color=gold>Close</color>");
                WobbleBombCase();
                CloseBombCase();
                break;
            case BombCaseState.Open:
                Debug.Log("<color=yellow>BombCaseState</color><color=gold>Open</color>");
                OpenBombCase(callback);
                break;
            case BombCaseState.Hacking:
                Debug.Log("<color=yellow>BombCaseState</color><color=gold>Hacking</color>");
                break;
            default:
                break;
        }
    }

    private void OpenBombCase(Action callback)
    {
        m_TopCasePart.DOLocalRotate(new Vector3(0f, m_CaseOpenedValue, 0f), 1.5f).SetEase(Ease.OutExpo).OnComplete(() => 
        {
            callback();
        });
    }
    private void CloseBombCase()
    {
        m_TopCasePart.DOLocalRotate(Vector3.zero, 2f).SetEase(Ease.OutSine).OnComplete(() => {  });
    }
    private void WobbleBombCase()
    {
        if (i_State != BombCaseState.Close)
            return;

        m_BombTransform.DOKill();
        m_BombTransform.DOShakePosition(2f, m_ShakeStrenght, m_ShakeIntensity)
            .SetEase(Ease.InOutBounce)
            .OnComplete(() => { WobbleBombCase(); });
    }

    public void SetWobbleIntensity(BombCaseSubState situation)
    {
        switch (situation)
        {
            case BombCaseSubState.OnBombCasePressDown:
                m_ShakeIntensity = m_ShakeIntensityIntense;
                m_ShakeStrenght = .2f;
                break;
            case BombCaseSubState.OnBombCasePressUp:
                m_ShakeIntensity = m_ShakeIntensityNormal;
                m_ShakeStrenght = .12f;
                break;
            case BombCaseSubState.NonInteractive:
                m_ShakeIntensity = m_ShakeIntensityNormal;
                m_ShakeStrenght = .12f;
                break;
            default:
                m_ShakeIntensity = m_ShakeIntensityNormal;
                break;

        }
    }

    private bool _isFixing = false;
    public void PlayFixBombPartsAnimation(bool play)
    {
        if(_isFixing != play)
            m_FixPartsAnimator?.SetTrigger(play ? "start" : "stop");
        
        _isFixing = play;
    }

    public void RotateTopCase(float normal)
    {
        m_TopCasePart.DOLocalRotate(
            normal >= .5f ? 
            Vector3.zero : 
            new Vector3(0f, m_CaseOpenedValue, 0f), 2.5f)
            .SetEase(normal >= .5f ? 
                Ease.OutSine : 
                Ease.OutExpo);
    }
}
