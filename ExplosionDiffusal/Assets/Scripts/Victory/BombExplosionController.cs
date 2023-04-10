using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class BombExplosionController : MonoBehaviour
{
    public struct AfterMathObjectMap
    {
        public GameObject obj;
        public Vector3 pos;
    }

    [Header("Bomb Explosive")]
    [SerializeField] private Explosive m_Explosive;
    [SerializeField] private List<Transform> m_ExplosionPositions;
    [Header("Bomb AfterMath")]
    [SerializeField] private List<GameObject> m_AfterMathFlyingObjects;
    [SerializeField] private List<Transform> m_FlyingObjectsPaths;
    [SerializeField] private Smoke m_Smoke;

    private List<GameObject> m_TempExplosionsList = new List<GameObject>();

    private List<AfterMathObjectMap> m_AfterMathMapper = new List<AfterMathObjectMap>();

    private void Awake()
    {
        m_AfterMathFlyingObjects.ForEach((obj) => {
            AfterMathObjectMap map;
            map.obj = obj;
            map.pos = obj.transform.localPosition;
            m_AfterMathMapper.Add(map);
        });
    }

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

        yield return new WaitForSeconds(.4f);

        m_Smoke.SmokeIt(true);
    }

    public void ResetAfterMathFlyingObject()
    {
        foreach (var map in m_AfterMathMapper)
        {
            map.obj.transform.localPosition = map.pos;
            map.obj.SetActive(false);
        }
    }
}
