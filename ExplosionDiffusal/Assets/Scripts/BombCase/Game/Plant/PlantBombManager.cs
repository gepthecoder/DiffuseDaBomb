using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum PlantBombState { Start, Hacking, Success, }

public class PlantBombManager : MonoBehaviour
{
    [SerializeField] private List<Light> m_CircuitLights;
    [SerializeField] private List<Highlighter> m_HighlightedObjects;

    private PlantBombState i_CurrentState = PlantBombState.Start;

    [HideInInspector] public bool loopLighting = false;

    private void Start()
    {
        TurnOnLightSmooth(false);
        HighlightElements(false);
    }

    public void TriggerPlantBehaviour(PlantBombState state)
    {
        i_CurrentState = state;

        switch (state)
        {
            case PlantBombState.Start:
                loopLighting = true;
                TurnOnLightSmooth(true);
                HighlightElements(true);
                break;
            case PlantBombState.Hacking:
                break;
            case PlantBombState.Success:
                break;
            default:
                break;
        }
    }

    private void TurnOnLightSmooth(bool on)
    {
        if (!loopLighting)
            return;

        foreach (var light in m_CircuitLights)
        {
            light.DOIntensity(on ? 2.5f : 1f, 2.5f).OnComplete(() => 
            {
                TurnOnLightSmooth(!on);
            });
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
}
