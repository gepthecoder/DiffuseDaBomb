using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBombActionHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_3dKeyboard;

    [SerializeField] private PlantBombHackingController m_HackingController;
    [SerializeField] private CameraManager m_CameraManager;
    [SerializeField] private UiManager m_UiManager;

    private void Start()
    {
        m_HackingController.OnHackingItemSelectedEvent.AddListener(OnHackingItemSelected);
    }

    private void OnDestroy()
    {
        m_HackingController.OnHackingItemSelectedEvent.RemoveListener(OnHackingItemSelected);
    }

    private void OnHackingItemSelected(HackingItemData data)
    {
        m_CameraManager.ZoomInOutOfTarget(data.Position, () => {
            m_UiManager.FadeInOutScreen(.77f);
        }, InitKeyboardView);
    }

    private void InitKeyboardView()
    {
        m_UiManager.EnableKeyBoardUI();
        m_3dKeyboard.SetActive(false);
    }

    public void DeinitKeyboardView()
    {
        m_3dKeyboard.SetActive(true);
    }
}
