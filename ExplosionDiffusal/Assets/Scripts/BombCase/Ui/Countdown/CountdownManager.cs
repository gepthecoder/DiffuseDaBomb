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
    [SerializeField] private CountdownObject m_RoundTimeCountdownObject;
    [SerializeField] private List<CountdownObject> m_DefuseTimeCountdownObjects;
    [SerializeField] private Image m_TouchBlocker; // alpha to .65, raycast target on

    [HideInInspector] public UnityEvent OnCountdownCompletedEvent = new UnityEvent(); // Game Start Countdown
    [HideInInspector] public UnityEvent<VictoryEventData> OnVictoryEvent = new UnityEvent<VictoryEventData>();

    private float m_CountdownTimeInSeconds_MatchStartDelay;
    private float m_CountdownTimeInMinutes_GameTime;
    private float m_CountdownTimeInMinutes_DefuseTime;
    private float m_CountDownTimeInMinutes_RoundTime; 

    private int m_NumberOfRounds;

    private bool m_VictoryInitialized = false;

    private bool m_Last60SecPlayed = false;
    private bool m_Last48SecPlayed = false;
    private bool m_Last16SecPlayed = false;

    private bool m_HasPlayedGameIntroSound = false;

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

                    InitRoundTimeCountdown();
                });
            });
        });

        m_GameStartDelayCountdownObject.OnCountdownAlmostCompletedEvent.AddListener(() => {
            if (m_HasPlayedGameIntroSound)
                return;

            AudioManager.INSTANCE.PlayGameIntroAudio();
            m_HasPlayedGameIntroSound = true;
        });

        m_RoundTimeCountdownObject.OnCountdownCompletedEvent.AddListener(() => {
            print("Round Time Ended!");
            Team team = RoundManager.instance.GetWinningTeamByVictoryType(VictoryType.RoundTimeEnded);
            OnVictoryEvent?.Invoke(new VictoryEventData(team, VictoryType.RoundTimeEnded));
        });

        m_DefuseTimeCountdownObjects.ForEach((timer) => { 
            timer.OnCountdownCompletedEvent.AddListener(() => {
                if (m_VictoryInitialized)
                    return;

                m_VictoryInitialized = true;

                AudioManager.INSTANCE.MuteAllBombCountdownLoops();

                Debug.Log("Explodeeeee!");
                Team team = RoundManager.instance.GetWinningTeamByVictoryType(VictoryType.BombExploded);
                OnVictoryEvent?.Invoke(new VictoryEventData(team, VictoryType.BombExploded));
            });
        });
    }

    private void Start()
    {
        m_RoundTimeCountdownObject.OnLast60SecLeftEvent.AddListener(() => {
            if (m_Last60SecPlayed)
                return;
            
            m_Last60SecPlayed = true;

            AudioManager.INSTANCE.Last60Sec();
        });

        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            timer.OnLast60SecLeftEvent.AddListener(() => {
                if (m_Last60SecPlayed)
                    return;
                m_Last60SecPlayed = true;

                AudioManager.INSTANCE.Last60Sec();
            });
            timer.OnLast48SecLeftEvent.AddListener(() => {
                if (m_Last48SecPlayed)
                    return;
                m_Last48SecPlayed = true;

                AudioManager.INSTANCE.Last48Sec();
            });
            timer.OnLast16SecLeftEvent.AddListener(() => {
                if (m_Last16SecPlayed)
                    return;
                m_Last16SecPlayed = true;

                AudioManager.INSTANCE.Last16Sec();
            });
        });
    }

    private void OnDestroy()
    {
        m_GameStartDelayCountdownObject.OnCountdownCompletedEvent.RemoveAllListeners();
        m_RoundTimeCountdownObject.OnCountdownCompletedEvent.RemoveAllListeners();
        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            timer.OnCountdownCompletedEvent.RemoveAllListeners();
            timer.OnLast60SecLeftEvent.RemoveAllListeners();
            timer.OnLast48SecLeftEvent.RemoveAllListeners();
            timer.OnLast16SecLeftEvent.RemoveAllListeners();
        });

        m_RoundTimeCountdownObject.OnLast60SecLeftEvent.RemoveAllListeners();
        m_GameStartDelayCountdownObject.OnCountdownAlmostCompletedEvent.RemoveAllListeners();
    }

    public void DeinitCountdownObjects()
    {
        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            if(timer.Type != CountdownObjectType.BombCaseMagnetic3D)
                timer.Deinit();
        });
    }

    public void ResetCountDownObjects()
    {
        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            timer.Default();
        });

        m_VictoryInitialized = false;

        m_Last60SecPlayed = false;
        m_Last48SecPlayed = false;
        m_Last16SecPlayed = false;
    }

    public void InitCountdown(MatchSettingsConfigData data)
    {
        m_CountdownTimeInSeconds_MatchStartDelay = data.MatchStartTimeInSeconds;
        m_CountdownTimeInMinutes_GameTime = data.GameTimeInMinutes;
        m_NumberOfRounds = data.ScoreLimit;

        m_CountDownTimeInMinutes_RoundTime = (int)(m_CountdownTimeInMinutes_GameTime / m_NumberOfRounds);

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
                    AudioManager.INSTANCE.FadeOutVolume(MenuAudioLoopType.Loop4, m_CountdownTimeInSeconds_MatchStartDelay);
                    m_GameStartDelayCountdownObject.StartCountdown(countdownTimeInSeconds);
                });
            });
        });
    }

    // MAIN COUNTDOWN
    public void InitRoundTimeCountdown()
    {
        float countdownTimeInSeconds = m_CountDownTimeInMinutes_RoundTime * 60;

        m_RoundTimeCountdownObject.SetInitialCountDownTime(countdownTimeInSeconds);
        m_RoundTimeCountdownObject.TryUpdateRoundNumber(RoundManager.instance.GetCurrentRound(), m_NumberOfRounds);

        m_RoundTimeCountdownObject.transform.DOScale(1.1f, .77f).OnComplete(() => {
            m_RoundTimeCountdownObject.transform.DOScale(1f, .25f).OnComplete(() => {
                m_RoundTimeCountdownObject.StartCountdown(countdownTimeInSeconds);
            });
        });
    }
    public void DeinitRoundTimeCountdown()
    {
        m_RoundTimeCountdownObject.transform.DOScale(1.1f, .5f).OnComplete(() => {
            m_RoundTimeCountdownObject.transform.DOScale(0f, 1f).OnComplete(() => {
                // RESET
                DeinitMainBombTimer();
            });
        });
    }

    public void InitMainBombTimer(float bombTime)
    {
        m_RoundTimeCountdownObject.InitMainBombTimer(bombTime);
    }

    public void DeinitMainBombTimer()
    {
        m_RoundTimeCountdownObject.DeinitMainTimerAlaDefault();
    }

    //

    public void InitDefuseBombTime(float countdownTimeInMinutes, List<CountdownObjectType> types)
    {
        m_CountdownTimeInMinutes_DefuseTime = countdownTimeInMinutes;

        InitMainBombTimer(countdownTimeInMinutes * 60);

        m_DefuseTimeCountdownObjects.ForEach((timer) => {
            types.ForEach((type) => {
                if (timer.Type == type)
                {
                    timer.StartCountdown(countdownTimeInMinutes * 60);
                }
            });
        });

    }

    public void DisableBombTimerOnDefuseEvent()
    {
        m_RoundTimeCountdownObject.DisableBombTimerOnDefuseEvent();
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
            if(timer.Type == syncTo) { syncObject = timer; }
        });

        timeRemaining = m_RoundTimeCountdownObject.GetTimeRemaining();

        if (timeRemaining != -1)
        {
            if(syncObject)
            {
                syncObject.StartCountdown(timeRemaining);
            }
        }
    }
}
