using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightType { PlasticBomb, }
public enum LightAction { Trip, On, Off, }

[System.Serializable]
public class LightObjectAnimator
{
    public LightType _Type;
    [SerializeField] private Animator _Animator;

    public void PlayAnimation(LightAction action)
    {
        switch (action)
        {
            case LightAction.Trip:
                _Animator.Play("trip");
                break;
            case LightAction.On:
                _Animator.Play("idle");
                break;
            case LightAction.Off:
                _Animator.Play("off");
                break;
            default:
                break;
        }
    }

}

public class LightsController : MonoBehaviour
{
    [SerializeField] private List<LightObjectAnimator> LightAnimators = new List<LightObjectAnimator>();

    public void PlayLightAnimator(LightType type, LightAction action)
    {
        LightAnimators.ForEach((lightSource) => { 
            if (lightSource._Type == type) {
                lightSource.PlayAnimation(action);
            } 
        });
    }
}
