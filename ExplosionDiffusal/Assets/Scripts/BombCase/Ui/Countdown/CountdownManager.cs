using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;

// TODO: 
// -> countdown timer can be set in settings
// -> after the time runs out, hide the timer (shrinken to a visible position), touch blocker and go to initial game state
// -> nice to have: audio signalization that the match has started

public class CountdownManager : MonoBehaviour
{
    [Header("Countdown")]
    [SerializeField] private CountdownObject m_GameStartDelayCountdownObject;
    [SerializeField] private CountdownObject m_GameTimeCountdownObject;
    [SerializeField] private List<CountdownObject> m_DefuseTimeCountdownObjects;
    [SerializeField] private Image m_TouchBlocker; // alpha to .65, raycast target on

    [HideInInspector] public UnityEvent OnCountdownCompletedEvent = new UnityEvent(); // Game Start Countdown
    [HideInInspector] public UnityEvent<VictoryEventData> OnVictoryEvent = new UnityEvent<VictoryEventData>();

    private float m_CountdownTimeInSeconds_MatchStartDelay;
    private float m_CountdownTimeInMinutes_GameTime;
    private float m_CountdownTimeInMinutes_DefuseTime;

    private bool m_VictoryInitialized = false;

    private void Awake()
    {
        m_GameStartDelayCountdownObject.OnCountdownCompletedEvent.AddListener(() => {
            m_GameStartDelayCountdownObject.transform.DOScale(.9f, 1f);
            m_GameStartDelayCountdownObject.transform.DOLocalMoveY(370f, 1f).OnComplete(() => {
                m_TouchBlocker.DOFade(0f, 1f).OnComplete(() => {
                    m_TouchBlocker.raycastTarget = false;
                });
                m_GameStartDelayCountdownObject.transform.DOScale(.2f, 1.5f);
                m_GameStartDelayCountdownObject.transform.DOLocalMoveY(800f, 1f).SetEase(Ease.InExpo).OnComplete(() => {
                    m_GameStartDelayCountdownObject.Deinit();
                    OnCountdownCompletedEvent?.Invoke();

                    InitGameTimeCountdown(m_CountdownTimeInMinutes_GameTime * 60);
                });
            });
        });

        m_GameTimeCountdownObject.OnCountdownCompletedEvent.AddListener(() => {
            print("Game Time Ended!");
            // TODO: Emmit Victory State
            OnVictoryEvent?.Invoke(new VictoryEventData(Team.None, VictoryType.GameTimeEnded));
        });

        m_DefuseTimeCountdownObjects.ForEach((timer) => { 
            timer.OnCountdownCompletedEvent.AddListener(() => {
                if (m_VictoryInitialized)
                    return;

                m_VictoryInitialized = true;

                Debug.Log("Explodeeeee!");
                OnVictoryEvent?.Invoke(new VictoryEventData(Team.Axis, VictoryType.BombExploded));
            });
        });
    }

    private void OnDestroy()
    {
        m_GameStartDelayCountdownObject.OnCountdownCompletedEvent.RemoveAllListeners();
        m_GameTimeCountdownObject.OnCountdownCompletedEvent.RemoveAllListeners();
        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            timer.OnCountdownCompletedEvent.RemoveAllListeners();
        });
    }

    public void DeinitCountdownObjects()
    {
        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            timer.Deinit();
        });
    }

    public void ResetCountDownObjects()
    {
        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            timer.Default();
        });

        m_VictoryInitialized = false;
    }

    public void InitCountdown(MatchSettingsConfigData data)
    {
        m_CountdownTimeInSeconds_MatchStartDelay = data.MatchStartTimeInSeconds;
        m_CountdownTimeInMinutes_GameTime = data.GameTimeInMinutes;

        InitGameStartDelayCountdown(m_CountdownTimeInSeconds_MatchStartDelay);
    }

    private void InitGameStartDelayCountdown(float countdownTimeInSeconds)
    {
        m_TouchBlocker.raycastTarget = true;
        m_GameStartDelayCountdownObject.SetInitialCountDownTime(countdownTimeInSeconds);

        m_TouchBlocker.DOFade(.65f, .77f).OnComplete(() =>
        {
            m_GameStartDelayCountdownObject.transform.DOScale(.9f, .8f);
            m_GameStartDelayCountdownObject.transform.DOLocalMoveY(370f, 1f).OnComplete(() =>
            {
                m_GameStartDelayCountdownObject.transform.DOScale(.85f, .5f);
                m_GameStartDelayCountdownObject.transform.DOLocalMoveY(460f, .5f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    m_GameStartDelayCountdownObject.StartCountdown(countdownTimeInSeconds);
                });
            });
        });
    }

    private void InitGameTimeCountdown(float countdownTimeInSeconds)
    {
        m_GameTimeCountdownObject.SetInitialCountDownTime(countdownTimeInSeconds);

        m_GameTimeCountdownObject.transform.DOScale(1.1f, .77f).OnComplete(() => {
            m_GameTimeCountdownObject.transform.DOScale(1f, .25f).OnComplete(() => {
                m_GameTimeCountdownObject.StartCountdown(countdownTimeInSeconds);
            });
        });
    }

    public void InitDefuseBombTime(float countdownTimeInMinutes, List<CountdownObjectType> types)
    {
        m_CountdownTimeInMinutes_DefuseTime = countdownTimeInMinutes;

        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            types.ForEach((type) => {
                if (timer.Type == type)
                {
                    timer.StartCountdown(countdownTimeInMinutes * 60);
                }
            });
        });
    }

    public void SetDefuseBombTimeText(float countdownTimeInMinutes, CountdownObjectType type, bool forceClear = false)
    {
        m_CountdownTimeInMinutes_DefuseTime = countdownTimeInMinutes;

        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            if (timer.Type == type)
            {
                if(forceClear)
                {
                    timer.DeinitTimer();
                }
                else {
                    timer.SetInitialCountDownTime(countdownTimeInMinutes * 60);
                }
            }
        });
    }

    public void SyncDefuseBombTime(CountdownObjectType syncFrom, CountdownObjectType syncTo)
    {
        float timeRemaining = -1f;
        CountdownObject syncObject = new CountdownObject();

        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            if (timer.Type == syncFrom)
            {
                timeRemaining = timer.GetTimeRemaining();
            }else if(timer.Type == syncTo) { syncObject = timer; }
        });

        if(timeRemaining != -1)
        {
            if(syncObject)
            {
                syncObject.StartCountdown(timeRemaining);
            }
        }
    }
}
