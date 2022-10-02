using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CodeManager : MonoBehaviour
{
    public static CodeManager instance;

    [SerializeField] private List<Code> codes = new List<Code>();

    [HideInInspector] public UnityEvent OnSetCodeEvent = new UnityEvent();

    private void Awake()
    {
        instance = this;
    }

    public void SetCode(CodeEncryptionType encryption, string pass)
    {
        Code code = GetCodeByEncryption(encryption);
        code.SetBombCode(pass);

        OnSetCodeEvent?.Invoke();
    }

    public void ValidateCode(CodeEncryptionType encryption, string pass)
    {
        Code code = GetCodeByEncryption(encryption);
        bool success = code.DoCodeValidation(pass);

        if(success)
        {
            // TODO EMIT EVENT
        }
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
