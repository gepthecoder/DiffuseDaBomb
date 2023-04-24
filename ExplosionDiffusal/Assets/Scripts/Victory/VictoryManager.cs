using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;

public enum Team { Axis, Allies, None }
public enum VictoryType { BombExploded, BombDefused, GameTimeEnded, }

public class VictoryEventData
{
    public BombCaseState _BombState_;
    public Team _WinningTeam_;
    public VictoryType _VictoryType_;
    public string _TeamName_;

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
}

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private VictorySequenceComponents m_VictorySequenceComponents;
    [SerializeField] private BombExplosionController m_BombExplosionController;

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
                - WIN ROUND CONDITION MET!
                    - BOMB EXPLODES SUBFLOW     v/
                    - BOMB DEFUSED SUBFLOW      
                    - ROUND TIME ENDED SUBFLOW  
                    - GAME TIME ENDED / SCORE LIMIT REACHED SUBFLOW:
       
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
                    Indicate Bomb Defused - UI POP UP - Victory - Repair (slightly different case)
         */

        switch (data._VictoryType_)
        {
            case VictoryType.BombExploded:
                StartCoroutine(BombExplosionSequence(data));
                break;
            case VictoryType.BombDefused:
                StartCoroutine(BombDefusedSequence(data));
                break;
            case VictoryType.GameTimeEnded:
                break;
            default:
                break;
        }
    }

    private IEnumerator BombDefusedSequence(VictoryEventData data)
    {
        m_VictorySequenceComponents._SceneSetupManager_.EnableBombCoverUps(true);

        yield return new WaitForSeconds(2f);

        m_VictorySequenceComponents._VictoryUiManager_.PlayBombDefusedAnime();

        yield return new WaitForSeconds(1f);

        OnZoomOutOfComplex?.Invoke(() => {
            m_VictorySequenceComponents._VictoryUiManager_.InitVictoryUi(data);

            bool isScoreLimit = false; // TODO
            m_VictorySequenceComponents._ScoreManager_.IncreaseScore(data._WinningTeam_, out isScoreLimit);

            Debug.Log($"Is Score Limit Reached: {isScoreLimit}");
        });

        yield break;
    }

    private IEnumerator BombExplosionSequence(VictoryEventData data)
    {
        // CASE 01
        if(data._BombState_ == BombCaseState.Close)
        {
            m_VictorySequenceComponents._BombManager_.ForceOpenBombBehaviour(() => {
                ExplodeBomb(data);
            });
        }

        // CASE 02
        else if (data._BombState_ == BombCaseState.Open || data._BombState_ == BombCaseState.Hacking) 
        {
            bool isSubState;
            m_VictorySequenceComponents._DefuseBombManager_.TryForceCloseEncryptor(out isSubState);

            if(isSubState)
            {
                yield return new WaitForSeconds(2f);    
            }

            OnZoomOutOfComplex?.Invoke(() => {
                ExplodeBomb(data);
            });
            
        }

        yield break;
    }

    public void ResetBombAfterMathEffect()
    {
        m_BombExplosionController.ResetAfterMathFlyingObject();
    }

    private void ExplodeBomb(VictoryEventData data)
    {
        // Explode Bomb
        m_BombExplosionController.ExplodeBomb(() => {
            m_VictorySequenceComponents._BombManager_.TurnOffAllLights();
            m_VictorySequenceComponents._BombManager_.InitClockMotion(false);
            m_VictorySequenceComponents._SceneSetupManager_.EnableBombCoverUps(true);
        });
        // Shake Cam
        m_VictorySequenceComponents._CameraManager_.ShakeCamera(() => {
            // WIN UI
            m_VictorySequenceComponents._VictoryUiManager_.InitVictoryUi(data);

            bool isScoreLimit = false;
            m_VictorySequenceComponents._ScoreManager_.IncreaseScore(data._WinningTeam_, out isScoreLimit);

            // TODO: score limit reached case
            Debug.Log($"Is Score Limit Reached: {isScoreLimit}");
        });
    }
}
