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

    public Transform AttackerTextObj;
    public Transform DefenderTextObj;

    private string scoreLimit;

    public void InitMainTeamHolder(MainTeamHolderData data)
    {
        scoreLimit = data.ScoreLimit;

        LogoImage.sprite = data.LogoSprite;
        ScoreText.text = $"{data.Score}/{data.ScoreLimit}";
        TeamNameText.text = $"{data.TeamName}";
        TeamCountText.text = $"{data.TeamCount}";
    }

    internal void Show(bool show)
    {
        if(show)
        {
            DoDoScaleIn_Emblem();
            DoDoScaleIn_TeamName();
            DoDoScaleIn_ScoreText();
            DoDoScaleIn_TeamCount();
            DoDoScaleIn_AttckDefObjects();

        }
        else
        {
            DoDoScaleOut_Emblem();
            DoDoScaleOut_TeamName();
            DoDoScaleOut_ScoreText();
            DoDoScaleOut_TeamCount();
            DoDoScaleOut_AttckDefObjects();
        }
    }

    // Show
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
    
    internal void DoDoScaleIn_AttckDefObjects()
    {
        AttackerTextObj.DOScale(1.15f, .77f).OnComplete(() => {
            AttackerTextObj.DOScale(1f, .25f);
        }); 
        
        DefenderTextObj.DOScale(1.15f, .77f).OnComplete(() => {
            DefenderTextObj.DOScale(1f, .25f);
        });
    }

    // Hide
    internal void DoDoScaleOut_Emblem()
    {
        LogoImage.transform.DOScale(1.15f, .77f).OnComplete(() => {
            LogoImage.transform.DOScale(0f, .25f);
        });

        LogoShineImage.transform.DOScale(1.15f, .77f).OnComplete(() => {
            LogoShineImage.transform.DOScale(0f, .25f);
        });
    }

    internal void DoDoScaleOut_TeamName()
    {
        TeamNameText.transform.DOScale(1.15f, .77f).OnComplete(() => {
            TeamNameText.transform.DOScale(0f, .25f);
        });
    }

    internal void DoDoScaleOut_ScoreText()
    {
        ScoreText.transform.DOScale(1.15f, .77f).OnComplete(() => {
            ScoreText.transform.DOScale(0f, .25f);
        });
    }

    internal void DoDoScaleOut_TeamCount()
    {
        TeamCountText.transform.DOScale(1.15f, .77f).OnComplete(() => {
            TeamCountText.transform.DOScale(0f, .25f);
        });
    }

    internal void DoDoScaleOut_AttckDefObjects()
    {
        AttackerTextObj.DOScale(1.15f, .77f).OnComplete(() => {
            AttackerTextObj.DOScale(0f, .25f);
        });

        DefenderTextObj.DOScale(1.15f, .77f).OnComplete(() => {
            DefenderTextObj.DOScale(0f, .25f);
        });
    }

    //

    internal void IncreaseScore(int totalScore)
    {
        ScoreText.transform.DOScale(2f, 1f).SetEase(Ease.InExpo).OnComplete(() => {
            ScoreText.text = $"{totalScore}/{scoreLimit}";
            ScoreText.transform.DOScale(1f, .5f).SetEase(Ease.OutExpo);
        });
    }

    internal void SwitchAttackerDefenderObjects() // only happens once per game cycle so its oke #justify
    {
        if(AttackerTextObj.gameObject.activeSelf)
        {
            AttackerTextObj.gameObject.SetActive(false);
            DefenderTextObj.gameObject.SetActive(true);
        } else
        {
            DefenderTextObj.gameObject.SetActive(false);
            AttackerTextObj.gameObject.SetActive(true);
        }
    }
}
