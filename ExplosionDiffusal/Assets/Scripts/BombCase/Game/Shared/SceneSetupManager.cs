using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneType { Planting, Defusing, Waiting, }

public class SceneSetupManager : MonoBehaviour
{
    [SerializeField] private GameObject m_BombCase;
    [SerializeField] private GameObject m_Curcuit;

    public void SetupScene(SceneType type)
    {
        switch (type)
        {
            case SceneType.Planting:
                m_BombCase.SetActive(false);
                m_Curcuit.SetActive(true);
                break;
            case SceneType.Defusing:
                break;
            case SceneType.Waiting:
                break;
            default:
                break;
        }
    }
}
