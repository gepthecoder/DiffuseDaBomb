using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HackingItemData
{
    public ClickableType SelectedType;
    public Transform Position;
    public CodeEncryptionType CodeEncryption;

    public bool CloseHackingItemSuccess;

    public HackingItemData(CodeEncryptionType codeEncryption) { CodeEncryption = codeEncryption; }
    public HackingItemData(CodeEncryptionType codeEncryption, bool closeHackingItemSuccess) 
    { CodeEncryption = codeEncryption; CloseHackingItemSuccess = closeHackingItemSuccess; }

    public HackingItemData(ClickableType selectedType, Transform position) { SelectedType = selectedType; Position = position; }
}

public class PlantBombHackingController : MonoBehaviour
{
    [SerializeField] private PlantBombActionHandler m_PlantBombActionHandler;

    private Dictionary<CodeEncryptionType, bool> m_TaskListInfo = new Dictionary<CodeEncryptionType, bool>()
    { { CodeEncryptionType.KeyboardEncryption, false }, { CodeEncryptionType.KeyPadEncryption, false } };

    private ClickableType m_CurrentSelected = ClickableType.None;

    [HideInInspector] public UnityEvent<HackingItemData> OnHackingItemSelectedEvent = new UnityEvent<HackingItemData>();
    [HideInInspector] public UnityEvent<HackingItemData> OnItemHackedEvent = new UnityEvent<HackingItemData>();
    [HideInInspector] public UnityEvent<HackingItemData> OnAllItemsHackedEvent = new UnityEvent<HackingItemData>();

    private void Awake()
    {
        if (OnHackingItemSelectedEvent == null)
        {
            OnHackingItemSelectedEvent = new UnityEvent<HackingItemData>();
        }

        if (OnItemHackedEvent == null)
        {
            OnItemHackedEvent = new UnityEvent<HackingItemData>();
        }

        m_PlantBombActionHandler.OnEncryptorCloseEvent.AddListener((data) => {
            m_CurrentSelected = ClickableType.None;
        });
    }

    private void OnDestroy()
    {
        m_PlantBombActionHandler.OnEncryptorCloseEvent.RemoveAllListeners();
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

        Deinit3dViews(DATA.CodeEncryption);
        m_PlantBombActionHandler.ActivateBombEffect(false, DATA.CodeEncryption);

        if (TaskDone())
        {
            // TODO: EMIT EVENT to GAME MANAGER -> DEFUSING
            OnAllItemsHackedEvent?.Invoke(DATA);
            PlayButtonPressedSFX(AudioEffect.BombsPlanted);
        }
        else
        {
            m_CurrentSelected = ClickableType.None;
            OnItemHackedEvent?.Invoke(DATA);
            PlayButtonPressedSFX(AudioEffect.Success);
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
    private void PlayButtonPressedSFX(AudioEffect fx)
    {
        AudioManager.INSTANCE.PlayButtonPressedSFX(fx);
    }

    private void Deinit3dViews(CodeEncryptionType type)
    {
        switch (type)
        {
            case CodeEncryptionType.KeyboardEncryption:
                m_PlantBombActionHandler.DeinitKeyboardView();
                break;
            case CodeEncryptionType.KeyPadEncryption:
                m_PlantBombActionHandler.DeinitKeypadView();
                break;
            default:
                break;
        }
    }
}
