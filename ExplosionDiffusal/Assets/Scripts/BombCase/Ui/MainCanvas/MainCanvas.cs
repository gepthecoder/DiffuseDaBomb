using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainCanvas : MonoBehaviour
{
    [Header("Teams")]
    [SerializeField] private MainTeamHolder TeamAxisPlaceholder;
    [SerializeField] private MainTeamHolder TeamAlliesPlaceholder;
    [Header("Countdown")]
    [SerializeField] private CountdownObject m_CountdownObject;
    [SerializeField] private Image m_TouchBlocker; // alpha to .65, raycast target on


    public void InitMainCanvas(DuelConfigData DATA)
    {
        gameObject.SetActive(true);

        TeamAxisPlaceholder.InitMainTeamHolder(new MainTeamHolderData(DATA.AxisConfigData.TeamEmblem, "0", DATA.AxisConfigData.TeamName));
        TeamAlliesPlaceholder.InitMainTeamHolder(new MainTeamHolderData(DATA.AlliesConfigData.TeamEmblem, "0", DATA.AlliesConfigData.TeamName));

        StartCoroutine(WaitAndExecuteTeams());
    }

    public void InitCountdown(float countdownTimeInSeconds)
    {
        m_TouchBlocker.raycastTarget = true;
        m_TouchBlocker.DOFade(.65F, .5f).OnComplete(() => {
            m_CountdownObject.transform.DOScale(1.1f, 1f);
            m_CountdownObject.transform.DOLocalMoveY(-120f, 1f).OnComplete(() => {
                m_CountdownObject.transform.DOScale(1f, .25f);
                m_CountdownObject.transform.DOLocalMoveY(-85f, .25f).SetEase(Ease.InOutBack).OnComplete(() => {
                    m_CountdownObject.StartCountdown(countdownTimeInSeconds);
                });
            });
        });
    }

    private IEnumerator WaitAndExecuteTeams()
    {
        yield return new WaitForSeconds(1);

        TeamAxisPlaceholder.DoDoScaleIn();
        TeamAlliesPlaceholder.DoDoScaleIn();
    }
}
