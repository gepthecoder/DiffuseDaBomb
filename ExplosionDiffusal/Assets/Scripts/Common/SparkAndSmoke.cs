using System.Collections;
using UnityEngine;

public class SparkAndSmoke : MonoBehaviour
{
    public ParticleSystem _SPARK_;
    public ParticleSystem _SMOKE_;

    public void StartSeq()
    {
        StartCoroutine(Seq());
    }

    private IEnumerator Seq()
    {
        _SPARK_.gameObject.SetActive(true);
        _SPARK_.Play();

        _SMOKE_.gameObject.SetActive(true);
        _SMOKE_.Play();

        yield return new WaitForSeconds(.5f);

        _SPARK_.gameObject.SetActive(false);
        _SPARK_.Stop();

        yield return new WaitForSeconds(5f);

        _SMOKE_.gameObject.SetActive(false);
        _SMOKE_.Stop();

        yield return new WaitForSeconds(.1f);

        Destroy(this);
    }
}
