using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public enum MatchSettingsStateType { GameTime, BombTime, ScoreLimit, None, }

public class MatchSettingsConfigData
{
    public int GameTimeInMinutes;
    public int BombTimeInMinutes;
    public int ScoreLimit;

    public MatchSettingsConfigData() { }

    public MatchSettingsConfigData(int gT, int bT, int scoreLimit)
    {
        this.GameTimeInMinutes = gT;
        this.BombTimeInMinutes = bT;
        this.ScoreLimit = scoreLimit;
    }
}

public class MatchSettingsController : MonoBehaviour
{
    [HideInInspector] public UnityEvent<MatchSettingsConfigData> OnSetMatchSettingsDoneEvent = new UnityEvent<MatchSettingsConfigData>();

    [SerializeField] private List<MatchSettingsItem> m_MatchSettingsItems;

    private MatchSettingsStateType m_CurrentState = MatchSettingsStateType.None;

    [SerializeField] private Button m_NextButton;

    private MatchSettingsConfigData m_Data = new MatchSettingsConfigData();

    private void Awake()
    {
        DefaultSetup();

        m_NextButton.onClick.AddListener(() => {
          
            m_NextButton.interactable = false;

            m_NextButton.transform.DOScale(1.15f, .25f).OnComplete(() => {
                m_NextButton.transform.DOScale(0f, .77f).OnComplete(() => {

                    GetMatchSettingsByType(m_CurrentState).OnHideItem();

                    // GO TO NEXT STATE AND SAVE CONFIG
                    if (m_CurrentState == MatchSettingsStateType.ScoreLimit)
                    {
                        // SHOW DUEL with small delay
                        StartCoroutine(OnMatchSettingsDoneDelay());
                    }
                    else
                    {
                        // SHOW NEXT STATE, HIDE PREVIOUS STATE
                        TriggerBehaviour(m_CurrentState + 1);
                    }
                });
            });
        });
    }

    private IEnumerator OnMatchSettingsDoneDelay()
    {
        yield return new WaitForSeconds(1f);
        OnSetMatchSettingsDoneEvent?.Invoke(m_Data);

    }

    private void DefaultSetup()
    {
        m_MatchSettingsItems.ForEach((item) => {
            item.GetCanvasGroup().alpha = 0;
            item.GetCanvasGroup().interactable = false;
        });
    }

    public void TriggerBehaviour(MatchSettingsStateType state)
    {
        m_CurrentState = state;

        switch (state)
        {
            case MatchSettingsStateType.GameTime:
                m_NextButton.transform.DOScale(1.15f, .77f).OnComplete(() => {
                    m_NextButton.transform.DOScale(1f, .25f).OnComplete(() => {
                        m_NextButton.interactable = true;
                    });
                });
                GetMatchSettingsByType(state).OnShowItem();
                break;
            case MatchSettingsStateType.BombTime:
                m_NextButton.transform.DOScale(1.15f, .77f).OnComplete(() => {
                    m_NextButton.transform.DOScale(1f, .25f).OnComplete(() => {
                        m_NextButton.interactable = true;
                    });
                });
                GetMatchSettingsByType(state).OnShowItem();
                break;
            case MatchSettingsStateType.ScoreLimit:
                m_NextButton.transform.DOScale(1.15f, .77f).OnComplete(() => {
                    m_NextButton.transform.DOScale(1f, .25f).OnComplete(() => {
                        m_NextButton.interactable = true;
                    });
                });
                GetMatchSettingsByType(state).OnShowItem();
                break;
            case MatchSettingsStateType.None:
                break;
            default:
                break;
        }
    }

    private MatchSettingsItem GetMatchSettingsByType(MatchSettingsStateType type)
    {
        MatchSettingsItem matchSettingsItem = null;

        m_MatchSettingsItems.ForEach((item) => {
            if(item.Type == type)
            {
                matchSettingsItem = item;
            }
        });

        return matchSettingsItem;
    }
}
