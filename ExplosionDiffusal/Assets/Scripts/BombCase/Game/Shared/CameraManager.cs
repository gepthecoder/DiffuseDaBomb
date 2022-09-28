using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera m_MainCam;

    private float m_InitialFieldOfView = 60f;
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
        m_MainCam.DOFieldOfView(m_MainCam.nearClipPlane, 1.5f).SetEase(Ease.InSine);
    }

    public void ZoomOutOfTarget()
    {
        m_MainCam.transform.localEulerAngles = new Vector3(90, 0, 0);

        m_MainCam.DOFieldOfView(m_InitialFieldOfView, 1f).SetEase(Ease.InSine);
        m_MainCam.transform.DOLocalMove(new Vector3(1, 12, 1), 1f);
    }
}
