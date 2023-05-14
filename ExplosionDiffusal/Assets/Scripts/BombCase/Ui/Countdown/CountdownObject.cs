using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;
using DG.Tweening;

public enum CountdownObjectType { Other2D, CircuitTimer3D, BombCaseTimer3D, MagneticBombTimer3D, BombCaseMagnetic3D }

public class CountdownObject : MonoBehaviour
{
    public CountdownObjectType Type;

    [SerializeField] private TextMeshProUGUI m_CountdownTimerText;
    [SerializeField] private TextMeshPro m_CountdownTimerText_3D;
    [SerializeField] private Image m_Fill;
    [SerializeField] private TextMeshProUGUI m_RoundNumberText;
    [SerializeField] private TextMeshProUGUI m_BombNumberText;

    [HideInInspector] public UnityEvent OnCountdownCompletedEvent = new UnityEvent();

    private TimeSpan m_TimePlaying;
    private bool m_TimerRunning;

    private float m_TimeRemaining;
    private float m_InitialTime;

    // Round & Bomb Time Positions

    private const float m_DefaultY = -20;
    private const float m_HideY = -200;

    private void Start()
    {
        m_TimerRunning = false;
    }

    public void SetInitialCountDownTime(float timeRemaining)
    {
        TimeSpan ts = TimeSpan.FromSeconds(timeRemaining);
        if (m_CountdownTimerText_3D)
        {
            m_CountdownTimerText_3D.text = ts.ToString("mm':'ss'.'ff");
        }
        if(m_CountdownTimerText)
        {
            m_CountdownTimerText.text = ts.ToString("mm':'ss'.'ff");
        }
    }

    public void DeinitTimer()
    {
        m_TimeRemaining = 0;

        if (m_CountdownTimerText_3D)
        {
            m_CountdownTimerText_3D.text = "XX:XX.XX";
        }
        if (m_CountdownTimerText)
        {
            m_CountdownTimerText.text = "XX:XX.XX";
        }
        if(m_BombNumberText)
        {
            m_BombNumberText.text = "XX:XX.XX";
        }

        StopAllCoroutines();
    }

    public void StartCountdown(float timeRemaining, bool isBombTimer = false)
    {
        m_TimeRemaining = timeRemaining;
        m_InitialTime = m_TimeRemaining;

        m_TimerRunning = true;
        StartCoroutine(CountDownAction(isBombTimer));
    }

    public float GetTimeRemaining()
    {
        return m_TimeRemaining;
    }

    private IEnumerator CountDownAction(bool isMainBombTimer)
    {
        while(m_TimerRunning)
        {
            m_TimeRemaining -= Time.deltaTime;

            if (m_Fill)
            {
                m_Fill.fillAmount = Mathf.InverseLerp(0, m_InitialTime, m_TimeRemaining);
            }
            m_TimePlaying = TimeSpan.FromSeconds(m_TimeRemaining);

            if(isMainBombTimer)
            {
                if(m_BombNumberText)
                {
                    m_BombNumberText.text = m_TimePlaying.ToString("mm':'ss'.'ff");
                }

            }else
            {
                if (m_CountdownTimerText)
                {
                    m_CountdownTimerText.text = m_TimePlaying.ToString("mm':'ss'.'ff");
                }

                if (m_CountdownTimerText_3D)
                {
                    m_CountdownTimerText_3D.text = m_TimePlaying.ToString("mm':'ss'.'ff");
                }
            }
          

            if (m_TimeRemaining <= 0) {

                m_TimeRemaining = 0;

                if (isMainBombTimer)
                {
                    m_BombNumberText.text = "XX:XX.XX";
                }
                else
                {

                    if (m_CountdownTimerText)
                        m_CountdownTimerText.text = "00:00.00";
                    if (m_CountdownTimerText_3D)
                        m_CountdownTimerText_3D.text = "00:00.00";
                }

                m_TimerRunning = false;

                if(!isMainBombTimer)
                    OnCountdownCompletedEvent?.Invoke();
            }

            yield return null;
        }

        yield return null;
    }

    public void Deinit()
    {
        gameObject.SetActive(false);
    }

    public void Default()
    {
        gameObject.SetActive(true);
        
        if(m_CountdownTimerText)
            m_CountdownTimerText.text = "";

        if (m_CountdownTimerText_3D)
            m_CountdownTimerText_3D.text = "";
    }

    public void TryUpdateRoundNumber(int currentRound, int totalRounds)
    {
        if(m_RoundNumberText != null)
        {
            m_RoundNumberText.text = 
                string.Format("<size=40><u>Round</u>\n</size><size=77>{0}</size><size=40>/</size><size=77>{1}</size>", 
                currentRound, totalRounds);
        }
    }

    public void InitMainBombTimer(float bombTime)
    {
        // deinit ala stop the round time timer
        DeinitTimer();

        // start bomb timer
        StartCountdown(bombTime, true);

        StartCoroutine(TakeOverSeq());
    }

    private IEnumerator TakeOverSeq()
    {
        // move rtimer down the axis while showing btimer
        m_CountdownTimerText.transform.DOLocalMoveY(m_DefaultY + 20f, .77f).SetEase(Ease.InExpo).OnComplete(() => {
            m_CountdownTimerText.transform.DOScale(0f, 2f);
            m_CountdownTimerText.transform.DOLocalMoveY(m_HideY, 1.5f).SetEase(Ease.InOutBack);
        });

        yield return new WaitForSeconds(1.2f);

        m_BombNumberText.transform.DOScale(1f, 1f);
        m_BombNumberText.transform.DOLocalMoveY(m_DefaultY + 20f, 1f).SetEase(Ease.InExpo).OnComplete(() => {
            m_BombNumberText.transform.DOLocalMoveY(m_DefaultY, .5f).SetEase(Ease.InOutBack);
            m_BombNumberText.color = Color.red;
        });
    }

    public void DeinitMainTimerAlaDefault()
    {
        m_CountdownTimerText.transform.localPosition = new Vector2(
            m_CountdownTimerText.transform.localPosition.x,
            m_DefaultY
        );
        m_CountdownTimerText.transform.localScale = Vector3.one;

        m_BombNumberText.transform.localPosition = new Vector2(
             m_BombNumberText.transform.localPosition.x,
             m_HideY
         );
        m_BombNumberText.transform.localScale = Vector3.zero;

        m_BombNumberText.color = Color.white;
    }

    public void DisableBombTimerOnDefuseEvent()
    {
        if(m_BombNumberText != null)
        {
            StopAllCoroutines();
            m_BombNumberText.text = "XX:XX.XX";
        } 
    }
}
