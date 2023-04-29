using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;

public enum PlantBombState { Start, Hacking, Success, Done, Null, } 

public class PlantBombManager : MonoBehaviour
{
    [SerializeField] private PlantBombHackingController m_HackingController;
    [SerializeField] private PlantBombActionHandler m_PlantBombActionHandler;
    [Space(5)]
    [SerializeField] private ClockMotionController m_ClockMotionController;
    [SerializeField] private LightsController m_LightsController;

    [SerializeField] private Lights m_Lights;

    [SerializeField] private List<Highlighter> m_HighlightedObjects;

    private PlantBombState i_CurrentState = PlantBombState.Start;
    private ClickableType m_CurrentSelectedEncryptor = ClickableType.None;

    [HideInInspector] public UnityEvent OnPlantBombDoneEvent = new UnityEvent();
    [HideInInspector] public UnityEvent<CodeEncryptionType> OnPlantBombEvent = new UnityEvent<CodeEncryptionType>();

    private void Start()
    {
        m_Lights.TurnOnLightSmooth(false);
        HighlightElements(false);

        Subscribe();
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        m_PlantBombActionHandler.OnEncryptorCloseEvent.AddListener(
            (data) => {
                if (data.gState != GameState.Planting)
                    return;

                m_CurrentSelectedEncryptor = ClickableType.None;

                TriggerPlantBehaviour(PlantBombState.Start, new HackingItemData(data.CodeEncryption, false));
            });

        m_HackingController.OnItemHackedEvent.AddListener(
            (data) => {
                OnPlantBombEvent?.Invoke(data.CodeEncryption);

                if(data.CodeEncryption == CodeEncryptionType.KeyboardEncryption)
                {
                    m_LightsController.PlayLightAnimator(LightType.PlasticBomb, LightAction.On);
                }

                m_CurrentSelectedEncryptor = ClickableType.None;

                TriggerPlantBehaviour(PlantBombState.Start, new HackingItemData(data.CodeEncryption, true)); }    
            );

        m_HackingController.OnAllItemsHackedEvent.AddListener(
            (data) => {
                m_Lights.EnableKeyboardLight(false);
                m_Lights.EnableKeypadLight(false);

                m_LightsController.PlayLightAnimator(LightType.PlasticBomb, LightAction.Trip);

                m_CurrentSelectedEncryptor = ClickableType.None;

                TriggerPlantBehaviour(PlantBombState.Done, data);
            });
    }

    private void UnSubscribe()
    {
        m_HackingController.OnItemHackedEvent.RemoveAllListeners();
        m_PlantBombActionHandler.OnEncryptorCloseEvent.RemoveAllListeners();
    }

    private void Update()
    {
        CheckUserInteraction();
    }

    public void TriggerPlantBehaviour(PlantBombState state, HackingItemData data = null)
    {
        i_CurrentState = state;

        switch (state)
        {
            case PlantBombState.Start:
                Debug.Log("<color=green>PlantBombState</color><color=gold>Start</color>");
                if(data != null) {
                    if(data.CloseHackingItemSuccess)
                    {
                        if (data.CodeEncryption == CodeEncryptionType.KeyboardEncryption)
                        {
                            m_Lights.EnableKeyboardLight(false);
                            m_Lights.LightEffectByType(CodeEncryptionType.KeyPadEncryption);
                        }
                        else if(data.CodeEncryption == CodeEncryptionType.KeyPadEncryption)
                        {
                            m_Lights.EnableKeypadLight(false);
                            m_Lights.LightEffectByType(CodeEncryptionType.KeyboardEncryption);
                        }
                    }           
                }
                else
                {
                    m_PlantBombActionHandler.SetMainStateForEncryptors(GameState.Planting);

                    m_Lights.LightEffect();
                    HighlightElements(true);
                }             
                break;
            case PlantBombState.Hacking:
                Debug.Log($"<color=green>PlantBombState</color><color=gold>Hacking</color>: {data.SelectedType}");
                m_CurrentSelectedEncryptor = data.SelectedType;
                m_HackingController.OnHackingItemSelected(data);
                break;
            case PlantBombState.Success:
                Debug.Log("<color=green>PlantBombState</color><color=gold>Success</color>");
                HighlightByType(false, data.CodeEncryption);
                m_Lights.LightUpBombs(true, data.CodeEncryption);

                m_HackingController.OnItemHacked(data);
                break;
            case PlantBombState.Done:
                Debug.Log("<color=green>PlantBombState</color><color=gold>Done</color>");
                OnPlantBombDoneEvent?.Invoke();
                TriggerPlantBehaviour(PlantBombState.Null);
                break;
            case PlantBombState.Null:
            default:
                break;
        }
    }

    private void HighlightElements(bool highlight)
    {
        foreach (var element in m_HighlightedObjects)
        {
            element.CanHiglight = false;

            if (highlight)
            {
                element.GetComponent<Clickable>().CanClick = true;
                element.CanHiglight = true;
                element.HighlightMe();
            }
        }
    }

    private void HighlightByType(bool highlight, CodeEncryptionType type)
    {
        foreach (var element in m_HighlightedObjects)
        {
            Code code = element.GetComponent<Code>();

            if (code.EncryptionType == type)
            {
                if(highlight)
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

    private void CheckUserInteraction()
    {
        if (Input.GetMouseButtonDown(0) && i_CurrentState == PlantBombState.Start)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var clickable = hit.transform.GetComponent<Clickable>();
                if (clickable != null)
                {
                    if(clickable.CanClick)
                    {
                        HackingItemData DATA = new HackingItemData(clickable.clickableType, clickable.positionWorldSpace);
                        TriggerPlantBehaviour(PlantBombState.Hacking, DATA);
                    }             
                }
            }
        }
    }

    public void InitClockMotion(bool enable)
    {
        m_ClockMotionController.EnableClockMotion(ClockMotionType.MultiComplexClock, enable);
    }

    public void InitLights(LightType type, LightAction action)
    {
        m_LightsController.PlayLightAnimator(type, action);
    }

    public void Deinit()
    {
        HighlightElements(true);

        InitClockMotion(false);
        InitLights(LightType.PlasticBomb, LightAction.Off);

        m_Lights.EnableKeyboardLight(true);
        m_Lights.EnableKeypadLight(true);

        m_Lights.LightUpBombs(false, CodeEncryptionType.KeyboardEncryption);
        m_Lights.LightUpBombs(false, CodeEncryptionType.KeyPadEncryption);

        m_LightsController.ForceStopLightSource(LightType.PlasticBomb);

        m_CurrentSelectedEncryptor = ClickableType.None;

        m_PlantBombActionHandler.ActivateBombEffect(true, CodeEncryptionType.KeyboardEncryption);
    }

    public void TryForceCloseEncryptor(out bool success)
    {
        if (m_CurrentSelectedEncryptor == ClickableType.None)
        {
            TriggerPlantBehaviour(PlantBombState.Null);
            success = false;
            return;
        }

        if (m_CurrentSelectedEncryptor == ClickableType.Keyboard)
        {
            m_PlantBombActionHandler.GetKeyboardEncryptor().OnEncryptorClose?.Invoke(new HackingItemData(CodeEncryptionType.KeyboardEncryption, GameState.Planting, true));
        }

        else if (m_CurrentSelectedEncryptor == ClickableType.Keypad)
        {
            m_PlantBombActionHandler.GetKeypadEncryptor().OnEncryptorClose?.Invoke(new HackingItemData(CodeEncryptionType.KeyPadEncryption, GameState.Planting, true));
        }

        success = true;
    }
}
