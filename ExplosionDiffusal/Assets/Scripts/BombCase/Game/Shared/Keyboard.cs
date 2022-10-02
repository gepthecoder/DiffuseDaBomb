using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum AudioEffect { Keypress, Success, Denial, }

public class Keyboard : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource m_KeyPressAudio;
    [SerializeField] private AudioSource m_OtherAudio;

    [SerializeField] private AudioClip m_KeyPressClip;
    [SerializeField] private AudioClip m_AccessDeniedClip;
    [SerializeField] private AudioClip m_AccessGrantedClip;

    [Header("Elements")]
    [SerializeField] private InputField m_InputField;
    [SerializeField] private GameObject m_EnterCodeText;

    private string m_CurrentString = "";

    private const int m_MaxCharacters = 7;
    private int m_CharacterCounter = 0;

    private bool m_CanShowEnterCode = true;

    private void Awake()
    {
        m_InputField.characterLimit = m_MaxCharacters;
        m_EnterCodeText.SetActive(m_CanShowEnterCode);
    }

    public void EnableObject(bool enable)
    {
        gameObject.SetActive(enable);
    }

    public void OnKeyButtonPress(string key) 
    {
        Debug.Log(key);
        bool isBackspace = key == "Backspace";
        bool isEnter = key == "Enter";

        if (m_CanShowEnterCode) { m_CanShowEnterCode = false; m_EnterCodeText.SetActive(m_CanShowEnterCode); }

        if (m_CharacterCounter >= m_MaxCharacters && !isBackspace && !isEnter)
        {
            PlayButtonPressedSFX(AudioEffect.Denial);
            return;
        }
        else if(IsSpecialKey(key)) {
            PlayButtonPressedSFX(AudioEffect.Keypress);
            return;
        }

        UpdateInputField(key);
    }

    private bool IsSpecialKey(string key)
    {
        return    key == "LeftShift"        ||
                   key == "LargeShift"      ||
                    key == "Space"          ||
                     key == "Alt"           ||
                      key == "AltGr"        ||
                       key == "Tab"         ||
                        key == "Ctrl"       ||
                         key == "CapsLock";
    }

    private void UpdateInputField(string key)
    {
        Debug.Log("UpdateOutputText: " + key);
        Event fakeEvent;
        switch (key)
        {
            case "Backspace":
                SendBackspace();
                return;
            case "Enter":
                SubmitCode();
                return;
            case "Esc":
                CloseKeyboard();
                return;

            case ",":
                fakeEvent = Event.KeyboardEvent("a");
                fakeEvent.keyCode = KeyCode.Comma;
                fakeEvent.character = key[0];
                break;
            case ".":
                fakeEvent = Event.KeyboardEvent("a");
                fakeEvent.keyCode = KeyCode.Period;
                fakeEvent.character = key[0];
                break;
            case "?":
                fakeEvent = Event.KeyboardEvent("a");
                fakeEvent.keyCode = KeyCode.Question;
                fakeEvent.character = key[0];
                break;
            case "&":
                fakeEvent = Event.KeyboardEvent("a");
                fakeEvent.keyCode = KeyCode.Ampersand;
                fakeEvent.character = key[0];
                break;
            case "^":
                fakeEvent = Event.KeyboardEvent("a");
                fakeEvent.keyCode = KeyCode.Caret;
                fakeEvent.character = key[0];
                break;
            case "%":
                fakeEvent = Event.KeyboardEvent("a");
                fakeEvent.keyCode = KeyCode.Percent;
                fakeEvent.character = key[0];
                break;
            case "#":
                fakeEvent = Event.KeyboardEvent("a");
                fakeEvent.keyCode = KeyCode.Hash;
                fakeEvent.character = key[0];
                break;
           
            default:
                if (key.Length != 1)
                {
                    Debug.LogError("Ignoring spurious multi-character key value: " + key);
                    return;
                }
                fakeEvent = Event.KeyboardEvent(key);
                char keyChar = key[0];
                fakeEvent.character = keyChar;
                if (Char.IsUpper(keyChar))
                {
                    fakeEvent.modifiers |= EventModifiers.Shift;
                }
                break;
        }
        Debug.Log("ProcessEvent: " + fakeEvent.functionKey);

        m_InputField.ProcessEvent(fakeEvent);
        m_InputField.ForceLabelUpdate();

        m_CurrentString += key;
        m_CharacterCounter++;

        PlayButtonPressedSFX(AudioEffect.Keypress);
    }

    private void SendBackspace()
    {
        if (m_CharacterCounter > 0)
        {
            PlayButtonPressedSFX(AudioEffect.Keypress);
            
            m_InputField.ProcessEvent(Event.KeyboardEvent("backspace"));
            m_InputField.ForceLabelUpdate();

            m_CharacterCounter -= 1;
            m_CurrentString = m_CurrentString.Substring(0, m_CurrentString.Length - 1);
            Debug.Log("ProcessEvent: backspace charCount: " + m_CharacterCounter);
        }
        else
        {
            m_CanShowEnterCode = true; m_EnterCodeText.SetActive(m_CanShowEnterCode);
            PlayButtonPressedSFX(AudioEffect.Denial);
        }
    }

    private void PlayButtonPressedSFX(AudioEffect effectType)
    {
        switch (effectType)
        {
            case AudioEffect.Keypress:
                m_KeyPressAudio.PlayOneShot(m_KeyPressClip);
                break;
            case AudioEffect.Success:
                m_OtherAudio.PlayOneShot(m_AccessGrantedClip);
                break;
            case AudioEffect.Denial:
                m_OtherAudio.PlayOneShot(m_AccessDeniedClip);
                break;
            default:
                break;
        }
    }

    private void CloseKeyboard()
    {
        EnableObject(false);
    }

    private void SubmitCode()
    {
        Debug.Log("SubmitCode: " + m_CurrentString);
        if(m_CurrentString.Length != m_MaxCharacters)
        {
            Debug.Log("SubmitCode Denial ");

            PlayButtonPressedSFX(AudioEffect.Denial);
            return;
        }

        CodeManager.instance.SetCode(CodeEncryptionType.KeyboardEncryption, m_CurrentString);

        PlayButtonPressedSFX(AudioEffect.Success);
        CloseKeyboard();
    }
}
