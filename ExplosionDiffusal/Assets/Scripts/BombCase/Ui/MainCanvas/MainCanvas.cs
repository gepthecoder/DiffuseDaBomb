using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainCanvas : MonoBehaviour
{
    [Header("Teams")]
    [SerializeField] private MainTeamHolder TeamAxisPlaceholder;
    [SerializeField] private MainTeamHolder TeamAlliesPlaceholder;

    public void InitMainCanvas(DuelConfigData DUEL_DATA, MatchSettingsConfigData MATCH_DATA)
    {
        gameObject.SetActive(true);
        TeamAxisPlaceholder.InitMainTeamHolder(new MainTeamHolderData(
            DUEL_DATA.AxisConfigData.TeamEmblem, "0", DUEL_DATA.AxisConfigData.TeamName, DUEL_DATA.AxisConfigData.TeamCount.ToString(), MATCH_DATA.ScoreLimit.ToString()));
        TeamAlliesPlaceholder.InitMainTeamHolder(new MainTeamHolderData(
            DUEL_DATA.AlliesConfigData.TeamEmblem, "0", DUEL_DATA.AlliesConfigData.TeamName, DUEL_DATA.AlliesConfigData.TeamCount.ToString(), MATCH_DATA.ScoreLimit.ToString()));
    }

    public MainTeamHolder GetAxisTeamHolder()
    {
        return TeamAxisPlaceholder;
    }

    public MainTeamHolder GetAlliesTeamHolder()
    {
        return TeamAlliesPlaceholder;
    }
}
