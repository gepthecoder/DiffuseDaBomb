using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;

public enum BombCaseState { Close, Open, Hacking, }
public enum BombCaseSubState { OnBombCasePressDown, OnBombCasePressUp, NonInteractive, }

public class BombManager : MonoBehaviour
{
    [SerializeField] private bool m_IsSuitcaseOpeningEnabled = true;
    [Space(5)]
    [SerializeField] private BombOpeningUiManager m_BombOpeningUiManager;
    [SerializeField] private BombCase m_BombCase;
    [SerializeField] private List<Light> m_BombCaseLights;

    [HideInInspector] public UnityEvent BombCaseOpeningEvent;

    private UnityEvent<BombCaseSubState> OnBombCaseInteractionEvent;

    private BombCaseState m_CurrentBombCaseState = BombCaseState.Close;
    private BombCaseSubState m_SubBombCaseState = BombCaseSubState.NonInteractive;

    private const string m_BombCaseTag = "BombCase";

    //--------------------------------------------/\--------------------------------------------\\
    private float m_OnDownTimer = 0;
    private float m_OnDownTreshold = 3f;
    private bool m_OpenSuitcase = false;
    private bool m_OnDownStart = false;
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
        else { m_BombCase.TriggerBehaviour(state); }
    }

    private void TurnOnLightSmooth(bool on)
    {
        foreach (var light in m_BombCaseLights)
        {
            light.DOIntensity(on ? 2f : 0f, 1.5f);
        }
    }

    private void CheckUserBombInteraction()
    {
        if (m_CurrentBombCaseState != BombCaseState.Close)
            return;

        if(m_IsSuitcaseOpeningEnabled)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
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
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == m_BombCaseTag)
                    {
                        TriggerBombBehaviour(BombCaseState.Open);
                    }
                }
            }
        }
    }
}
