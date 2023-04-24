using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public enum CountdownObjectType { Other2D, CircuitTimer3D, BombCaseTimer3D, MagneticBombTimer3D, BombCaseMagnetic3D }

public class CountdownObject : MonoBehaviour
{
    public CountdownObjectType Type;

    [SerializeField] private TextMeshProUGUI m_CountdownTimerText;
    [SerializeField] private TextMeshPro m_CountdownTimerText_3D;
    [SerializeField] private Image m_Fill;

    [HideInInspector] public UnityEvent OnCountdownCompletedEvent = new UnityEvent();

    private TimeSpan m_TimePlaying;
    private bool m_TimerRunning;

    private float m_TimeRemaining;
    private float m_InitialTime;

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

        StopAllCoroutines();
    }

    public void StartCountdown(float timeRemaining)
    {
        m_TimeRemaining = timeRemaining;
        m_InitialTime = m_TimeRemaining;

        m_TimerRunning = true;
        StartCoroutine(CountDownAction());
    }

    public float GetTimeRemaining()
    {
        return m_TimeRemaining;
    }

    private IEnumerator CountDownAction()
    {
        while(m_TimerRunning)
        {
            m_TimeRemaining -= Time.deltaTime;

            if (m_Fill)
            {
                m_Fill.fillAmount = Mathf.InverseLerp(0, m_InitialTime, m_TimeRemaining);
            }
            m_TimePlaying = TimeSpan.FromSeconds(m_TimeRemaining);

            if(m_CountdownTimerText)
            {
                m_CountdownTimerText.text = m_TimePlaying.ToString("mm':'ss'.'ff");
            }

            if(m_CountdownTimerText_3D)
            {
                m_CountdownTimerText_3D.text = m_TimePlaying.ToString("mm':'ss'.'ff");
            }

            if (m_TimeRemaining <= 0) {

                m_TimeRemaining = 0;

                if(m_CountdownTimerText)
                    m_CountdownTimerText.text = "00:00.00";
                if (m_CountdownTimerText_3D)
                    m_CountdownTimerText_3D.text = "00:00.00";

                m_TimerRunning = false;
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
}
