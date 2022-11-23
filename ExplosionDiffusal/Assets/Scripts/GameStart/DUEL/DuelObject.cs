using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;
using TMPro;

public enum DuelObjectType { Attacker, Defender, None }

public class DuelObject : MonoBehaviour, IPointerClickHandler
{
    public DuelObjectType ID;
    [HideInInspector] public UnityEvent<DuelObjectType> OnDuelObjectSelected = new UnityEvent<DuelObjectType>();

    public Image Emblem;
    public TextMeshProUGUI TeamName;
    public TextMeshProUGUI TeamCount;

    public Image QuestionMark;

    private SettingsItemData m_ConfigData = new SettingsItemData();

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
        OnDuelObjectSelected?.Invoke(ID);
    }

    internal void OnSettingsChanged(SettingsItemData data)
    {
        DrawDataAndSaveConfig(data);
    }

    private void DrawDataAndSaveConfig(SettingsItemData data)
    {
        if (QuestionMark.isActiveAndEnabled) { QuestionMark.enabled = false; }

        if (data.TeamCount != 0)
        {
            m_ConfigData.TeamCount = data.TeamCount;

            TeamCount.text = data.TeamCount.ToString();

            Debug.Log($"DrawDataAndSaveConfig: Team Count: {data.TeamCount}");
        }

        if (data.TeamEmblem != null)
        {
            m_ConfigData.TeamEmblem = data.TeamEmblem;

            Emblem.sprite = data.TeamEmblem;

            Debug.Log($"DrawDataAndSaveConfig: Team Emblem: {data.TeamEmblem.name}");
        }

        if (data.TeamName != "")
        {
            m_ConfigData.TeamName = data.TeamName;

            TeamName.text = data.TeamName;

            Debug.Log($"DrawDataAndSaveConfig: Team Name: {data.TeamName}");
        }
    }
}
