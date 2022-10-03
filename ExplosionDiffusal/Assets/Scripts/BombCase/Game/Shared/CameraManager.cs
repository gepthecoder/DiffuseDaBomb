using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera m_MainCam;

    private float m_InitialFieldOfView = 60f;
    private float m_ItemSelectedFieldOfView = 30f;

    private Vector3 m_InitalCameraPosition = new Vector3(.14f, 7.78f, -5.61f);
    private Vector3 m_InitalCameraRotation = new Vector3(55f, 0, 0);

    private void Start()
    {
        InitCamera();
    }

    private void InitCamera()
    {
        m_MainCam.transform.localPosition = m_InitalCameraPosition;
        m_MainCam.transform.localEulerAngles = m_InitalCameraRotation;
    }

    public void ZoomInToTarget()
    {
        m_MainCam.DOFieldOfView(m_MainCam.nearClipPlane, 1f).SetEase(Ease.InSine);
    }

    public void ZoomInOutOfTarget(Transform target, Action callback = null, Action callback1 = null)
    {
        Debug.Log("ZoomInOutOfTarget");

        m_MainCam.transform.DOMove(new Vector3(target.position.x, m_MainCam.transform.position.y, target.position.z), .5f)
            .SetEase(Ease.InOutBack)
            .OnComplete((() =>
            {
                callback();
                m_MainCam.DOFieldOfView(m_MainCam.nearClipPlane, .5f).SetEase(Ease.InSine)
                .OnComplete((() => {
                    callback1();
                    ZoomOutOfTarget(true); 
                }));
            }));
    }

    public void ZoomOutOfTarget(bool seq = false)
    {
        m_MainCam.transform.localEulerAngles = new Vector3(90, 0, 0);

        m_MainCam.DOFieldOfView(!seq ? m_InitialFieldOfView : m_ItemSelectedFieldOfView, .77f).SetEase(Ease.InSine);

        if(!seq)
            m_MainCam.transform.DOLocalMove(new Vector3(1, 12, 1), 1f);
    }
}
