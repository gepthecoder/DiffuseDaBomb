using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HackingItemData
{
    public ClickableType SelectedType;
    public Transform Position;

    public HackingItemData() { }
    public HackingItemData(ClickableType selectedType, Transform position) { SelectedType = selectedType; Position = position; }
}

public class PlantBombHackingController : MonoBehaviour
{
    private Dictionary<ClickableType, bool> m_TaskListInfo = new Dictionary<ClickableType, bool>()
    { { ClickableType.Keyboard, false }, { ClickableType.Keypad, false } };

    private ClickableType m_CurrentSelected = ClickableType.None;

    [HideInInspector] public UnityEvent<HackingItemData> OnHackingItemSelectedEvent = new UnityEvent<HackingItemData>();

    private void Awake()
    {
        if (OnHackingItemSelectedEvent == null)
        {
            OnHackingItemSelectedEvent = new UnityEvent<HackingItemData>();
        }
    }

    public void OnHackingItemSelected(HackingItemData data)
    {
        if (m_CurrentSelected != ClickableType.None)
            return;

        m_CurrentSelected = data.SelectedType;

        OnHackingItemSelectedEvent?.Invoke(data);
    }
}
