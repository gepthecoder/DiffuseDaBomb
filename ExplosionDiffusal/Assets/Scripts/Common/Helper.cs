using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Helper : MonoBehaviour
{
    public static Helper INSTANCE;

    private void Awake()
    {
        INSTANCE = this;
    }

    public bool IsPointerOverUI()
    {
        PointerEventData eventDataPointer = new PointerEventData(EventSystem.current);
        eventDataPointer.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataPointer, results);
        return results.Count > 0;
    }
}
