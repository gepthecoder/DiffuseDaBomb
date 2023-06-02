using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public enum MatchSettingsStateType { GameTime, BombTime, StartMatchCountdownTime, ScoreLimit, None, }

public class MatchSettingsConfigData
{
    public int GameTimeInMinutes;
    public int BombTimeInMinutes;
    public int MatchStartTimeInSeconds;
    public int ScoreLimit;

    public MatchSettingsConfigData() { }

    public MatchSettingsConfigData(int gT, int bT, int scoreLimit, int sMcT)
    {
        this.GameTimeInMinutes = gT;
        this.BombTimeInMinutes = bT;
        this.ScoreLimit = scoreLimit;
        this.MatchStartTimeInSeconds = sMcT;
    }
}

public class MatchSettingsController : MonoBehaviour
{
    [HideInInspector] public UnityEvent<MatchSettingsConfigData> OnSetMatchSettingsDoneEvent = new UnityEvent<MatchSettingsConfigData>();
    [HideInInspector] public UnityEvent OnBackToGameModeEvent = new UnityEvent();

    [SerializeField] private List<MatchSettingsItem> m_MatchSettingsItems;

    private MatchSettingsStateType m_CurrentState = MatchSettingsStateType.None;

    [SerializeField] private Button m_NextButton;
    [SerializeField] private Button m_PreviousButton;

    private MatchSettingsConfigData m_Data = new MatchSettingsConfigData();

    private void Awake()
    {
        DefaultSetup();

        m_PreviousButton.onClick.AddListener(() => {
            m_PreviousButton.interactable = false;
            GetMatchSettingsByType(m_CurrentState).EnableArrowInteraction(false);

            m_PreviousButton.transform.DOScale(1.15f, .25f).OnComplete(() => {
                m_PreviousButton.transform.DOScale(0f, .77f).OnComplete(() => {

                    GetMatchSettingsByType(m_CurrentState).OnHideItem();

                    if(m_CurrentState == MatchSettingsStateType.GameTime) {

                        OnBackToGameModeEvent?.Invoke();

                    } else
                    {
                        TriggerBehaviour(m_CurrentState - 1);
                    }
                });
            });

        });

        m_NextButton.onClick.AddListener(() => {
          
            m_NextButton.interactable = false;
            GetMatchSettingsByType(m_CurrentState).EnableArrowInteraction(false);

            m_NextButton.transform.DOScale(1.15f, .25f).OnComplete(() => {
                m_NextButton.transform.DOScale(0f, .77f).OnComplete(() => {

                    GetMatchSettingsByType(m_CurrentState).OnHideItem();

                    // GO TO NEXT STATE AND SAVE CONFIG
                    if (m_CurrentState == MatchSettingsStateType.ScoreLimit)
                    {
                        // SHOW DUEL with small delay
                        m_PreviousButton.transform.DOScale(0f, .77f);
                        m_PreviousButton.interactable = false;
                        // save config
                        SaveConfig();

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



    private void SaveConfig()
    {
        m_MatchSettingsItems.ForEach((item) => {
            switch (item.Type)
            {
                case MatchSettingsStateType.GameTime:
                    m_Data.GameTimeInMinutes = item.m_Value;
                    break;
                case MatchSettingsStateType.BombTime:
                    m_Data.BombTimeInMinutes = item.m_Value;
                    break;
                case MatchSettingsStateType.ScoreLimit:
                    m_Data.ScoreLimit = item.m_Value;
                    break;
                case MatchSettingsStateType.StartMatchCountdownTime:
                    m_Data.MatchStartTimeInSeconds = item.m_Value;
                    break;
                case MatchSettingsStateType.None:
                default:
                    break;
            }
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
            item.GetCanvasGroup().blocksRaycasts = false;
        });
    }

    public void TriggerBehaviour(MatchSettingsStateType state)
    {
        m_CurrentState = state;

        // NEXT BUTTON
        m_NextButton.transform.DOScale(1.15f, .77f).OnComplete(() => {
            m_NextButton.transform.DOScale(1f, .25f).OnComplete(() => {
                m_NextButton.interactable = true;
            });
        });

        // PREVIOUS BUTTON
       
        m_PreviousButton.transform.DOScale(1.15f, .77f).OnComplete(() => {
            m_PreviousButton.transform.DOScale(1f, .25f).OnComplete(() => {
                m_PreviousButton.interactable = true;
            });
        });

        NavigationManager.instance.SetNavigationPointerByState(StartMatchState.ModeSelection, state);

        // SHOW MATCH SETTING BY TYPE
        GetMatchSettingsByType(state).OnShowItem();
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
