using System;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine;

public class GameModeObject : GameModeObjectBase
{
    private bool m_CanInteract = false;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(!m_CanInteract) {
            return;
        }

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

    public void SetInteractability(bool canInteract)
    {
        m_CanInteract = canInteract;
    }
}
