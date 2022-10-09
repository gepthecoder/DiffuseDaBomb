using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Lights : MonoBehaviour
{
    [SerializeField] private Light m_KeyboardLight = new Light();
    [SerializeField] private Light m_KeypadLight = new Light();

    [SerializeField] private List<Light> m_PlascticBombLights = new List<Light>();
    [SerializeField] private List<Light> m_TnTLights = new List<Light>();

    private bool m_CanLoopLightEffect = false;
    private bool m_CanLoopLightEffectByType = false;
    private const float m_EffectTime = 2f;

    public void EnableKeyboardLight(bool enable)
    {
        m_KeyboardLight.DOIntensity(enable ? 2.5f : 0.15f, 1.5f);
    }

    public void EnableKeypadLight(bool enable)
    {
        m_KeypadLight.DOIntensity(enable ? 2.5f : 0.15f, 1.5f);
    }
  
    public void LightUpBombs(bool enable, CodeEncryptionType type)
    {
        StopLightEffect();

        switch (type)
        {
            case CodeEncryptionType.KeyPadEncryption:
                EnableLights(enable, m_TnTLights);
                break;
            case CodeEncryptionType.KeyboardEncryption:
                EnableLights(enable, m_PlascticBombLights);
                break;
            default:
                break;
        }
    }

    public void LightEffect()
    {
        m_CanLoopLightEffect = true;
        StartCoroutine(LightShowEffect());
    }

    public void LightEffectByType(CodeEncryptionType type)
    {
        m_CanLoopLightEffectByType = true;
        StartCoroutine(LightShowEffectByType(type));
    }

    public void StopLightEffect()
    {
        m_CanLoopLightEffect = false;
        StopAllCoroutines();
    }

    private void EnableLights(bool enable, List<Light> lights)
    {
        foreach (var light in lights)
        {
            light.DOIntensity(enable ? 10f : .15f, 1.5f);
        }
    }

    public void TurnOnLightSmooth(bool on)
    {
        EnableKeyboardLight(on);
        EnableKeypadLight(on);
    }

    public void TurnOnLightSmoothByType(bool on, CodeEncryptionType type)
    {
        if(type == CodeEncryptionType.KeyboardEncryption) {
            EnableKeyboardLight(on);
        }
        else if(type == CodeEncryptionType.KeyPadEncryption)
        {
            EnableKeypadLight(on);
        }
    }

    private IEnumerator LightShowEffect()
    {
        bool on = true;
        while (m_CanLoopLightEffect)
        {
            TurnOnLightSmooth(on);
            on = !on;

            yield return new WaitForSeconds(m_EffectTime);
        }
    }

    private IEnumerator LightShowEffectByType(CodeEncryptionType type)
    {
        bool on = true;
        while (m_CanLoopLightEffectByType)
        {
            TurnOnLightSmoothByType(on, type);
            on = !on;

            yield return new WaitForSeconds(m_EffectTime);
        }
    }

}
