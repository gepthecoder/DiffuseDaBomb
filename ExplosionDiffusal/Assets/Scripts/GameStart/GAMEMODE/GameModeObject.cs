using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine;

public class GameModeObject : GameModeObjectBase
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        OnGameModeSelectedEvent?.Invoke(Type);
    }

    public override void OnSelected()
    {
        transform.DOScale(Vector3.one, .5f);
    }

    public override void OnDeSelected()
    {
        transform.DOScale(new Vector3(.77f, .77f, .77f), .5f);
    }

}
