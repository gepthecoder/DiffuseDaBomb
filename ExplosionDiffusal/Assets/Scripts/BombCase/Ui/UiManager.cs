using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System;

public class UiManager : MonoBehaviour
{
    [SerializeField] private StartMatchManagerUI m_StartMatchManagerUI;
    [Space(5)]
    [SerializeField] private Keyboard m_KeyboardUI;
    [SerializeField] private Keypad m_KeypadUI;
    [Space(5)]
    [SerializeField] private Image m_DarkenPanel;

    [HideInInspector] public UnityEvent OnFadeInEvent = new UnityEvent();

    private void Awake() { Sub(); }

    private void OnDestroy() { DeSub(); }

    private void Start() => SetupScene();

    private void SetupScene()
    {
        m_DarkenPanel.gameObject.SetActive(true);
    }

    private void Sub()
    {
        m_StartMatchManagerUI.OnFadeOutEffectEvent.AddListener((ACTION) =>
        {
            FadeOutScreenAction(ACTION);
        });

    }

    private void DeSub()
    {
        m_StartMatchManagerUI.OnFadeOutEffectEvent.RemoveAllListeners();
    }

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

    public void FadeOutScreenAction(Action action)
    {
        m_DarkenPanel.DOFade(0, .77f).SetEase(Ease.InOutQuad)
            .OnComplete(() => {
                action();
            });
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
