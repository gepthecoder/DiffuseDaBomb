using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private Camera m_MainCam;

    private float m_InitialFieldOfView = 60f;
    private float m_ItemSelectedFieldOfView = 30f;

    private Vector3 m_InitalCameraPosition = new Vector3(.14f, 7.78f, -5.61f);
    private Vector3 m_InitalCameraRotation = new Vector3(55f, 0, 0);

    private void Awake()
    {
        Instance = this;
    }

    public (Vector3, Vector3) GetInitalCameraPositionAndRotation()
    {
        return (m_InitalCameraPosition, m_InitalCameraRotation);
    }

    public void ZoomInToTarget()
    {
        m_MainCam.DOFieldOfView(m_MainCam.nearClipPlane + 4, 1f).SetEase(Ease.InOutBack);
    }

    public void ZoomInOutOfTarget(Transform target, Action callback = null, Action callback1 = null)
    {
        m_MainCam.transform.DOMove(new Vector3(target.position.x, m_MainCam.transform.position.y, target.position.z), .5f)
            .SetEase(Ease.InOutBack)
            .OnComplete((() =>
            {
                callback();
                m_MainCam.DOFieldOfView(m_MainCam.nearClipPlane, .4f).SetEase(Ease.InSine)
                .OnComplete((() => {
                    callback1();
                    ZoomOutOfTarget(true); 
                }));
            }));
    }

    public void ZoomOutOfTarget(bool seq = false)
    {
        m_MainCam.transform.localEulerAngles = new Vector3(90, 0, 0);

        m_MainCam.DOFieldOfView(!seq ? m_InitialFieldOfView : m_ItemSelectedFieldOfView, !seq ? .77f : .4f).SetEase(Ease.InSine);

        if(!seq)
            m_MainCam.transform.DOLocalMove(new Vector3(1, 12, 1), 1f);
    }

    public void ZoomInOutToBombCaseView(Action setupScene, Action closeBombCase)
    {
        m_MainCam.DOFieldOfView(m_MainCam.nearClipPlane, .77f).SetEase(Ease.InSine)
            .OnComplete(() => {
                m_MainCam.transform.localEulerAngles = m_InitalCameraRotation;
                setupScene();
                m_MainCam.transform.DOLocalMove(m_InitalCameraPosition, .77f);
                m_MainCam.DOFieldOfView(m_InitialFieldOfView, .77f).SetEase(Ease.InSine)
                .OnComplete(() => {
                    closeBombCase();
                });
            });

    }

    public void ShakeCamera(Action onCamSeqFinnished)
    {
        m_MainCam.DOShakePosition(3f, 1.5f, 5).OnComplete(() => {
            onCamSeqFinnished();
        });
    }
}
