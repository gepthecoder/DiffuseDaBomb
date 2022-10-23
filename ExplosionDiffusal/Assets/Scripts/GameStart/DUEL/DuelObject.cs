using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;

public enum DuelObjectType { Attacker, Defender, None }

public class DuelObject : MonoBehaviour, IPointerClickHandler
{
    public DuelObjectType ID;
    [HideInInspector] public UnityEvent<DuelObjectType> OnDuelObjectSelected = new UnityEvent<DuelObjectType>();

    public void OnSelected()
    {
        transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .2f);
    }

    public void OnDeSelected()
    {
        transform.DOScale(new Vector3(.9f, .9f, .9f), .2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnDuelObjectSelected?.Invoke(ID);
    }
}
