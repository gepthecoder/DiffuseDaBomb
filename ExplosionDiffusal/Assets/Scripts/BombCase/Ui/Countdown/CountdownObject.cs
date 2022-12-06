using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_CountdownTimerText;
    [SerializeField] private Image m_Fill;

    [SerializeField] private float m_TimeDuration;

    private float m_TimeRemaining;

    public void StartCountdown(float timeRemaining)
    {
        m_TimeRemaining = timeRemaining;
        StartCoroutine(CountDownAction());
    }

    private IEnumerator CountDownAction()
    {
        while(m_TimeRemaining >= 0)
        {
            m_CountdownTimerText.text = $"{m_TimeRemaining / 60:00} : {m_TimeRemaining % 60:00} : {m_TimeRemaining * 1000:00}";
            m_Fill.fillAmount = Mathf.InverseLerp(0, m_TimeDuration, m_TimeRemaining);
            m_TimeRemaining--;
            yield return new WaitForSeconds(1f);
        }
        OnCountDownCompleted();
        yield break;
    }

    private void OnCountDownCompleted()
    {
        print("End");
    }
}
