using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndMatchManager : MonoBehaviour
{
    [SerializeField] private Animator m_EndMatchAnime;
    [SerializeField] private TextMeshProUGUI m_WinnerText;

    public void InitEndMatch(VictoryEventData DATA)
    {
        m_WinnerText.text = string.Format(m_WinnerText.text, DATA._TeamName_);
        m_EndMatchAnime?.Play("END");
    }
}
