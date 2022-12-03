using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] private Transform TeamAxisPlaceholder;
    [SerializeField] private Transform TeamAlliesPlaceholder;

    public void InitMainCanvas()
    {
        gameObject.SetActive(true);
        StartCoroutine(WaitAndExecuteTeams());
    }

    private IEnumerator WaitAndExecuteTeams()
    {
        yield return new WaitForSeconds(1);

        TeamAxisPlaceholder.DOScale(1.15f, .77f).OnComplete(() => {
            TeamAxisPlaceholder.DOScale(1f, .25f);
        });

        TeamAlliesPlaceholder.DOScale(1.15f, .77f).OnComplete(() => {
            TeamAlliesPlaceholder.DOScale(1f, .25f);
        });
    }
}
