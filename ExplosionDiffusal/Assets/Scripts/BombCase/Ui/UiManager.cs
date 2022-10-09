using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Keyboard m_KeyboardUI;
    [SerializeField] private Keypad m_KeypadUI;

    [SerializeField] private Image m_DarkenPanel;

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
    public void EnableKeyPadUI()
    {
        m_KeypadUI.EnableObject(true);
    }
    public void DisableKeyBoardUI()
    {
        m_KeyboardUI.EnableObject(false);
    }
    public void DisableKeyPadUI()
    {
        m_KeypadUI.EnableObject(false);
    }
}
