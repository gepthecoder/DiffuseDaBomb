using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HistoryItemData
{
    public string WinningTeamEmblemNameString;
    public string LosingTeamEmblemNameString;

    public string WinningTeamNameString;
    public string LosingTeamNameString;

    public int WinningTeamScore;
    public int LosingTeamScore;

    public string EndMatchDateString;
    public string EndMatchTimeString;

    public HistoryItemData() { }
    public HistoryItemData(string Wemblem, string Lemblem, string WteamName, string LteamName, int Wscore, int Lscore, string endDate, string endTime)
    {

        this.WinningTeamEmblemNameString = Wemblem;
        this.LosingTeamEmblemNameString = Lemblem;
        this.WinningTeamNameString = WteamName;
        this.LosingTeamNameString = LteamName;
        this.WinningTeamScore = Wscore;
        this.LosingTeamScore = Lscore;
        this.EndMatchDateString = endDate;
        this.EndMatchTimeString = endTime;
    }
}

public class HistoryItemObject : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private TextMeshProUGUI m_IndexText;
    [Header("Emblem")]
    [SerializeField] private List<Sprite> m_AllSprites;
    [SerializeField] private Image m_WinningTeamImage;
    [SerializeField] private Image m_LosingTeamImage;
    [Header("Team Name")]
    [SerializeField] private TextMeshProUGUI m_WinningTeamNameText;
    [SerializeField] private TextMeshProUGUI m_LosingTeamNameText;
    [Header("Team Score")]
    [SerializeField] private TextMeshProUGUI m_WinningTeamScoreText;
    [SerializeField] private TextMeshProUGUI m_LosingTeamScoreText;
    [Header("End Match")]
    [SerializeField] private TextMeshProUGUI m_EndMatchDateText;
    [SerializeField] private TextMeshProUGUI m_EndMatchTimeText;
    [Header("Status")]
    [SerializeField] private EndMatchTeamStatusIcon m_EndMatchTeamStatusIcon;

    public void SetHistoryItemObjectData(HistoryItemData data, int index)
    {
        if (data == null)
            return;

        m_IndexText.text = $"{index}";

        m_WinningTeamImage.sprite = GetSpriteByName(data.WinningTeamEmblemNameString);
        m_LosingTeamImage.sprite = GetSpriteByName(data.LosingTeamEmblemNameString);

        m_WinningTeamNameText.text = data.WinningTeamNameString;
        m_LosingTeamNameText.text = data.LosingTeamNameString;

        m_WinningTeamScoreText.text = $"{data.WinningTeamScore}";
        m_LosingTeamScoreText.text = $"{data.LosingTeamScore}";

        m_EndMatchDateText.text = $"{data.EndMatchDateString}";
        m_EndMatchTimeText.text = $"{data.EndMatchTimeString}";

        bool isDraw = data.WinningTeamScore == data.LosingTeamScore;
        m_EndMatchTeamStatusIcon.EnableStatusIcons(isDraw);
    }

    private Sprite GetSpriteByName(string name) {
        return m_AllSprites.Where(x => x.name == name).FirstOrDefault();
    }
}
