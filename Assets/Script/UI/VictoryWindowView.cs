using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Solarmax;






public class VictoryWindowView : BaseWindow
{
    public UIEventListener eventListener;
    public GameObject nextButton;
    public GameObject returnButton;
    public GameObject friendButton;
    public GameObject allButton;
    public GameObject friendButtonLine;
    public GameObject allButtonLine;
    public GameObject friendTemplate;
    public GameObject limitGo;
    public GameObject localPlayer;

    public GameObject threeStars;
    public GameObject fourStars;
    public GameObject twoStars;
    public GameObject oneStarInFour;
    public GameObject oneStarInThree;
    public GameObject twoStarInFour;
    public GameObject threeStarInFour;
    public GameObject twoStarInThree;


    public GameObject awardPower;
    public GameObject awardMoney;
    public GameObject scoreNum;

    public GameObject backButton;

    public GameObject moneyGo;
    public GameObject powerGo;
    public GameObject leftStar;

    private Team winTeam;

    private int starNum  = 1;
    private string selectedCfgId;


    public override bool Init ()
	{
        base.Init();
        UIEventListener.Get(returnButton).onClick = OnReturnButtonClick;
        UIEventListener.Get(nextButton).onClick = OnNextButtonClick;
        UIEventListener.Get(backButton).onClick = OnReturnButtonClick;
        //RegisterEvent(EventId.VictioryWindowViewShow);
        return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
        ShowTestFriend();
        SetPlayerBaseInfo();
    }

	public override void OnHide ()
	{
        BattleSystem.Instance.Reset();
    }

	
	public void OnCloseClick ()
	{
        //UISystem.Get().HideAllWindow();
        //UISystem.Get().ShowWindow("LobbyWindowView");
        //EventSystem.Instance.FireEvent(EventId.UpdateChapterWindow, 0 );
    }


    public void ShowTestFriend()
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject go = NGUITools.AddChild(limitGo, friendTemplate);
            go.SetActive(true);
        }
        limitGo.GetComponent<UIGrid>().repositionNow = true;
        
    }

    public void OnReturnButtonClick(GameObject go)
    {
        //UISystem.Instance.HideAllWindow();
        //UISystem.Instance.ShowWindow ("LobbyWindowView");
        //EventSystem.Instance.FireEvent(EventId.UpdateChapterWindow);
    }

    public void OnNextButtonClick(GameObject go)
    {
        //UISystem.Instance.HideAllWindow();
        //UISystem.Instance.ShowWindow("LobbyWindowView");
        //EventSystem.Instance.FireEvent(EventId.UpdateChapterWindow);
    }

    public override void OnUIEventHandler(EventId eventId, params object[] args)
    {
        //if(eventId == EventId.VictioryWindowViewShow)
        //{
        //    string selectLevelId = args[0] as string;
        //    selectedCfgId = selectLevelId;
        //    Debug.Log("select level id ************************************:  " + selectLevelId);
        //    LevelConfig cfg = LevelConfigConfigProvider.Instance.GetData(selectLevelId);
        //    awardPower.GetComponent<UILabel>().text = "x " + cfg.awardPower;
        //    awardMoney.GetComponent<UILabel>().text = "x " + cfg.awardGold;
        //    winTeam = args[1] as Team;
        //    ShowStarBehavior(winTeam, cfg);
        //    ShowScoreNum(winTeam, cfg);
        //    BattleSystem.Instance.Reset();
        //}
    }

    //public void ShowScoreNum(Team resultTeam, LevelConfig cfg)
    //{
    //    int starScore = starNum * 10000;
    //    int hitShipScore = resultTeam.hitships * 1 > 5000 ? 5000 : resultTeam.hitships * 1;
    //    int produceScore = resultTeam.produces * 1 > 5000 ? 5000 : resultTeam.produces * 1;
    //    float timeScore = 3600 - BattleSystem.Instance.sceneManager.GetBattleTime() < 0 ? 0 : 3600 - BattleSystem.Instance.sceneManager.GetBattleTime();
    //    int totalScore = (int)((cfg.scoreper) * (starScore + hitShipScore + produceScore + timeScore));
    //    LevelDataHandler.Instance.saveVictionaryData(cfg, totalScore, starNum);
    //    scoreNum.GetComponent<UILabel>().text = totalScore.ToString();
    //}

    //public void ShowStarBehavior(Team resultTeam, LevelConfig cfg)
    //{
    //    if(cfg.star4Dead == -1)
    //    {
    //        starNum = 3;
    //        threeStars.SetActive(true);
    //        fourStars.SetActive(false);
    //        oneStarInFour.SetActive(false);
    //        oneStarInThree.SetActive(false);
    //        twoStars.SetActive(false);
    //        twoStarInFour.SetActive(false);
    //        threeStarInFour.SetActive(false);
    //        twoStarInThree.SetActive(false);
    //        return;
    //    }
    //     if(resultTeam.destory < cfg.star4Dead)
    //    {
    //        starNum = 4;
    //        threeStars.SetActive(false);
    //        fourStars.SetActive(true);
    //        oneStarInFour.SetActive(false);
    //        oneStarInThree.SetActive(false);
    //        twoStars.SetActive(false);
    //        twoStarInFour.SetActive(false);
    //        threeStarInFour.SetActive(false);
    //        twoStarInThree.SetActive(false);
    //    }
    //    else if(resultTeam.destory >= cfg.star4Dead && resultTeam.destory < cfg.star3Dead)
    //    {
    //        starNum = 3;
            
    //        if(cfg.maxStar == 3)
    //        {
    //            threeStars.SetActive(true);
    //            twoStarInFour.SetActive(false);
    //            threeStarInFour.SetActive(false);
    //            twoStarInThree.SetActive(false);
    //        }
    //        else
    //        {
    //            threeStarInFour.SetActive(true);
    //            twoStarInFour.SetActive(false);
    //            twoStarInThree.SetActive(false);
    //        }
    //        fourStars.SetActive(false);
    //        oneStarInFour.SetActive(false);
    //        oneStarInThree.SetActive(false);
    //        twoStars.SetActive(false);
    //    }
    //    else if(resultTeam.destory >= cfg.star3Dead && resultTeam.destory < cfg.star2Dead)
    //    {
    //        starNum = 2;
    //        if(cfg.maxStar == 3)
    //        {
    //            twoStarInThree.SetActive(true);
    //            twoStarInFour.SetActive(false);
    //            threeStarInFour.SetActive(false);
    //        }
    //        else
    //        {
    //            twoStarInFour.SetActive(true);
    //            threeStarInFour.SetActive(false);
    //            twoStarInThree.SetActive(false);
    //        }
    //        threeStars.SetActive(false);
    //        fourStars.SetActive(false);
    //        oneStarInFour.SetActive(false);
    //        oneStarInThree.SetActive(false);
    //        twoStars.SetActive(false);
    //    }
    //    else if(resultTeam.destory > cfg.star2Dead)
    //    {
    //        starNum = 1;
    //        threeStars.SetActive(false);
    //        fourStars.SetActive(false);
    //        oneStarInFour.SetActive(cfg.maxStar != 3);
    //        oneStarInThree.SetActive(cfg.maxStar == 3);
    //        twoStars.SetActive(false);
    //        twoStarInFour.SetActive(false);
    //        threeStarInFour.SetActive(false);
    //        twoStarInThree.SetActive(false);
    //    }
    //}

    private void SetPlayerBaseInfo()
    {
        if (LocalPlayer.Get().playerData != null)
        {
            moneyGo.GetComponent<UILabel>().text = LocalPlayer.Get().playerData.money.ToString();
            powerGo.GetComponent<UILabel>().text = FormatPower();
        }
    }
     
    private string FormatPower()
    {
        int nPower = LocalPlayer.Get().playerData.power;
        string fmt = string.Empty;
        fmt = string.Format("{0} / 1000", nPower);
        return fmt;
    }

}

