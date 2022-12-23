using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class VictoryUiManager : MonoBehaviour
{
    [SerializeField] private Animator m_VictoryVFX;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI m_AxisAlliesWinText;
    [SerializeField] private TextMeshProUGUI m_TeamNameWinText;

    [HideInInspector] public UnityEvent OnVictoryShownEvent = new UnityEvent();

    public void InitVictoryUi(VictoryEventData DATA)
    {
        SetupUiData(DATA);

        m_VictoryVFX.Play("W");

        StartCoroutine(AwaitAndEmmitVictoryShown());
    }

    private void SetupUiData(VictoryEventData DATA)
    {
        m_AxisAlliesWinText.text = $"{DATA._WinningTeam_} WIN";
        m_TeamNameWinText.text = $"{DATA._TeamName_}";
    }

    private IEnumerator AwaitAndEmmitVictoryShown()
    {
        yield return new WaitForSeconds(6f); // depends on the W animation lenght
        OnVictoryShownEvent?.Invoke();
    }
}
