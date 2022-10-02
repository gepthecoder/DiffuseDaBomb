using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HackingItemData
{
    public ClickableType SelectedType;
    public Transform Position;
    public CodeEncryptionType CodeEncryption;

    public HackingItemData() { }
    public HackingItemData(CodeEncryptionType codeEncryption) { CodeEncryption = codeEncryption; }
    public HackingItemData(ClickableType selectedType, Transform position) { SelectedType = selectedType; Position = position; }
}

public class PlantBombHackingController : MonoBehaviour
{
    [SerializeField] private PlantBombActionHandler m_PlantBombActionHandler;

    private Dictionary<CodeEncryptionType, bool> m_TaskListInfo = new Dictionary<CodeEncryptionType, bool>()
    { { CodeEncryptionType.KeyboardEncryption, false }, { CodeEncryptionType.KeyPadEncryption, false } };

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

    public void OnItemHacked(HackingItemData DATA)
    {
        m_TaskListInfo[DATA.CodeEncryption] = true;
        m_PlantBombActionHandler.DeinitKeyboardView();

        if (TaskDone())
        {
            // TODO: EMIT EVENT to GAME MANAGER -> DEFUSING
        }

    }

    private bool TaskDone()
    {
        foreach (var task in m_TaskListInfo)
        {
            if(task.Value == false)
            {
                return false;
            }
        }

        return true;
    }
}
