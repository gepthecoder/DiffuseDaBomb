using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum GameModeType { OriginalEdition, SpecialEdition, None }

public class GameModeObjectBase : MonoBehaviour, IPointerClickHandler
{
    public GameModeType Type;

    [HideInInspector] public UnityEvent<GameModeType> OnGameModeSelectedEvent = new UnityEvent<GameModeType>();
    public virtual void OnPointerClick(PointerEventData eventData) {
    }

    public virtual void OnSelected() { }
    public virtual void OnDeSelected() { }
}
