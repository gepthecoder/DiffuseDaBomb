using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;
using System;

public enum DuelObjectType { Attacker, Defender, None }

public class DuelObject : MonoBehaviour, IPointerClickHandler
{
    public DuelObjectType ID;
    [HideInInspector] public UnityEvent<DuelObjectType> OnDuelObjectSelected = new UnityEvent<DuelObjectType>();

    public Image Emblem;
    public TextMeshProUGUI TeamName;
    public TextMeshProUGUI TeamCount;
    public Animator TeamCountBgAnimator;

    public Image QuestionMark;
    public Image TeamNamePlaceHodler;

    public SettingsItemData m_ConfigData = new SettingsItemData();

    private bool m_IsInteractable = true;

    public void OnSelected()
    {
        transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .2f);
    }

    public void OnDeSelected()
    {
        transform.DOScale(new Vector3(.9f, .9f, .9f), .2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!m_IsInteractable)
            return;

        OnDuelObjectSelected?.Invoke(ID);
    }

    public void SetInteractability(bool enabled)
    {
        m_IsInteractable = enabled;
    }

    internal void OnSettingsChanged(SettingsItemData data, Action action)
    {
        if (data.TeamCount != 0)
        {
            m_ConfigData.TeamCount = data.TeamCount;
            TeamCountBgAnimator.Play("FadeIn");
            TeamCount.text = data.TeamCount.ToString();
        }

        if (data.TeamEmblem != null)
        {
            m_ConfigData.TeamEmblem = data.TeamEmblem;

            Emblem.sprite = data.TeamEmblem;

            if (QuestionMark.isActiveAndEnabled)
            {
                QuestionMark.enabled = false;
                Emblem.DOFade(1f, .4f);
            }
        }

        if (data.TeamName != "")
        {
            m_ConfigData.TeamName = data.TeamName;

            TeamName.text = data.TeamName;

            if (TeamNamePlaceHodler.color.a == 0f)
            {
                TeamNamePlaceHodler.DOFade(1f, .3f);
            }
        }

        action?.Invoke();
    }

    public bool IsDuelObjectReady()
    {
        if (m_ConfigData.TeamCount == 0) {
            return false;
        }
        else if (m_ConfigData.TeamEmblem == null) {
            return false;
        }
        else if (m_ConfigData.TeamName == "") {
            return false;
        } 
        else {
            return true;
        }
    }
}