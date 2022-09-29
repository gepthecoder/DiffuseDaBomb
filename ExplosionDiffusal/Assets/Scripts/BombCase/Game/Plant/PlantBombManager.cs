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

    private bool m_CanLoopLightEffect = false;
    private const float m_EffectTime = 2f;

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
                m_CanLoopLightEffect = true;
                StartCoroutine(LightShowEffect());
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
        Debug.Log($"Plant Bomb Manager: TurnOnLightSmooth: {on}");

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
}
