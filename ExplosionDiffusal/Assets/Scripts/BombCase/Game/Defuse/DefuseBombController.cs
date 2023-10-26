using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DefuseBombController : MonoBehaviour
{
    [SerializeField] private UiManager m_UiManager;
    [SerializeField] private CameraManager m_CameraManager;
    [Space(5)]
    [SerializeField] private GameObject m_3dKeyboard;
    [SerializeField] private GameObject m_3dKeypad;
    [Space(5)]
    [SerializeField] private List<GameObject> m_PlasticBombCoverObjects;

    private Dictionary<CodeEncryptionType, bool> m_TaskListInfo = new Dictionary<CodeEncryptionType, bool>()
            { { CodeEncryptionType.KeyboardEncryption, false }, { CodeEncryptionType.KeyPadEncryption, false } };

    private ClickableType m_CurrentSelected = ClickableType.None;

    [HideInInspector] public UnityEvent<HackingItemData> OnItemHackedEvent = new UnityEvent<HackingItemData>();
    [HideInInspector] public UnityEvent<HackingItemData> OnAllItemsHackedEvent = new UnityEvent<HackingItemData>();

    public void OnHackingItemSelected(HackingItemData data)
    {
        if (m_CurrentSelected != ClickableType.None)
            return;

        m_CurrentSelected = data.SelectedType;

        AudioManager.INSTANCE.PlayAudioEffectByType(AudioEffect.Defuse);

        m_CameraManager.ZoomInOutOfTarget(data.Position, () => {
            m_UiManager.FadeInOutScreen(.77f);
        }, () =>
        {
            if (data.SelectedType == ClickableType.Keyboard) { InitKeyboardView(); }
            if (data.SelectedType == ClickableType.Keypad) { InitKeyPadView(); }
        });

    }

    public void OnItemHacked(HackingItemData data)
    {
        m_TaskListInfo[data.CodeEncryption] = true;

        if(TaskDone())
        {
            OnAllItemsHackedEvent?.Invoke(data);
            // reset
            ResetTaskInfo();
        }
        else
        {
            OnItemHackedEvent?.Invoke(data);
        }

        AudioManager.INSTANCE.PlayAudioEffectByType(AudioEffect.BombsDefused);
        m_CurrentSelected = ClickableType.None;

        ActivateBombEffect(true, data.CodeEncryption);
        Deinit3dViews(data.CodeEncryption);
    }

    public void OnHackingItemDeselected()
    {
        m_CurrentSelected = ClickableType.None;
    }

    public void ResetTaskInfo()
    {
        m_TaskListInfo[CodeEncryptionType.KeyboardEncryption] = false;
        m_TaskListInfo[CodeEncryptionType.KeyPadEncryption] = false;
    }

    private bool TaskDone()
    {
        foreach (var task in m_TaskListInfo)
        {
            if (task.Value == false)
            {
                return false;
            }
        }

        return true;
    }

    public void ActivateBombEffect(bool activate, CodeEncryptionType type)
    {
        switch (type)
        {
            case CodeEncryptionType.KeyboardEncryption:

                foreach (var item in m_PlasticBombCoverObjects)
                {
                    item.SetActive(activate);
                }
                break;
            case CodeEncryptionType.KeyPadEncryption:
            default:
                break;
        }

    }

    private void Deinit3dViews(CodeEncryptionType type)
    {
        switch (type)
        {
            case CodeEncryptionType.KeyboardEncryption:
                DeinitKeyboardView();
                break;
            case CodeEncryptionType.KeyPadEncryption:
                DeinitKeypadView();
                break;
            default:
                break;
        }
    }

    private void InitKeyboardView()
    {
        m_UiManager.EnableKeyBoardUI();
        m_3dKeyboard.SetActive(false);
    }

    private void InitKeyPadView()
    {
        m_UiManager.EnableKeyPadUI();
        m_3dKeypad.SetActive(false);
    }

    public void DeinitKeyboardView()
    {
        m_3dKeyboard.SetActive(true);
    }

    public void DeinitKeypadView()
    {
        m_3dKeypad.SetActive(true);
    }
}
