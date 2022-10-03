using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum PlantBombState { Start, Hacking, Success, } 

public class PlantBombManager : MonoBehaviour
{
    [SerializeField] private PlantBombHackingController m_HackingController;

    [SerializeField] private List<Light> m_CircuitLights;
    [SerializeField] private List<Highlighter> m_HighlightedObjects;

    private PlantBombState i_CurrentState = PlantBombState.Start;

    private bool m_CanLoopLightEffect = false;
    private const float m_EffectTime = 2f;

    private void Start()
    {
        TurnOnLightSmooth(false);
        HighlightElements(false);

        Subscribe();
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        m_HackingController.OnItemHackedEvent.AddListener(
            (data) => { TriggerPlantBehaviour(PlantBombState.Start, data); }    
        );
    }
    private void UnSubscribe()
    {
        m_HackingController.OnItemHackedEvent.RemoveAllListeners();
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
                m_CanLoopLightEffect = true;
                StartCoroutine(LightShowEffect());
                HighlightElements(true);
                break;
            case PlantBombState.Hacking:
                Debug.Log($"<color=green>PlantBombState</color><color=gold>Hacking</color>: {data.SelectedType}");
                m_HackingController.OnHackingItemSelected(data);
                break;
            case PlantBombState.Success:
                Debug.Log("<color=green>PlantBombState</color><color=gold>Success</color>");
                m_HackingController.OnItemHacked(data);
                break;
            default:
                break;
        }
    }

    private void TurnOnLightSmooth(bool on)
    {
        foreach (var light in m_CircuitLights)
        {
            light.DOIntensity(on ? 2.5f : 0.77f, m_EffectTime);
        }
    }

    private IEnumerator LightShowEffect()
    {
        bool on = true;
        while (m_CanLoopLightEffect)
        {
            TurnOnLightSmooth(on);
            on = !on;

            yield return new WaitForSeconds(m_EffectTime);
        }
    }

    private void HighlightElements(bool highlight)
    {
        foreach (var element in m_HighlightedObjects)
        {
            element.CanHiglight = false;

            if (highlight)
            {
                element.CanHiglight = true;
                element.HighlightMe();
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
                    HackingItemData DATA = new HackingItemData(clickable.clickableType, clickable.positionWorldSpace);
                    TriggerPlantBehaviour(PlantBombState.Hacking, DATA);
                }
            }
        }
    }
}
