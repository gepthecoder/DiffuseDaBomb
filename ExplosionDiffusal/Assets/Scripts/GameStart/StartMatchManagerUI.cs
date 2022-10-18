using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class StartMatchManagerUI : MonoBehaviour
{
    
    [Header("START MATCH")]
    [SerializeField] private Button m_StartMatchButton;
    [Header("PRE MATCH")]
    [SerializeField] private Transform m_GameTile;
    [SerializeField] private Transform m_GameTileEndPoint;
    [SerializeField] private Image m_BackgroundImage;

    [HideInInspector] public UnityEvent OnStartMatchButtonClickedEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<Action> OnFadeOutEffectEvent = new UnityEvent<Action>();

    private void Awake()
    {
        m_StartMatchButton.onClick.AddListener(() => {
            OnStartMatchButtonClickedEvent?.Invoke();
        });
    }

    public void TriggerBehaviour(StartMatchState state)
    {
        switch (state)
        {
            case StartMatchState.Initial:
                OnFadeOutEffectEvent?.Invoke(() => {
                    SpawnLayout();
                });
                break;
            default:
                break;
        }
    }

    private void SpawnLayout()
    {
        m_GameTile.DOScale(new Vector3(1.2f,1.2f,1.2f), 1f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => {
                m_BackgroundImage.DOColor(new Color(
                    m_BackgroundImage.color.r, 
                    m_BackgroundImage.color.g,
                    m_BackgroundImage.color.b,
                    .77f), 1f);
                m_GameTile.DOScale(Vector3.one, 1f).SetEase(Ease.InCubic);
                m_GameTile.DOMoveY(m_GameTileEndPoint.position.y, 1f);
            });
    }

}
