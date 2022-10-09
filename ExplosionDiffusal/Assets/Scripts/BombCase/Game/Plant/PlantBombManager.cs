using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;

public enum PlantBombState { Start, Hacking, Success, Done } 

public class PlantBombManager : MonoBehaviour
{
    [SerializeField] private PlantBombHackingController m_HackingController;
    [SerializeField] private PlantBombActionHandler m_PlantBombActionHandler;

    [SerializeField] private Lights m_Lights;

    [SerializeField] private List<Highlighter> m_HighlightedObjects;

    private PlantBombState i_CurrentState = PlantBombState.Start;

    [HideInInspector] public UnityEvent OnPlantBombDoneEvent = new UnityEvent();

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
                TriggerPlantBehaviour(PlantBombState.Start, new HackingItemData(data.CodeEncryption, false));
            });

        m_HackingController.OnItemHackedEvent.AddListener(
            (data) => { 
                TriggerPlantBehaviour(PlantBombState.Start, new HackingItemData(data.CodeEncryption, true)); }    
            );

        m_HackingController.OnAllItemsHackedEvent.AddListener(
            (data) => {
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
                    m_Lights.LightEffect();
                    HighlightElements(true);
                }             
                break;
            case PlantBombState.Hacking:
                Debug.Log($"<color=green>PlantBombState</color><color=gold>Hacking</color>: {data.SelectedType}");
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
                break;
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
        if (Input.GetMouseButtonDown(0) && i_CurrentState != PlantBombState.Hacking)
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
}
