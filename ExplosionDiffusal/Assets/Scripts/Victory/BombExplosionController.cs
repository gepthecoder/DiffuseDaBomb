using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BombExplosionController : MonoBehaviour
{
    [Header("Bomb Explosive")]
    [SerializeField] private Explosive m_Explosive;
    [SerializeField] private List<Transform> m_ExplosionPositions;
    [Header("Bomb AfterMath")]
    [SerializeField] private List<GameObject> m_AfterMathFlyingObjects;
    [SerializeField] private List<Transform> m_FlyingObjectsPaths;

    private List<GameObject> m_TempExplosionsList = new List<GameObject>();
    public void ExplodeBomb(Action action)
    {
        print("BOOOOOOOM");
        action();
        StartCoroutine(ExplodeBombSequence());
    }

    private IEnumerator ExplodeBombSequence()
    {
        // SPAWN EXPLOSIVES
        m_ExplosionPositions.ForEach((pos) => {
            GameObject explosive = Instantiate(m_Explosive.gameObject, pos.localPosition, Quaternion.identity);
            explosive.transform.SetParent(pos);
            explosive.transform.localPosition = Vector3.zero;

            m_TempExplosionsList.Add(explosive);

            // TRIGGER EXPLOSIVE
            explosive.GetComponent<Explosive>().TriggerBombExplosion();
        });


        // BOMB AFTERMATH FOR BOMBCASE
        foreach (var item in m_AfterMathFlyingObjects)
        {
            item.SetActive(true);
            item.transform.DOJump(m_FlyingObjectsPaths[UnityEngine .Random.Range(0, m_FlyingObjectsPaths.Count)].position, 1, 1, 1f);

            yield return new WaitForSeconds(.01f);
        }
    }
}
