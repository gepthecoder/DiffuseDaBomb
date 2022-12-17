using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClockMotionObject
{
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

public class ClockMotionController : MonoBehaviour
{
    [SerializeField] private ClockMotionObject m_ClockMotionObject;

    public void EnableClockMotion()
    {
        m_ClockMotionObject.EnableClockMotion();
    }
    public void DisableClockMotion()
    {
        m_ClockMotionObject.DisableClockMotion();
    }
}
