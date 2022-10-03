using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public enum BombCaseState { Close, Open, Hacking, }

public class BombManager : MonoBehaviour
{
    [SerializeField] private bool m_IsSuitcaseOpeningEnabled = true;
    [Space(5)]
    [SerializeField] private BombOpeningUiManager m_BombOpeningUiManager;
    [SerializeField] private BombCase m_BombCase;
    [SerializeField] private List<Light> m_BombCaseLights;

    [HideInInspector] public UnityEvent BombCaseOpeningEvent;

    private BombCaseState m_CurrentBombCaseState = BombCaseState.Close;

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
    }

    private void Start()
    {
        TurnOnLightSmooth(true);
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
                        m_BombOpeningUiManager.ShowSlider(true);
                        m_OnDownStart = true;        
                    }
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                m_OnDownStart = false;
            }

            if (m_OnDownStart)
            {
                m_OnDownTimer += Time.deltaTime;
                m_BombOpeningUiManager.SetSliderValue(m_OnDownTimer);
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
                TriggerBombBehaviour(BombCaseState.Open);
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
