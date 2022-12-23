using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_BombExplosionParticles;

    public void TriggerBombExplosion()
    {
        m_BombExplosionParticles.ForEach((explosion) => {
            explosion.Play();
        });
    }
}
