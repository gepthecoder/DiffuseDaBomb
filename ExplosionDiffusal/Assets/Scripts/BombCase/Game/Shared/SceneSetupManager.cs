using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum SceneType { Planting, Defusing, Waiting, Default, }

public class SceneSetupManager : MonoBehaviour
{
    [SerializeField] private GameObject m_BombCase;
    [SerializeField] private GameObject m_Curcuit;

    [Header("Covers")]
    [SerializeField] private List<GameObject> m_BombCaseCoverUps;

    public void SetupScene(SceneType type)
    {
        switch (type)
        {
            case SceneType.Planting:
                m_BombCase.SetActive(false);
                m_Curcuit.SetActive(true);
                break;
            case SceneType.Defusing:
                m_BombCase.SetActive(false);
                m_Curcuit.SetActive(true);
                break;
            case SceneType.Waiting:
                break;

            case SceneType.Default:
            default:
                m_BombCase.SetActive(true);
                m_Curcuit.SetActive(false);
                break;
        }
    }

    public void EnableBombCoverUps(bool enable)
    {
        m_BombCaseCoverUps.ForEach((OBJ) => { OBJ.SetActive(enable); });
    }
}
