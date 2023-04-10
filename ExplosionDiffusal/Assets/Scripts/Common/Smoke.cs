using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_SmokesList;

    float m_FastSimullation = 5f;
    float m_StartAlpha = .5f;

    public void SmokeIt(bool smoke)
    {
        ResetSmokeAlpha();

        m_SmokesList.ForEach((particle) => {
            ParticleSystem.MainModule settings = particle.main;
            settings.simulationSpeed = 1f;

            if (smoke) { particle.Play(); }
            else particle.Stop();
        });
    }

    public void SetSmokeAlpha(float value)
    {
        print($"SetSmokeAlpha : {value}");

        float DIVIDED = value / 2;
        float INVERSED = Mathf.InverseLerp(.5f, 0f, DIVIDED);
        float alpha = INVERSED / 2;

        m_SmokesList.ForEach((smoke) => {
            ParticleSystem.MainModule settings = smoke.main;
            settings.simulationSpeed = alpha >= .49f ? 1f : m_FastSimullation;

            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(
                settings.startColor.color.r,
                settings.startColor.color.g,
                settings.startColor.color.b,
                alpha
            ));
        });
    }

    public void ResetSmokeAlpha()
    {
        m_SmokesList.ForEach((smoke) => {
            ParticleSystem.MainModule settings = smoke.main;

            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(
               settings.startColor.color.r,
               settings.startColor.color.g,
               settings.startColor.color.b,
               m_StartAlpha
           ));
        });
    }
}
