using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*
    >> MODE SELECTION                   (0)
    >> MATCH SETTINGS                   
        >> GameTime                     (1)
        >> BombTime                     (2)
        >> StartMatchCountdownTime      (3)
        >> ScoreLimit                   (4)
    >> DUEL                             (5)

    Total:                               6

*/

[System.Serializable]
public class NavigationObject
{
    [Range(0, 5)]
    public int ID;
    public Image m_Image;
}

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager instance;

    [Header("UI")]
    [SerializeField] private Transform m_NavigationObjectUI;
    [SerializeField] private List<NavigationObject> m_NavigationObjectList;
    [Space(5)]
    [SerializeField] private Color m_DefaultColor;
    [SerializeField] private Color m_HighlightColor;

    private int m_CurrentNavigationPointer = 0;

    private void Awake()
    {
        instance = this;
    }

    public void ShowMenuNavigation(bool show)
    {
        m_NavigationObjectUI.DOScale(show ? 1f : 0f, 1f).SetEase(Ease.InBack);
    }

    public void SetNavigationPointerByState(StartMatchState mainState, MatchSettingsStateType matchSettingState = MatchSettingsStateType.None)
    {
        if(matchSettingState != MatchSettingsStateType.None)
        {
            switch (matchSettingState)
            {
                case MatchSettingsStateType.GameTime:
                    m_CurrentNavigationPointer = 1;
                    break;
                case MatchSettingsStateType.BombTime:
                    m_CurrentNavigationPointer = 2;
                    break;
                case MatchSettingsStateType.StartMatchCountdownTime:
                    m_CurrentNavigationPointer = 3;
                    break;
                case MatchSettingsStateType.ScoreLimit:
                    m_CurrentNavigationPointer = 4;
                    break;

                default:
                    m_CurrentNavigationPointer = -1;
                    break;
            }

        } else
        {
            switch (mainState)
            {
                case StartMatchState.ModeSelection:
                    m_CurrentNavigationPointer = 0;
                    ShowMenuNavigation(true);
                    break;
                case StartMatchState.Duel:
                    m_CurrentNavigationPointer = 5;
                    break;
               
                default:
                    m_CurrentNavigationPointer = -1;
                    break;
            }
        }

        if(m_CurrentNavigationPointer != -1)
        {
            HighlightNavigationPointerByID(m_CurrentNavigationPointer);
        }
    }

    private void HighlightNavigationPointerByID(int ID)
    {
        m_NavigationObjectList.ForEach((navObj) => {
            navObj.m_Image.DOColor(navObj.ID == ID ? m_HighlightColor : m_DefaultColor, .77f);
        });
    }
}
