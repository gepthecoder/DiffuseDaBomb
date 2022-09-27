using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlantBombState { Start, Hacking, Success, }

public class PlantBombManager : MonoBehaviour
{
    private PlantBombState i_CurrentState = PlantBombState.Start; 
    public void TriggerPlantBehaviour(PlantBombState state)
    {
        i_CurrentState = state;

        switch (state)
        {
            case PlantBombState.Start:
                break;
            case PlantBombState.Hacking:
                break;
            case PlantBombState.Success:
                break;
            default:
                break;
        }
    }
}
