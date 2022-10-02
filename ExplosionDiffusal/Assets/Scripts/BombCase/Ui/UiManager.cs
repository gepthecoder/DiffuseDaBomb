using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Image m_DarkenPanel;
    [SerializeField] private Keyboard m_KeyboardUI;

    [HideInInspector] public UnityEvent OnFadeInEvent = new UnityEvent();

    public void FadeInScreen(float duration, bool invoke)
    {
        m_DarkenPanel.DOFade(1f, duration).SetEase(Ease.InOutQuad).OnComplete(() => 
        { 
            if(invoke)
                OnFadeInEvent?.Invoke(); 
        });
    }

    public void FadeInOutScreen(float duration)
    {
        m_DarkenPanel.DOFade(1f, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            FadeOutScreen();
        });
    }

    public void FadeOutScreen()
    {
        m_DarkenPanel.DOFade(0, .77f).SetEase(Ease.InOutQuad);
    }

    public void EnableKeyBoardUI()
    {
        m_KeyboardUI.EnableObject(true);
    }
}