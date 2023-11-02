using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text = null;

    public void SetTeamNameText(string tName)
    {
        if(m_Text)
        {
            m_Text.text = tName;
        }
    }

    public string GetTeamName()
    {
        if (m_Text)
        {
            return m_Text.text;
        }

        return string.Empty;
    }
}
