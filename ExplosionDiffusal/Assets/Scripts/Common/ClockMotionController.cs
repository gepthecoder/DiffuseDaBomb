using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClockMotionObject
{
    public ClockMotionType Type;

    public Animator m_LongPointerAnime;
    public Animator m_ShortPointerAnime;

    public void EnableClockMotion()
    {
        m_LongPointerAnime.enabled = true;
        m_ShortPointerAnime.enabled = true;
        
        m_LongPointerAnime.Play("long");
        m_ShortPointerAnime.Play("short");
    }

    public void DisableClockMotion()
    {
        m_LongPointerAnime.enabled = false;
        m_ShortPointerAnime.enabled = false;
    }
}

public enum ClockMotionType { BombCaseClock, MultiComplexClock }

public class ClockMotionController : MonoBehaviour
{
    [SerializeField] private List<ClockMotionObject> m_ClockMotionObjects;

    public void EnableClockMotion(ClockMotionType type, bool enable)
    {
        m_ClockMotionObjects.ForEach((clock) =>
        {
            if(clock.Type == type)
            {
                if (enable)
                    clock.EnableClockMotion();
                else
                    clock.DisableClockMotion();
            }
        });
    }
}
