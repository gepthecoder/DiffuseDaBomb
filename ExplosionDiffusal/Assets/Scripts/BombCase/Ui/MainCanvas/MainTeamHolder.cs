using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainTeamHolderData
{
    public Sprite LogoSprite;
    public string Score;
    public string TeamName;
    public string TeamCount;
    public string ScoreLimit;

    public MainTeamHolderData() { }
    public MainTeamHolderData(Sprite logo, string score, string teamName, string teamCount, string scoreLimit) {
        LogoSprite = logo;
        Score = score;
        TeamName = teamName;
        TeamCount = teamCount;
        ScoreLimit = scoreLimit;
    }
}

public class MainTeamHolder : MonoBehaviour
{
    public Image LogoImage;
    public Image LogoShineImage;
    public Text ScoreText;
    public Text TeamNameText;
    public Text TeamCountText;

    private string scoreLimit;

    public void InitMainTeamHolder(MainTeamHolderData data)
    {
        scoreLimit = data.ScoreLimit;

        LogoImage.sprite = data.LogoSprite;
        ScoreText.text = $"{data.Score}/{data.ScoreLimit}";
        TeamNameText.text = $"{data.TeamName}";
        TeamCountText.text = $"{data.TeamCount}";
    }

    internal void DoDoScaleIn_Emblem()
    {
        LogoImage.transform.DOScale(1.15f, .77f).OnComplete(() => {
            LogoImage.transform.DOScale(1f, .25f);
        });

        LogoShineImage.transform.DOScale(1.15f, .77f).OnComplete(() => {
            LogoShineImage.transform.DOScale(1f, .25f);
        });
    }

    internal void DoDoScaleIn_TeamName()
    {
        TeamNameText.transform.DOScale(1.15f, .77f).OnComplete(() => {
            TeamNameText.transform.DOScale(1f, .25f);
        });
    }

    internal void DoDoScaleIn_ScoreText()
    {
        ScoreText.transform.DOScale(1.15f, .77f).OnComplete(() => {
            ScoreText.transform.DOScale(1f, .25f);
        });
    }

    internal void DoDoScaleIn_TeamCount()
    {
        TeamCountText.transform.DOScale(1.15f, .77f).OnComplete(() => {
            TeamCountText.transform.DOScale(1f, .25f);
        });
    }

    internal void IncreaseScore(int totalScore)
    {
        ScoreText.transform.DOScale(2f, 1f).SetEase(Ease.InExpo).OnComplete(() => {
            ScoreText.text = $"{totalScore}/{scoreLimit}";
            ScoreText.transform.DOScale(1f, .5f).SetEase(Ease.OutExpo);
        });
    }
}
