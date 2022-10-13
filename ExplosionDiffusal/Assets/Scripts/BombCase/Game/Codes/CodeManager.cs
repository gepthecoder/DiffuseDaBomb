using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CodeManager : MonoBehaviour
{
    public static CodeManager instance;

    [SerializeField] private List<Code> codes = new List<Code>();

    [HideInInspector] public UnityEvent<CodeEncryptionType> OnSetCodeEvent = new UnityEvent<CodeEncryptionType>();
    [HideInInspector] public UnityEvent<CodeEncryptionType> OnValidateCodeEvent = new UnityEvent<CodeEncryptionType>();

    private void Awake()
    {
        instance = this;
    }

    public string TryGetCode(CodeEncryptionType encryption)
    {
        return GetCodeByEncryption(encryption).GetBombCode();
    }

    public void SetCode(CodeEncryptionType encryption, string pass)
    {
        Code code = GetCodeByEncryption(encryption);
        code.SetBombCode(pass);

        OnSetCodeEvent?.Invoke(encryption);
    }

    public bool ValidateCode(CodeEncryptionType encryption, string pass)
    {
        Code code = GetCodeByEncryption(encryption);
        bool success = code.DoCodeValidation(pass);

        if(success)
        {
            OnValidateCodeEvent?.Invoke(encryption);
            return true;
        } else
        {
            Debug.Log("<color=red>CodeValidation Failed</color><color=black>!</color>");
        }

        return false;
    }

    private Code GetCodeByEncryption(CodeEncryptionType encryption)
    {
        foreach (var code in codes)
        {
            if (code.EncryptionType == encryption)
                return code;
        }

        return null;
    }
}
