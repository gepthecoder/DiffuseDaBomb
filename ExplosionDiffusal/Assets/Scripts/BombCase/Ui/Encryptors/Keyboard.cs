using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class Keyboard : Encryptor
{
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
        if(enable)
        {
            gameObject.SetActive(true);
            gameObject.transform.DOScale(Vector3.one, 1f)
                .SetEase(Ease.InOutCubic);
        } else
        {
            gameObject.SetActive(false);
            gameObject.transform.localScale = Vector3.zero;
        }
    }

    override public void OnKeyButtonPress(string key) 
    {
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

    private void PlayButtonPressedSFX(AudioEffect fx)
    {
        AudioManager.INSTANCE.PlayButtonPressedSFX(fx);
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
        //Debug.Log("UpdateOutputText: " + key);
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
                CloseEncryptor();
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
        //Debug.Log("ProcessEvent: " + fakeEvent.functionKey);

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
            //Debug.Log("ProcessEvent: backspace charCount: " + m_CharacterCounter);
        }
        else
        {
            m_CanShowEnterCode = true; m_EnterCodeText.SetActive(m_CanShowEnterCode);
            PlayButtonPressedSFX(AudioEffect.Denial);
        }
    }

  
    public override void CloseEncryptor()
    {
        OnEncryptorClose?.Invoke(new HackingItemData(CodeEncryptionType.KeyboardEncryption));
    }

    private void SubmitCode()
    {
        Debug.Log("<color=gold>SubmitCode</color>: " + m_CurrentString);
        if(m_CurrentString.Length != m_MaxCharacters)
        {
            Debug.Log("SubmitCode Denial");
            PlayButtonPressedSFX(AudioEffect.Denial);
            return;
        }

        if(currentGameState == GameState.Planting)
        {
            CodeManager.instance.SetCode(CodeEncryptionType.KeyboardEncryption, m_CurrentString);
        } else if(currentGameState == GameState.Defusing)
        {
            CodeManager.instance.ValidateCode(CodeEncryptionType.KeyboardEncryption, m_CurrentString);
        }

        EnableObject(false);
    }

    public void ClearCode()
    {
        m_CurrentString = "";
        m_CharacterCounter = 0;
        m_InputField.clearText();
        m_CanShowEnterCode = true; m_EnterCodeText.SetActive(m_CanShowEnterCode);
    }

    public void InitializeBombDefusalCodes()
    {
        BombDefusalCodesUi.SetActive(true);

        var codeText = BombDefusalCodesUi.GetComponentInChildren<Text>();
        string code = CodeManager.instance.TryGetCode(CodeEncryptionType.KeyboardEncryption);
        codeText.text = code;
    }

}
