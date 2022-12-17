using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DefuseBombState { Start, Hacking, Success, Done, Null }

public class DefuseBombManager : MonoBehaviour
{
    [SerializeField] private DefuseBombController m_DefuseBombController;
    [Space(5)]
    [SerializeField] private ClockMotionController m_ClockMotionController;
    [Space(5)]
    [SerializeField] private ItemInteractor m_Keyboard;
    [SerializeField] private ItemInteractor m_Keypad;
    [Space(5)]
    [SerializeField] private Keyboard m_KeyboardUI;
    [SerializeField] private Keypad m_KeypadUI;
    [Space(5)]
    [SerializeField] private Lights m_Lights;
    [SerializeField] private List<Highlighter> m_HighlightedObjects;

    private DefuseBombState i_CurrentState = DefuseBombState.Null;

    [HideInInspector] public UnityEvent OnDefuseBombDoneEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<CodeEncryptionType> OnDefuseBombEvent = new UnityEvent<CodeEncryptionType>();

    private void Awake()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    private void UnSubscribe()
    {
        m_DefuseBombController.OnItemHackedEvent.RemoveAllListeners();
        m_DefuseBombController.OnAllItemsHackedEvent.RemoveAllListeners();
        m_KeyboardUI.OnEncryptorClose.RemoveAllListeners();
        m_KeypadUI.OnEncryptorClose.RemoveAllListeners();
    }

    private void Subscribe()
    {
        m_DefuseBombController.OnAllItemsHackedEvent.AddListener((data) => {
            TriggerDefuseBehaviour(DefuseBombState.Done, data);
        });

        m_DefuseBombController.OnItemHackedEvent.AddListener((data) => {
            OnDefuseBombEvent?.Invoke(data.CodeEncryption);
            TriggerDefuseBehaviour(DefuseBombState.Start, new HackingItemData(data.CodeEncryption, true));
        });

        m_KeyboardUI.OnEncryptorClose.AddListener((data) => {
            if (data.gState != GameState.Defusing)
                return;

            m_DefuseBombController.OnHackingItemDeselected();
            TriggerDefuseBehaviour(DefuseBombState.Start, new HackingItemData(data.CodeEncryption, false));
        });

        m_KeypadUI.OnEncryptorClose.AddListener((data) => {
            if (data.gState != GameState.Defusing)
                return;

            m_DefuseBombController.OnHackingItemDeselected();
            TriggerDefuseBehaviour(DefuseBombState.Start, new HackingItemData(data.CodeEncryption, false));
        });
    }

    private void Update()
    {
        CheckUserInteraction();
    }

    public void TriggerDefuseBehaviour(DefuseBombState state, HackingItemData data = null)
    {
        i_CurrentState = state;

        switch (i_CurrentState)
        {
            case DefuseBombState.Start:
                Debug.Log($"<color=blue>DefuseBombState</color><color=gold>Start</color>");
                if(data != null){ break; } else {
                    SetupInitialBombDefuseSettings();
                }
                break;
            case DefuseBombState.Hacking:
                Debug.Log($"<color=blue>DefuseBombState</color><color=gold>Hacking</color>: {data.SelectedType}");
                m_DefuseBombController.OnHackingItemSelected(data);
                break;
            case DefuseBombState.Success:
                Debug.Log($"<color=blue>DefuseBombState</color><color=gold>Success</color>: {data.CodeEncryption}");
                HighlightByType(false, data.CodeEncryption);
                m_Lights.LightUpBombs(false, data.CodeEncryption);

                m_DefuseBombController.OnItemHacked(data);
                break;
            case DefuseBombState.Done:
                Debug.Log($"<color=blue>DefuseBombState</color><color=gold>Done</color>");
                OnDefuseBombDoneEvent?.Invoke();
                break;
            case DefuseBombState.Null:
            default:
                break;
        }
    }

    private void CheckUserInteraction()
    {
        if (Input.GetMouseButtonDown(0) && i_CurrentState == DefuseBombState.Start) { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var interactor = hit.transform.GetComponent<ItemInteractor>();
                if (interactor != null)
                {
                    if (interactor._Clickable.CanClick)
                    {
                        HackingItemData DATA = new HackingItemData(interactor._Clickable.clickableType, interactor._Clickable.positionWorldSpace);
                        TriggerDefuseBehaviour(DefuseBombState.Hacking, DATA);
                    }
                }
            }
        }
    }

    private void SetupInitialBombDefuseSettings()
    {
        m_Keyboard._Clickable.CanClick = true;
        m_Keypad._Clickable.CanClick = true;

        m_Keyboard._Highlighter.CanHiglight = true;
        m_Keyboard._Highlighter.HighlightMe();
        m_Keypad._Highlighter.CanHiglight = true;
        m_Keypad._Highlighter.HighlightMe();

        m_KeyboardUI.ClearCode();
        m_KeypadUI.ClearCode();

        m_KeyboardUI.currentGameState = GameState.Defusing;
        m_KeypadUI.currentGameState = GameState.Defusing;

        m_KeyboardUI.InitializeBombDefusalCodes();
        m_KeypadUI.InitializeBombDefusalCodes();
    }

    private void HighlightByType(bool highlight, CodeEncryptionType type)
    {
        foreach (var element in m_HighlightedObjects)
        {
            Code code = element.GetComponent<Code>();

            if (code.EncryptionType == type)
            {
                if (highlight)
                {
                    element.CanHiglight = true;
                    element.HighlightMe();
                }
                else
                {
                    element.GetComponent<Clickable>().CanClick = false;
                    element.StopHighlightEffect();
                }

                break;
            }
        }
    }

    public void InitClockMotion(bool enable)
    {
        if (enable)
        {
            m_ClockMotionController.EnableClockMotion();

        }
        else
        {
            m_ClockMotionController.DisableClockMotion();
        }
    }
}
