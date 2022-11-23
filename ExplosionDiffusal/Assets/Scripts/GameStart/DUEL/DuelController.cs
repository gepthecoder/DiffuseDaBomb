using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DuelController : MonoBehaviour
{
    [SerializeField] private List<DuelObject> m_DuelObjects;

    private DuelObjectType m_CurrentSelectedTeam = DuelObjectType.None;

    [HideInInspector] public UnityEvent<DuelObjectType> OnDuelObjectSelectedEvent = new UnityEvent<DuelObjectType>();
    [HideInInspector] public UnityEvent OnDuelConfigSetEvent = new UnityEvent();

    private bool m_IsConfigReady = false;

    private void Awake()
    {
        m_DuelObjects.ForEach((OBJECT) => { 
            OBJECT.OnDuelObjectSelected.AddListener((TYPE) => {
                OnDuelObjectSelected(TYPE);
            });
        });
    }

    private void OnDestroy()
    {
        m_DuelObjects.ForEach((OBJECT) => {
            OBJECT.OnDuelObjectSelected.RemoveAllListeners();
        });
    }

    public void OnSettingsChanged(SettingsItemData data)
    {
        DuelObject duelObj = GetDuelObjByType(data.Type == SettingsItemType.Axis ? DuelObjectType.Attacker : DuelObjectType.Defender);
        duelObj.OnSettingsChanged(data, ()=> {
            if (m_IsConfigReady)
                return;

            if (IsConfigReady())
            {
                m_IsConfigReady = true;
                OnDuelConfigSetEvent?.Invoke();
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

        OnDuelObjectSelectedEvent?.Invoke(m_CurrentSelectedTeam);
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
} 
