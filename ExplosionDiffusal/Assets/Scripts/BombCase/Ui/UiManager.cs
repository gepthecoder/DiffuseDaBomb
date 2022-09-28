using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Image m_DarkenPanel;

    [HideInInspector] public UnityEvent OnFadeInEvent = new UnityEvent();

    public void FadeInScreen()
    {
        m_DarkenPanel.DOFade(1f, 2f).SetEase(Ease.InOutQuad).OnComplete(() => { OnFadeInEvent?.Invoke(); });
    }

    public void FadeOutScreen()
    {
        m_DarkenPanel.DOFade(0, 1f).SetEase(Ease.InOutQuad);
    }
}
