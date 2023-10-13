using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Events;

public class SuitcaseHelper : MonoBehaviour
{
    public static SuitcaseHelper INSTANCE;

    [SerializeField] private CanvasGroup m_SuitcaseCGroup;

    [HideInInspector] public UnityEvent OnCloseSuitcaseButtonEvent = new UnityEvent();

    private void Awake()
    {
        INSTANCE = this;

        if (OnCloseSuitcaseButtonEvent == null)
        {
            OnCloseSuitcaseButtonEvent = new UnityEvent();
        }
    }

    public void ShowCloseSuitcaseButton(bool show)
    {
        m_SuitcaseCGroup.interactable = show;

        float endVal = show ? 1f : 0f;
        m_SuitcaseCGroup.DOFade(endVal, .3f);
    }

    public void CloseSuitcase()
    {
        ShowCloseSuitcaseButton(false);

        OnCloseSuitcaseButtonEvent?.Invoke();
    }

}
