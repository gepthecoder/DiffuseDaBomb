using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public enum BombCaseState { Close, Open, ZoomIn, ZoomOut, }

public class BombManager : MonoBehaviour
{
    [SerializeField] private BombCase m_BombCase;
    [SerializeField] private List<Light> m_BombCaseLights;

    [HideInInspector] public UnityEvent BombCaseOpeningEvent;

    private BombCaseState m_CurrentBombCaseState = BombCaseState.Close;

    private const string m_BombCaseTag = "BombCase";

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

        if(m_CurrentBombCaseState == BombCaseState.Open)
        {
            m_BombCase.TriggerBehaviour(state, () => 
            { 
                TriggerBombBehaviour(BombCaseState.ZoomIn);
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
        if (Input.GetMouseButtonDown(0) && m_CurrentBombCaseState != BombCaseState.Open)
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
