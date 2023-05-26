using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EndMatchObjectData
{
    public Sprite m_WinningTeamSprite;
    public Sprite m_LosingTeamSprite;

    public string m_WinningTeamNameString;
    public string m_LosingTeamNameString;

    public int m_WinningTeamScore;
    public int m_LosingTeamScore;

    public string m_EndMatchDateString;
    public string m_EndMatchTimeString;

    public SettingsItemData m_SettingsItemDataWinner;
    public SettingsItemData m_SettingsItemDataLoser;

    public bool m_IsDraw;

    public EndMatchObjectData() { }

    public EndMatchObjectData
        (Sprite wT, Sprite lT, string wTName, string lTName, int wTScore, int lTScore, string eMDate, string eMTime, SettingsItemData sItemDataWinner, SettingsItemData sItemDataLoser, bool isDraw)
    {
        m_WinningTeamSprite = wT; m_LosingTeamSprite = lT;
        m_WinningTeamNameString = wTName; m_LosingTeamNameString = lTName;
        m_WinningTeamScore = wTScore; m_LosingTeamScore = lTScore;
        m_EndMatchDateString = eMDate; m_EndMatchTimeString = eMTime;
        m_SettingsItemDataWinner = sItemDataWinner; m_SettingsItemDataLoser = sItemDataLoser;
        m_IsDraw = isDraw;
    }
}

[System.Serializable]
public class EndMatchTeamStatusIcon
{
    [SerializeField] private GameObject m_WinnerVictoryImage;
    [SerializeField] private GameObject m_WinnerDrawImage;
    [Space(5)]
    [SerializeField] private GameObject m_LoserVictoryImage;
    [SerializeField] private GameObject m_LoserDrawImage;

    public void EnableStatusIcons(bool isDraw)
    {
        if(isDraw)
        {
            m_WinnerVictoryImage.SetActive(false);
            m_WinnerDrawImage.SetActive(true);

            m_LoserVictoryImage.SetActive(false);
            m_LoserDrawImage.SetActive(true);
        } else
        {
            m_WinnerVictoryImage.SetActive(true);
            m_WinnerDrawImage.SetActive(false);

            m_LoserVictoryImage.SetActive(true);
            m_LoserDrawImage.SetActive(false);
        }
    }
}

public class EndMatchObject : MonoBehaviour
{
    [Header("Duel")]
    [SerializeField] private Image m_WinningTeamImage;
    [SerializeField] private Image m_LosingTeamImage;
    [SerializeField] private EndMatchTeamStatusIcon m_EndMatchTeamStatusIcon;
    [Header("Match Info")]
    [SerializeField] private TextMeshProUGUI m_WinningTeamNameText;
    [SerializeField] private TextMeshProUGUI m_LosingTeamNameText;
    [SerializeField] private TextMeshProUGUI m_WinningTeamScoreText;
    [SerializeField] private TextMeshProUGUI m_LosingTeamScoreText;
    [Header("Time Info")]
    [SerializeField] private TextMeshProUGUI m_EndMatchDateText; // format: 12/05/2023
    [SerializeField] private TextMeshProUGUI m_EndMatchTimeText; // format: 16:20
    [Header("History Index")]
    [SerializeField] private TextMeshProUGUI m_HistoryIndexText; 


    public void SetEndMatchObjectData(EndMatchObjectData data, int index = -1)
    {
        if (data == null)
            return;

        m_WinningTeamImage.sprite = data.m_WinningTeamSprite;
        m_LosingTeamImage.sprite = data.m_LosingTeamSprite;

        m_EndMatchTeamStatusIcon.EnableStatusIcons(data.m_IsDraw);

        m_WinningTeamNameText.text = data.m_WinningTeamNameString;
        m_LosingTeamNameText.text = data.m_LosingTeamNameString;

        m_WinningTeamScoreText.text = $"{data.m_WinningTeamScore}";
        m_LosingTeamScoreText.text = $"{data.m_LosingTeamScore}";

        m_EndMatchDateText.text = $"{data.m_EndMatchDateString}";
        m_EndMatchTimeText.text = $"{data.m_EndMatchTimeString}";

        if(index != -1 && m_HistoryIndexText != null) {
            m_HistoryIndexText.text = $"{index}";
        }
    }
}
