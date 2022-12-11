using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
// TODO: 
// -> countdown timer can be set in settings
// -> after the time runs out, hide the timer (shrinken to a visible position), touch blocker and go to initial game state
// -> nice to have: audio signalization that the match has started

public class CountdownManager : MonoBehaviour
{
    [Header("Countdown")]
    [SerializeField] private CountdownObject m_CountdownObject;
    [SerializeField] private Image m_TouchBlocker; // alpha to .65, raycast target on

    [HideInInspector] public UnityEvent OnCountdownCompletedEvent = new UnityEvent(); 

    private void Awake()
    {
        m_CountdownObject.OnCountdownCompletedEvent.AddListener(() => {
            m_CountdownObject.transform.DOScale(1.077f, 1f);
            m_CountdownObject.transform.DOLocalMoveY(370f, 1f).OnComplete(() => {
                m_TouchBlocker.DOFade(0f, 1f).OnComplete(() => {
                    m_TouchBlocker.raycastTarget = false;
                });
                m_CountdownObject.transform.DOScale(.5f, 1.5f);
                m_CountdownObject.transform.DOLocalMoveY(800f, 1f).SetEase(Ease.InExpo).OnComplete(() => {
                    m_CountdownObject.Deinit();
                    OnCountdownCompletedEvent?.Invoke();
                });
            });
        });
    }

    public void InitCountdown(float countdownTimeInSeconds)
    {
        m_TouchBlocker.raycastTarget = true;
        m_TouchBlocker.DOFade(.65f, .77f).OnComplete(() =>
        {
            m_CountdownObject.transform.DOScale(1.1f, 1f);
            m_CountdownObject.transform.DOLocalMoveY(370f, 1f).OnComplete(() =>
            {
                m_CountdownObject.transform.DOScale(1f, .5f);
                m_CountdownObject.transform.DOLocalMoveY(450f, .5f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    m_CountdownObject.StartCountdown(countdownTimeInSeconds);
                });
            });
        });
    }
}
