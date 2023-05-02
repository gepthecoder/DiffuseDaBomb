using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System;

public class VictoryUiManager : MonoBehaviour
{
    [Header("Main Victory")]
    [SerializeField] private Animator m_VictoryVFX;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI m_AxisAlliesWinText;
    [SerializeField] private TextMeshProUGUI m_TeamNameWinText;
    [Header("Bomb Defused")]
    [SerializeField] private Animator m_BombDefusedVFX;
    [Header("Round Time Limit Reached")]
    [SerializeField] private Animator m_RoundTimeLimitVFX;


    [HideInInspector] public UnityEvent<VictoryEventData> OnVictoryShownEvent = new UnityEvent<VictoryEventData>();

    public void InitVictoryUi(VictoryEventData DATA)
    {
        SetupUiData(DATA);

        m_VictoryVFX.Play("W");

        StartCoroutine(AwaitAndEmmitVictoryShown(DATA));
    }

    private void SetupUiData(VictoryEventData DATA)
    {
        m_AxisAlliesWinText.text = $"{DATA._WinningTeam_} WIN";
        m_TeamNameWinText.text = $"{DATA._TeamName_}";
    }

    private IEnumerator AwaitAndEmmitVictoryShown(VictoryEventData DATA)
    {
        yield return new WaitForSeconds(6f); // depends on the W animation lenght
        OnVictoryShownEvent?.Invoke(DATA);
    }

    public void PlayBombDefusedAnime()
    {
        m_BombDefusedVFX.Play("POPUP");
    }

    public void PlayRoundTimeLimitReachedAnime()
    {
        m_RoundTimeLimitVFX.Play("POPUP");
    }
}
