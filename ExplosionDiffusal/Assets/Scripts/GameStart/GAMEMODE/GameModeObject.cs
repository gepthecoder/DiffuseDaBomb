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
        if(Type == GameModeType.SpecialEdition)
        {
            // wobble lock
            LockAnime?.Play("wobbleLock");
        }
        transform.DOScale(Vector3.one, .5f);
    }

    public override void OnDeSelected()
    {
        transform.DOScale(new Vector3(.77f, .77f, .77f), .5f);
    }

}
