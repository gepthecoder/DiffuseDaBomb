using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainCanvas : MonoBehaviour
{
    [Header("Teams")]
    [SerializeField] private MainTeamHolder TeamAxisPlaceholder;
    [SerializeField] private MainTeamHolder TeamAlliesPlaceholder;

    public void InitMainCanvas(DuelConfigData DATA)
    {
        gameObject.SetActive(true);

        TeamAxisPlaceholder.InitMainTeamHolder(new MainTeamHolderData(DATA.AxisConfigData.TeamEmblem, "0", DATA.AxisConfigData.TeamName));
        TeamAlliesPlaceholder.InitMainTeamHolder(new MainTeamHolderData(DATA.AlliesConfigData.TeamEmblem, "0", DATA.AlliesConfigData.TeamName));

        StartCoroutine(WaitAndExecuteTeams());
    }

    private IEnumerator WaitAndExecuteTeams()
    {
        yield return new WaitForSeconds(1);

        TeamAxisPlaceholder.DoDoScaleIn();
        TeamAlliesPlaceholder.DoDoScaleIn();
    }
}
