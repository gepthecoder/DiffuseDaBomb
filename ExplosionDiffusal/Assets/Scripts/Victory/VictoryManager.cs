using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;

public enum Team { Axis, Allies, None }
public enum VictoryType { BombExploded, BombDefused, RoundTimeEnded, }

public class VictoryEventData
{
    public BombCaseState _BombState_;
    public Team _WinningTeam_;
    public VictoryType _VictoryType_;
    public string _TeamName_;
    public bool _ScoreLimitReached_;
    public Team _ScoreLimitReachedByTeam_;
    public bool _IsDraw_;

    public VictoryEventData(Team team, VictoryType type) { _WinningTeam_ = team; _VictoryType_ = type; }
    public VictoryEventData(Team team, VictoryType type, BombCaseState bombState, string teamName) 
    { _WinningTeam_ = team; _VictoryType_ = type; _BombState_ = bombState; _TeamName_ = teamName; }
}

[System.Serializable]
public class VictorySequenceComponents
{
    public BombManager _BombManager_;
    public CameraManager _CameraManager_;
    public VictoryUiManager _VictoryUiManager_;
    public SceneSetupManager _SceneSetupManager_;
    public ScoreManager _ScoreManager_;
    public DefuseBombManager _DefuseBombManager_;
    public PlantBombManager _PlantBombManager_;
    public BombExplosionController _BombExplosionController_;
    public CountdownManager _CountdownManager_;
}

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private VictorySequenceComponents m_VictorySequenceComponents;

    [HideInInspector] public UnityEvent<VictoryEventData> OnVictoryShownEvent = new UnityEvent<VictoryEventData>();
    [HideInInspector] public UnityEvent<Action> OnZoomOutOfComplex = new UnityEvent<Action>();

    private void Awake()
    {
        m_VictorySequenceComponents._VictoryUiManager_.OnVictoryShownEvent.AddListener((data) => {
            // Invoke GameManager to trigger repair state
            OnVictoryShownEvent?.Invoke(data);
        });
    }

    private void OnDestroy()
    {
        m_VictorySequenceComponents._VictoryUiManager_.OnVictoryShownEvent.RemoveAllListeners();
    }

    public void InitVictory(VictoryEventData data)
    {
        /*
            FLOW:
                - WIN ROUND CONDITION MET:
                    - BOMB EXPLODES SUBFLOW                             v/
                    - BOMB DEFUSED SUBFLOW                              v/
                    - ROUND TIME ENDED SUBFLOW                          v/
                    - GAME TIME ENDED / SCORE LIMIT REACHED SUBFLOW
       
                *******************************************************************************
                *                               BOMB EXPLODES                                 *
                *******************************************************************************
                - Case 01: 
                    Closed Bomb Case State - bomb case is closed - circuit not visible  v/
                - Case 02:
                    MultiComplex Bomb State - bomb case is opened - circuit is visible  v/
                    ?? Special Cases: when you are in keyboard/pad substate             v/
                    
                *******************************************************************************
                *                               BOMB DEFUSED                                  *
                *******************************************************************************
                
                - Case 01:
                    Indicate Bomb Defused - UI POP UP - Victory - Repair (slightly different case) v/

                *******************************************************************************
                *                             ROUND TIME ENDED                                *
                *******************************************************************************
                
                - Case 01:
                    Indicate Round Time Elapsed - UI POP UP - Resolve SubStates - Victory - Sparks - Repair v/
         */

        switch (data._VictoryType_)
        {
            case VictoryType.BombExploded:
                StartCoroutine(BombExplosionSequence(data));
                break;
            case VictoryType.BombDefused:
                StartCoroutine(BombDefusedSequence(data));
                break;
            case VictoryType.RoundTimeEnded:
                StartCoroutine(RoundTimeEndedSequence(data));
                break;
            default:
                break;
        }
    }

    private IEnumerator RoundTimeEndedSequence(VictoryEventData data)
    {
        m_VictorySequenceComponents._SceneSetupManager_.EnableBombCoverUps(true);
        m_VictorySequenceComponents._BombManager_.DisableBombInteractionAndWobbleEffect();

        m_VictorySequenceComponents._VictoryUiManager_.PlayRoundTimeLimitReachedAnime();

        yield return new WaitForSeconds(2f);

        // CASE 02
        if (data._BombState_ == BombCaseState.Open || data._BombState_ == BombCaseState.Hacking)
        {
            bool isDefuseSubState;
            m_VictorySequenceComponents._DefuseBombManager_.TryForceCloseEncryptor(out isDefuseSubState);
            bool isPlantSubState;
            m_VictorySequenceComponents._PlantBombManager_.TryForceCloseEncryptor(out isPlantSubState);

            if (isDefuseSubState || isPlantSubState)
            {
                yield return new WaitForSeconds(2f);
            }

            OnZoomOutOfComplex?.Invoke(() => {
            });
        }

        m_VictorySequenceComponents._BombManager_.IgniteSparks();

        yield return new WaitForSeconds(1f);

        bool isScoreLimit = false;
        bool isDraw = false;
        Team winningTeam = Team.None;
        m_VictorySequenceComponents._ScoreManager_.IncreaseScore(data._WinningTeam_, out isScoreLimit, out winningTeam, out isDraw);

        Debug.Log($"Is Score Limit Reached: {isScoreLimit}");

        data._ScoreLimitReached_ = isScoreLimit;
        data._ScoreLimitReachedByTeam_ = winningTeam;
        data._IsDraw_ = isDraw;

        AudioManager.INSTANCE.PlayWinningTeamVO(data._WinningTeam_);

        // WIN UI
        m_VictorySequenceComponents._VictoryUiManager_.InitVictoryUi(data);

        yield return new WaitForSeconds(1f);

        m_VictorySequenceComponents._CountdownManager_.DeinitRoundTimeCountdown();
    }

    private IEnumerator BombDefusedSequence(VictoryEventData data)
    {
        m_VictorySequenceComponents._SceneSetupManager_.EnableBombCoverUps(true);

        AudioManager.INSTANCE.TriggerDefuseBombAudio(data._WinningTeam_);

        yield return new WaitForSeconds(2f);

        m_VictorySequenceComponents._VictoryUiManager_.PlayBombDefusedAnime();

        yield return new WaitForSeconds(1f);

        OnZoomOutOfComplex?.Invoke(() => {
            bool isScoreLimit = false;
            bool isDraw = false;
            Team winningTeam = Team.None;

            m_VictorySequenceComponents._ScoreManager_.IncreaseScore(data._WinningTeam_, out isScoreLimit, out winningTeam, out isDraw);

            Debug.Log($"Is Score Limit Reached: {isScoreLimit}");

            data._ScoreLimitReached_ = isScoreLimit;
            data._ScoreLimitReachedByTeam_ = winningTeam;
            data._IsDraw_ = isDraw;

            m_VictorySequenceComponents._VictoryUiManager_.InitVictoryUi(data);


            m_VictorySequenceComponents._BombManager_.IgniteSparks();
        });

        yield return new WaitForSeconds(2f);

        m_VictorySequenceComponents._CountdownManager_.DeinitRoundTimeCountdown();
    }

    private IEnumerator BombExplosionSequence(VictoryEventData data)
    {
        // CASE 01
        if(data._BombState_ == BombCaseState.Close)
        {
            // Trigger SFX
            AudioManager.INSTANCE.TriggerExplosionAudio(data._WinningTeam_);

            m_VictorySequenceComponents._BombManager_.ForceOpenBombBehaviour(() => {
                ExplodeBomb(data);
            });
        }

        // CASE 02
        else if (data._BombState_ == BombCaseState.Open || data._BombState_ == BombCaseState.Hacking) 
        {
            bool isSubState;
            m_VictorySequenceComponents._DefuseBombManager_.TryForceCloseEncryptor(out isSubState);

            if (isSubState)
            {
                yield return new WaitForSeconds(2f);    
            }

            AudioManager.INSTANCE.TriggerExplosionAudio(data._WinningTeam_);


            OnZoomOutOfComplex?.Invoke(() => {
                ExplodeBomb(data);
            });
            
        }

        yield return new WaitForSeconds(2f);

        m_VictorySequenceComponents._CountdownManager_.DeinitRoundTimeCountdown();
    }

    public void ResetBombAfterMathEffect()
    {
        m_VictorySequenceComponents._BombExplosionController_.ResetAfterMathFlyingObject();
    }

    private void ExplodeBomb(VictoryEventData data)
    {
        // Audio
        AudioManager.INSTANCE.ExplodeOnly();

        // Explode Bomb
        m_VictorySequenceComponents._BombExplosionController_.ExplodeBomb(() => {
            m_VictorySequenceComponents._BombManager_.TurnOffAllLights();
            m_VictorySequenceComponents._BombManager_.InitClockMotion(false);
            m_VictorySequenceComponents._SceneSetupManager_.EnableBombCoverUps(true);
        });

        // Shake Cam
        m_VictorySequenceComponents._CameraManager_.ShakeCamera(() => {
            bool isScoreLimit = false;
            bool isDraw = false;
            Team winningTeam = Team.None;

            m_VictorySequenceComponents._ScoreManager_.IncreaseScore(data._WinningTeam_, out isScoreLimit, out winningTeam, out isDraw);

            Debug.Log($"Is Score Limit Reached: {isScoreLimit}");

            data._ScoreLimitReached_ = isScoreLimit;
            data._ScoreLimitReachedByTeam_ = winningTeam;
            data._IsDraw_ = isDraw;

            // WIN UI
            m_VictorySequenceComponents._VictoryUiManager_.InitVictoryUi(data);
        });
    }
}
