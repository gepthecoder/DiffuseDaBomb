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

    public MainTeamHolderData() { }
    public MainTeamHolderData(Sprite logo, string score, string teamName) {
        LogoSprite = logo;
        Score = score;
        TeamName = teamName;
    }
}

public class MainTeamHolder : MonoBehaviour
{
    public Image LogoImage;
    public Text ScoreText;
    public Text TeamNameText;

    public void InitMainTeamHolder(MainTeamHolderData data)
    {
        LogoImage.sprite = data.LogoSprite;
        ScoreText.text = $"{data.Score}";
        TeamNameText.text = $"{data.TeamName}";
    }

    internal void DoDoScaleIn()
    {
        transform.DOScale(1.15f, .77f).OnComplete(() => {
            transform.DOScale(1f, .25f);
        });
    }
}
