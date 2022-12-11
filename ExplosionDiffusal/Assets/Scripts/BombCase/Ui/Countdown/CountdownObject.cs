using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class CountdownObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_CountdownTimerText;
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

    public void StartCountdown(float timeRemaining)
    {
        m_TimeRemaining = timeRemaining;
        m_InitialTime = m_TimeRemaining;

        m_TimerRunning = true;

        StartCoroutine(CountDownAction());
    }

    private IEnumerator CountDownAction()
    {
        while(m_TimerRunning)
        {
            m_TimeRemaining -= Time.deltaTime;
            m_Fill.fillAmount = Mathf.InverseLerp(0, m_InitialTime, m_TimeRemaining);
            m_TimePlaying = TimeSpan.FromSeconds(m_TimeRemaining);
            m_CountdownTimerText.text = m_TimePlaying.ToString("mm':'ss'.'ff");

            if(m_TimeRemaining <= 0) { 
                m_TimerRunning = false;
                OnCountdownCompletedEvent?.Invoke();
            }

            yield return null;
        }

        yield return null;
    }

    internal void Deinit()
    {
        gameObject.SetActive(false);
    }
}
