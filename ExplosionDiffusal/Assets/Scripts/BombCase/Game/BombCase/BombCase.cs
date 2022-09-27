using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BombCase : MonoBehaviour
{
    [SerializeField] private Transform m_TopCasePart;
    [SerializeField] private Transform m_BombTransform;

    private float m_CaseOpenedValue = 87f;

    private BombCaseState i_State;

    public void TriggerBehaviour(BombCaseState state, Action callback = null)
    {
        i_State = state;

        switch (state)
        {
            case BombCaseState.Close:
                WobbleBombCase();
                break;
            case BombCaseState.Open:
                OpenBombCase(callback);
                break;
            case BombCaseState.ZoomIn:
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
        m_TopCasePart.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f).SetEase(Ease.OutSine).OnComplete(() => {  });
    }
    private void WobbleBombCase()
    {
        if (i_State != BombCaseState.Close)
            return;

        m_BombTransform.DOShakePosition(2f, .3f, 7).OnComplete(() => { WobbleBombCase(); });
    }

}
