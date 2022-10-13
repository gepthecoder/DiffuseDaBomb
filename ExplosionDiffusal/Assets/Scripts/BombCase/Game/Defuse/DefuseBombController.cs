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

    private Dictionary<CodeEncryptionType, bool> m_TaskListInfo = new Dictionary<CodeEncryptionType, bool>()
            { { CodeEncryptionType.KeyboardEncryption, false }, { CodeEncryptionType.KeyPadEncryption, false } };

    private ClickableType m_CurrentSelected = ClickableType.None;

    [HideInInspector] public UnityEvent<HackingItemData> OnItemHackedEvent = new UnityEvent<HackingItemData>();


    public void OnHackingItemSelected(HackingItemData data)
    {
        if (m_CurrentSelected != ClickableType.None)
            return;

        m_CurrentSelected = data.SelectedType;

        AudioManager.INSTANCE.PlayButtonPressedSFX(AudioEffect.Defuse);

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
            // TODO: EMIT EVENT to GAME MANAGER -> VICTORY
            AudioManager.INSTANCE.PlayButtonPressedSFX(AudioEffect.BombsDefused);
        }
        else
        {
            m_CurrentSelected = ClickableType.None;
            OnItemHackedEvent?.Invoke(data);
            AudioManager.INSTANCE.PlayButtonPressedSFX(AudioEffect.BombsDefused);
        }
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
