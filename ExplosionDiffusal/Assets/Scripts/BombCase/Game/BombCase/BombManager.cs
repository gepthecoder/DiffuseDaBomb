using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;

public enum BombCaseState { Close, Open, Hacking, Null }
public enum BombCaseSubState { OnBombCasePressDown, OnBombCasePressUp, NonInteractive, }

public class BombManager : MonoBehaviour
{
    [SerializeField] private ClockMotionController m_ClockMotionController;
    [Space(5)]
    [SerializeField] private bool m_IsSuitcaseOpeningEnabled = true;
    [Space(5)]
    [SerializeField] private BombOpeningUiManager m_BombOpeningUiManager;
    [SerializeField] private BombCase m_BombCase;
    [SerializeField] private List<Light> m_BombCaseLights;
    [SerializeField] private List<Light> m_BombCaseCircuitLights;
    [Space(5)]
    [SerializeField] private Sparks m_Sparks;
    [SerializeField] private LayerMask m_LayerMaskInteraction;

    [HideInInspector] public UnityEvent BombCaseOpeningEvent;

    private UnityEvent<BombCaseSubState> OnBombCaseInteractionEvent;

    private BombCaseState m_CurrentBombCaseState = BombCaseState.Close;
    private BombCaseSubState m_SubBombCaseState = BombCaseSubState.NonInteractive;

    private const string m_BombCaseTag = "BombCase";

    //--------------------------------------------/\--------------------------------------------\\
    private float m_OnDownTimer = 0;
    private float m_OnDownTreshold = 3f;
    private float m_OnDownTresholdShort = 2f;
    private bool m_OpenSuitcase = false;
    private bool m_OnDownStart = false;
    private bool m_CanCheckBombInteraction = false;
    //--------------------------------------------\/--------------------------------------------\\

    private void Awake()
    {
        if (BombCaseOpeningEvent == null)
        {
            BombCaseOpeningEvent = new UnityEvent();
        }

        if (OnBombCaseInteractionEvent == null)
        {
            OnBombCaseInteractionEvent = new UnityEvent<BombCaseSubState>();
        }

        m_BombCase.Init();
    }

    private void Start()
    {
        TurnOnLightSmooth(true);
        SetupBombOpeningSlider(m_OnDownTreshold);

        OnBombCaseInteractionEvent.AddListener( (bombCaseSubState) => {
            m_BombCase.SetWobbleIntensity(bombCaseSubState);
        });
    }

    private void OnDestroy()
    {
        OnBombCaseInteractionEvent.RemoveAllListeners();
    }

    private void SetupBombOpeningSlider(float onDownTreshold)
    {
        if (!m_IsSuitcaseOpeningEnabled)
            return;

        m_BombOpeningUiManager.SetupSlider(onDownTreshold);
    }

    private void Update()
    {
        CheckUserBombInteraction();
    }

    public void TriggerBombBehaviour(BombCaseState state)
    {
        m_CurrentBombCaseState = state;

        if (m_CurrentBombCaseState == BombCaseState.Open)
        {
            m_BombCase.TriggerBehaviour(state, () => 
            { 
                TriggerBombBehaviour(BombCaseState.Hacking);
            });

            BombCaseOpeningEvent?.Invoke();
        }
        else {
            m_CanCheckBombInteraction = state == BombCaseState.Close ? true : false;
            m_BombCase.TriggerBehaviour(state); 
        }
    }

    public void ForceOpenBombBehaviour(Action action)
    {
        m_CurrentBombCaseState = BombCaseState.Open;

        m_BombCase.TriggerBehaviour(m_CurrentBombCaseState, action);
    }

    public void TurnOnLightSmooth(bool on)
    {
        foreach (var light in m_BombCaseLights)
        {
            light.DOIntensity(on ? 2f : 0f, 1.5f);
        }
    }

    public void TurnOffAllLights()
    {
        TurnOnLightSmooth(false);

        m_BombCaseCircuitLights.ForEach((light) => {
            light.enabled = false;
        });
    }

    public void TurnOnAllCircuitLights(bool on)
    {
        m_BombCaseCircuitLights.ForEach((light) => {
            light.enabled = on;
        });
    }

    public void DisableBombInteractionAndWobbleEffect()
    {
        m_CanCheckBombInteraction = false;
        m_BombCase.TriggerBehaviour(BombCaseState.Null);

        if(m_CurrentBombCaseState == BombCaseState.Close)
        {
            m_BombCase.OpenBombCase(() => { });
        }
    }

    private void CheckUserBombInteraction()
    {
        if (m_CurrentBombCaseState != BombCaseState.Close || !m_CanCheckBombInteraction)
            return;

        if(m_IsSuitcaseOpeningEnabled)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, m_LayerMaskInteraction))
                {
                    if (Helper.INSTANCE != null && Helper.INSTANCE.IsPointerOverUI())
                        return;

                    if (hit.transform.tag == m_BombCaseTag)
                    {
                        OnBombCaseInteractionEvent?.Invoke(BombCaseSubState.OnBombCasePressDown);
                        m_BombOpeningUiManager.ShowSlider(true);
                        m_OnDownStart = true;        
                    }
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                m_OnDownStart = false;
                OnBombCaseInteractionEvent?.Invoke(BombCaseSubState.OnBombCasePressUp);
            }

            if (m_OnDownStart)
            {
                m_OnDownTimer += Time.deltaTime;
            } else
            {
                m_OnDownTimer -= Time.deltaTime * 2;
                if(m_OnDownTimer <= 0)
                {
                    m_OnDownTimer = 0;
                    m_BombOpeningUiManager.ShowSlider(false);
                }
            }

            if(m_OnDownTimer >= m_OnDownTresholdShort)
            {
                AudioManager.INSTANCE.PlayAudioEffectByType(AudioEffect.OpenBomb);
            }

            if (m_OnDownTimer >= m_OnDownTreshold)
            {
                m_OpenSuitcase = true;
                m_OnDownTimer = 0;
                m_BombOpeningUiManager.ShowSlider(false);
            }

            m_BombOpeningUiManager.SetSliderValue(m_OnDownTimer);

            if (m_OpenSuitcase)
            {
                m_OpenSuitcase = false;
                m_OnDownStart = false;
                TriggerBombBehaviour(BombCaseState.Open);

                OnBombCaseInteractionEvent?.Invoke(BombCaseSubState.NonInteractive);            
            }

        } else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, m_LayerMaskInteraction))
                {
                    if (Helper.INSTANCE != null && Helper.INSTANCE.IsPointerOverUI())
                        return;

                    if (hit.transform.tag == m_BombCaseTag)
                    {
                        TriggerBombBehaviour(BombCaseState.Open);
                    }
                }
            }
        }
    }

    public void InitClockMotion(bool enable)
    {
        m_ClockMotionController.EnableClockMotion(ClockMotionType.BombCaseClock, enable);
    }

    public BombCaseState GetCurrentBombCaseState()
    {
        return m_CurrentBombCaseState;
    }

    public void IgniteSparks()
    {
        m_Sparks.IgniteSparkSystem();
    }

    public void ResetSparks()
    {
        m_Sparks.ResetSparks();
    }

    public void SetSparkSpeed(float speed)
    {
        m_Sparks.SetIgniteSpeed(speed);
    }
}
