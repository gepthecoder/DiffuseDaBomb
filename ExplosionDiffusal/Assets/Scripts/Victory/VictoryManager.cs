using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

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
}

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private VictorySequenceComponents m_VictorySequenceComponents;
    [SerializeField] private BombExplosionController m_BombExplosionController;

    public void InitVictory(VictoryEventData data)
    {
        /*
            FLOW:
                - WIN ROUND CONDITION MET!
                    - BOMB EXPLODES SUBFLOW
                    - BOMB DEFUSED SUBFLOW
                    - GAME TIME ENDED / SCORE LIMIT REACHED SUBFLOW:
       
                *******************************************************************************
                *                               BOMB EXPLODES                                 *
                *******************************************************************************
                - Case 1: 
                    Closed Bomb Case State - bomb case is closed - circuit not visible
                - Case 2:
                    MultiComplex Bomb State - bomb case is opened - circuit is visible

                ?? Joined Behaviour:
                    

                ?? Special Cases:

                
         */

        switch (data._VictoryType_)
        {
            case VictoryType.BombExploded:
                StartCoroutine(BombExplosionSequence(data));
                break;
            case VictoryType.BombDefused:
                break;
            case VictoryType.GameTimeEnded:
                break;
            default:
                break;
        }
    }

    private IEnumerator BombExplosionSequence(VictoryEventData data)
    {
        if(data._BombState_ == BombCaseState.Close)
        {
            m_VictorySequenceComponents._BombManager_.ForceOpenBombBehaviour(() => {
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
            });
        } 
        else { }


        yield break;
    }
}
