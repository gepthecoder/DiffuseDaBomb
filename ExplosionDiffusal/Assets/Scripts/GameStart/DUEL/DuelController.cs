using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class DuelConfigData {
    public SettingsItemData AxisConfigData;
    public SettingsItemData AlliesConfigData;

    public DuelConfigData() { }

    public DuelConfigData(SettingsItemData axisData, SettingsItemData alliesData) { AxisConfigData = axisData; AlliesConfigData = alliesData; }
}

public class DuelController : MonoBehaviour
{
    [SerializeField] private List<DuelObject> m_DuelObjects;
    [SerializeField] private Button m_PreviousButton;

    private DuelObjectType m_CurrentSelectedTeam = DuelObjectType.None;

    [HideInInspector] public UnityEvent<DuelObjectType> OnDuelObjectSelectedEvent = new UnityEvent<DuelObjectType>();
    [HideInInspector] public UnityEvent<DuelConfigData> OnDuelConfigSetEvent = new UnityEvent<DuelConfigData>();
    [HideInInspector] public UnityEvent OnPreviousButtonClickedEvent = new UnityEvent();

    private DuelConfigData m_DuelConfigData = new DuelConfigData();

    private void Awake()
    {
        m_DuelObjects.ForEach((OBJECT) => { 
            OBJECT.OnDuelObjectSelected.AddListener((TYPE) => {
                OnDuelObjectSelected(TYPE);
            });
        });

        m_PreviousButton.onClick.AddListener(() => {
            m_PreviousButton.interactable = false;
            
            OnPreviousButtonClickedEvent?.Invoke();
        });
    }

    private void OnDestroy()
    {
        m_DuelObjects.ForEach((OBJECT) => {
            OBJECT.OnDuelObjectSelected.RemoveAllListeners();
        });

        m_PreviousButton.onClick.RemoveAllListeners();
    }

    public void Init() {

        m_PreviousButton.transform.DOScale(1.1f, .5f).OnComplete(() => {
            m_PreviousButton.interactable = true;
            m_PreviousButton.transform.DOScale(1f, .25f);
        });
    }

    public void Deinit() {
        m_PreviousButton.transform.DOScale(0f, .5f).OnComplete(() => {
            m_PreviousButton.interactable = true;
        });

        OnDuelObjectSelected(DuelObjectType.None);
    }

    public void OnSettingsChanged(SettingsItemData data)
    {
        DuelObject duelObj = GetDuelObjByType(data.Type == SettingsItemType.Axis ? DuelObjectType.Attacker : DuelObjectType.Defender);
        duelObj.OnSettingsChanged(data, ()=> {
            if (IsConfigReady())
            {
                m_DuelConfigData.AxisConfigData = GetDuelObjByType(DuelObjectType.Attacker).m_ConfigData;
                m_DuelConfigData.AlliesConfigData = GetDuelObjByType(DuelObjectType.Defender).m_ConfigData;

                OnDuelConfigSetEvent?.Invoke(m_DuelConfigData);
            }
        });
    }

    private bool IsConfigReady()
    {
        foreach (var item in m_DuelObjects)
        {
            if (!item.IsDuelObjectReady())
            {
                return false;
            }
        }

        return true;
    }

    private void OnDuelObjectSelected(DuelObjectType TYPE)
    {
        if (TYPE == m_CurrentSelectedTeam)
            return;

        m_CurrentSelectedTeam = TYPE;

        m_DuelObjects.ForEach((obj) => {
            if (obj.ID == m_CurrentSelectedTeam) obj.OnSelected();
            else obj.OnDeSelected();
        });

        if(m_CurrentSelectedTeam != DuelObjectType.None)
        {
            OnDuelObjectSelectedEvent?.Invoke(m_CurrentSelectedTeam);
        }
    }

    public DuelObject GetDuelObjByType(DuelObjectType type)
    {
        foreach (var item in m_DuelObjects)
        {
            if(item.ID == type)
            {
                return item;
            }
        }

        return null;
    }

    internal void DeActivateDuelObjectInteractability()
    {
        m_DuelObjects.ForEach((duelObj) => {
            duelObj.SetInteractability(false);
        });
    }

    public void HidePreviousButton()
    {
        m_PreviousButton.transform.DOScale(0f, .3f);
    }
} 
