using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparks : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> m_Sparks;
    [Header("Spark System Generator")]
    [SerializeField] private SparkAndSmoke m_SparkAndSmoke;
    [SerializeField] private List<Transform> m_TempPositions;

    private bool m_CanIgniteSparks = true;

    private float m_IngniteSpeed;
    private const float m_IngniteSpeedMin = .3f;
    private const float m_IngniteSpeedMax = 2f;

    private List<GameObject> m_Bin = new List<GameObject>();

    public void ResetSparks()
    {
        EnableAllSparks(false);
        m_CanIgniteSparks = true;

        StopAllCoroutines();

        if(m_Bin.Count > 0)
        {
            m_Bin.ForEach((spark) => {
                Destroy(spark);
            });
        }
    }

    // SPARKS
    public void EnableAllSparks(bool enable)
    {
        m_Sparks.ForEach((spark) => {
            spark.gameObject.SetActive(enable);
            if (enable) { spark.Play(); }
            else { spark.Stop(); }
        });       
    }

    public IEnumerator EnableSparksWDelay(bool enable, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        EnableAllSparks(enable);
    }
    //

    // Spark System Generator

    public void IgniteSparkSystem()
    {
        StartCoroutine(SparkSystem());
    }

    private IEnumerator SparkSystem()
    {
        m_IngniteSpeed = m_IngniteSpeedMin;

        // loop until condition
        while (m_CanIgniteSparks)
        {
            // get random position & VFX ref
            Transform pos = m_TempPositions[Random.Range(0, m_TempPositions.Count)];
            SparkAndSmoke VFX = Instantiate(m_SparkAndSmoke.gameObject, pos.position, Quaternion.identity)
                .GetComponent<SparkAndSmoke>();

            VFX.transform.SetParent(pos);
            VFX.StartSeq();

            m_Bin.Add(VFX.gameObject);

            yield return new WaitForSeconds(m_IngniteSpeed);
        }

        yield return null;
    }

    public void SetIgniteSpeed(float speed) // .3f - 2f
    {
        m_IngniteSpeed = Mathf.Lerp(m_IngniteSpeedMin, m_IngniteSpeedMax, speed);
    }

    //
}
