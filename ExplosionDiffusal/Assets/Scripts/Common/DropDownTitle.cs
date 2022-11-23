using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class DropDownTitle : MonoBehaviour, ISelectHandler
{
    [SerializeField] TMP_Dropdown.OptionData title;

    private bool m_WasNvrSelected = true;

    private TMP_Dropdown m_DropDown;

    private void Start()
    {
        m_DropDown = GetComponent<TMP_Dropdown>();
        InitTitleOnDropDown();
    }

    private void InitTitleOnDropDown()
    {
        m_DropDown.options.Insert(m_DropDown.value, title);
        m_DropDown.RefreshShownValue();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(m_WasNvrSelected) { RemoveTitle(); m_WasNvrSelected = false; }
    }

    private void RemoveTitle()
    {
        m_DropDown.options.RemoveAt(m_DropDown.value);
        m_DropDown.onValueChanged.Invoke(m_DropDown.value);
        m_DropDown.RefreshShownValue();
    }
}
