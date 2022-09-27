using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BombCaseState { Close, Open, ZoomIn, ZoomOut, }

public class BombManager : MonoBehaviour
{
    [SerializeField] private BombCase m_BombCase;
    [SerializeField] private CameraManager m_CameraManager;

    [HideInInspector] public UnityEvent BombCaseOpeningEvent;

    private BombCaseState m_CurrentBombCaseState = BombCaseState.Close;

    private void Awake()
    {
        if (BombCaseOpeningEvent == null)
        {
            BombCaseOpeningEvent = new UnityEvent();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && m_CurrentBombCaseState != BombCaseState.Open)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "BombCase")
                {
                    TriggerBombBehaviour(BombCaseState.Open);
                }
            }
        }
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
}
