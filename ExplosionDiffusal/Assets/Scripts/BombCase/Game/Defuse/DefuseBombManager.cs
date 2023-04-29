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
    [SerializeField] private LightsController m_LightsController;
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
    private ClickableType m_CurrentSelectedEncryptor = ClickableType.None;

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
            m_LightsController.PlayLightAnimator(LightType.PlasticBomb, LightAction.Off);

            m_CurrentSelectedEncryptor = ClickableType.None;

            TriggerDefuseBehaviour(DefuseBombState.Done, data);
        });

        m_DefuseBombController.OnItemHackedEvent.AddListener((data) => {
            if (data.CodeEncryption == CodeEncryptionType.KeyboardEncryption)
            {
                m_LightsController.PlayLightAnimator(LightType.PlasticBomb, LightAction.Off);
            }

            m_CurrentSelectedEncryptor = ClickableType.None;

            OnDefuseBombEvent?.Invoke(data.CodeEncryption);
            TriggerDefuseBehaviour(DefuseBombState.Start, new HackingItemData(data.CodeEncryption, true));
        });

        m_KeyboardUI.OnEncryptorClose.AddListener((data) => {
            if (data.gState != GameState.Defusing)
                return;

            m_CurrentSelectedEncryptor = ClickableType.None;

            m_DefuseBombController.OnHackingItemDeselected();

            DefuseBombState goToState = data.ForceCloseMultiComplexBomb ? DefuseBombState.Null : DefuseBombState.Start;
            TriggerDefuseBehaviour(goToState, new HackingItemData(data.CodeEncryption, false));
        });

        m_KeypadUI.OnEncryptorClose.AddListener((data) => {
            if (data.gState != GameState.Defusing)
                return;

            m_CurrentSelectedEncryptor = ClickableType.None;

            m_DefuseBombController.OnHackingItemDeselected();

            DefuseBombState goToState = data.ForceCloseMultiComplexBomb ? DefuseBombState.Null : DefuseBombState.Start;
            TriggerDefuseBehaviour(goToState, new HackingItemData(data.CodeEncryption, false));
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
                if(data != null){
                    break; 
                } 
                else {
                    SetupInitialBombDefuseSettings();
                }
                break;
            case DefuseBombState.Hacking:
                Debug.Log($"<color=blue>DefuseBombState</color><color=gold>Hacking</color>: {data.SelectedType}");
                m_CurrentSelectedEncryptor = data.SelectedType;
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
                TriggerDefuseBehaviour(DefuseBombState.Null);
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

    public void ResetBombDefuseSettings()
    {
        m_Keyboard._Clickable.CanClick = true;
        m_Keypad._Clickable.CanClick = true;

        m_Keyboard._Highlighter.CanHiglight = true;
        m_Keyboard._Highlighter.HighlightMe();
        m_Keypad._Highlighter.CanHiglight = true;
        m_Keypad._Highlighter.HighlightMe();

        m_KeyboardUI.ClearCode();
        m_KeypadUI.ClearCode2();

        m_CurrentSelectedEncryptor = ClickableType.None;

        m_KeyboardUI.EnableDefusalCodesUi(false);
        m_KeypadUI.EnableDefusalCodesUi(false);

        m_DefuseBombController.ResetTaskInfo();
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

        m_CurrentSelectedEncryptor = ClickableType.None;
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
        m_ClockMotionController.EnableClockMotion(ClockMotionType.MultiComplexClock, enable);
    }

    public void TryForceCloseEncryptor(out bool success)
    {
        if (m_CurrentSelectedEncryptor == ClickableType.None)
        {
            TriggerDefuseBehaviour(DefuseBombState.Null);
            success = false;
            return;
        }

        if (m_CurrentSelectedEncryptor == ClickableType.Keyboard)
        {
            m_KeyboardUI.OnEncryptorClose?.Invoke(new HackingItemData(CodeEncryptionType.KeyboardEncryption, GameState.Defusing, true));
        }

        else if (m_CurrentSelectedEncryptor == ClickableType.Keypad)
        {
            m_KeypadUI.OnEncryptorClose?.Invoke(new HackingItemData(CodeEncryptionType.KeyPadEncryption, GameState.Defusing, true));
        }

        success = true;
    }
}
