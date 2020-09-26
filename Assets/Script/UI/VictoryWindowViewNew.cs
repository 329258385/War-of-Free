using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Solarmax;






public class VictoryWindowViewNew : BaseWindow
{

    /// <summary>
    /// 动画播放器
    /// </summary>
    public UIPlayAnimation aniPlayer;	// 动画


    /// <summary>
    /// 界面结果信息
    /// </summary>
    public UILabel          awardPower;
    public UILabel          awardMoney;
    public UILabel          scoreNum;

    /// <summary>
    /// 星星 
    /// </summary>
    public GameObject[]     starList;
    

    private int             starNum  = 1;
    private int             starGold  = 0;
    private int             starPower = 0;
    private string          selectedCfgId;


    public override bool Init ()
	{
        base.Init();
        RegisterEvent(EventId.VictioryWindowViewShow);
        return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
    }

	public override void OnHide ()
	{
       
    }


    public override void OnUIEventHandler(EventId eventId, params object[] args)
    {
        if (eventId == EventId.VictioryWindowViewShow) {
            string selectLevelId = args[0] as string;
            selectedCfgId = selectLevelId;
            //LevelConfig cfg = LevelConfigConfigProvider.Instance.GetData(selectLevelId);
            //awardPower.text      = "x " + cfg.awardPower;
            //awardMoney.text      = "x " + cfg.awardGold;

            int destorys = (int)args[1];
            int hitships = (int)args[2];
            int produces = (int)args[3];
            //LocalPlayer.Get().ChangeMoney(cfg.awardGold);
            //LocalPlayer.Get().ChangePower(cfg.awardPower);
            //ShowStarBehavior(destorys, cfg);
            //CalcStarGoldAndPower(cfg);
            awardMoney.text = "x " + starGold.ToString();
            awardPower.text = "x " + starPower.ToString();
            LocalPlayer.Get().ChangeMoney(starGold);
            //int score = ShowScoreNum(hitships, produces, cfg);
            aniPlayer.onFinished.Add(new EventDelegate(AnimatorPlayEnd));
            PlayAnimation("RewardsWindowView_in");
            PlayStarSound();
            LocalStorageSystem.Instance.SaveLocalChapters();
            LocalStorageSystem.Instance.SaveLocalAccount(true);
            //UserSyncSysteam.Get().SaveAccout2Cloud();
            EventSystem.Instance.FireEvent(EventId.OnUnLockNextLevel);
            //float totalTime = BattleSystem.Instance.sceneManager.GetBattleTime();
            //Flurry.Instance.FlurryBattleEndEvent(selectLevelId, "0", score.ToString(), starNum.ToString(), hitships.ToString(), destorys.ToString(), totalTime.ToString());
        }
    }

    public void PlayStarSound()
    {
        float startTime = 0.8f;
        float stepTime = 0.2f;
        if(starNum == 1)
        {
            Invoke("PlaySingleStarSound", startTime);
        }
        else if(starNum ==2)
        {
            Invoke("PlaySingleStarSound", startTime);
            Invoke("PlaySingleStarSound", startTime + 1 * stepTime);
        }
        else if(starNum==3)
        {
            Invoke("PlaySingleStarSound", startTime);
            Invoke("PlaySingleStarSound", startTime + 1 * stepTime);
            Invoke("PlaySingleStarSound", startTime + 2 * stepTime);
        }
        else
        {
            Invoke("PlaySingleStarSound", startTime);
            Invoke("PlaySingleStarSound", startTime + 1 * stepTime);
            Invoke("PlaySingleStarSound", startTime + 2 * stepTime);
            Invoke("PlaySingleStarSound", startTime + 3 * stepTime);
        }
    }

    public void PlaySingleStarSound()
    {
        AudioManger.Get().PlayEffect("starSound");
    }

    //public void CalcStarGoldAndPower(LevelConfig cfg)
    //{
    //    starGold = 0;
    //    if (LevelDataHandler.Instance.currentLevel == null)
    //    {
    //        return;
    //    }
    //    if (LevelDataHandler.Instance.currentLevel.star < starNum)
    //    {
    //        int star = LevelDataHandler.Instance.currentLevel.star;
    //        if (star == 0 && star < starNum)
    //        {
    //            starGold += cfg.star1Gold;
    //            starPower += 0;
    //            star++;
    //        }
    //        if (star == 1 && star < starNum)
    //        {
    //            starGold += cfg.star2Gold;
    //            starPower += 2;
    //            star++;
    //        }
    //        if (star == 2 && star < starNum)
    //        {
    //            starGold += cfg.star3Gold;
    //            starPower += 3;
    //            star++;
    //        }
    //        if (star == 3 && star < starNum)
    //        {
    //            starGold += cfg.star4Gold;
    //            starPower += 5;
    //            star++;
    //        }
    //    }
    //}

    //public int ShowScoreNum(int hitships, int produces, LevelConfig cfg)
    //{
    //    int starScore       = starNum * 10000;
    //    int hitShipScore    = hitships * 1 > 5000 ? 5000 : hitships * 1;
    //    int produceScore    = produces * 1 > 5000 ? 5000 : produces * 1;
    //    float timeScore     = 3600 - BattleSystem.Instance.sceneManager.GetBattleTime() < 0 ? 0 : 3600 - BattleSystem.Instance.sceneManager.GetBattleTime();
    //    int totalScore      = (int)((cfg.scoreper) * (starScore + hitShipScore + produceScore + timeScore));

    //    if (LevelDataHandler.Instance.IsNeedSend(starNum, totalScore))
    //    {
    //        NetSystem.Instance.helper.RequestSetLevelSorce(cfg.id, LocalAccountStorage.Get().account, totalScore);
    //    }
    //    LevelDataHandler.Instance.SetLevelStarToLocalStorage(starNum, totalScore );
    //    scoreNum.text       = totalScore.ToString();
    //    return totalScore;
    //}


    /// <summary>
    /// 计算得到的星数量
    /// </summary>
    //public void ShowStarBehavior(int destory, LevelConfig cfg)
    //{
    //    if (cfg.maxStar == 3) {
    //        if (destory >= cfg.star2Dead) {
    //            starNum = 1;
    //        } else if (destory >= cfg.star3Dead) {
    //            starNum = 2;
    //        } else {
    //            starNum = 3;
    //        }
    //    } else if (cfg.maxStar == 4) {
    //        if (cfg.star4Dead == -1) {
    //            starNum = 3;
    //        } else if (destory < cfg.star4Dead) {
    //            starNum = 4;
    //        } else if (destory >= cfg.star4Dead && destory < cfg.star3Dead) {
    //            starNum = 3;
    //        } else if (destory >= cfg.star3Dead && destory < cfg.star2Dead) {
    //            starNum = 2;
    //        } else if (destory > cfg.star2Dead) {
    //            starNum = 1;
    //        }
    //    } else {
    //        LoggerSystem.Instance.Warn("关卡配置的最大星星数量[{0}]不正确！！！", cfg.maxStar);
    //        return;
    //    }

    //    int objects = starList.Length;
    //    if (objects < starNum)
    //        return;

    //    for (int i = 0; i < objects; i++)
    //    {
    //        if( i < starNum )
    //            starList[i].SetActive(true);
    //        else
    //            starList[i].SetActive(false);
    //    }
    //}


    /// <summary>
    /// 星座飞入动画播放完回调
    /// </summary>
    private void AnimatorPlayEnd()
    {
        aniPlayer.onFinished.Clear();
        UISystem.Get().HideWindow("VictoryWindowView");
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    private void PlayAnimation(string strAni, float speed = 1.0f)
    {
        aniPlayer.clipName = strAni;
        aniPlayer.resetOnPlay = true;
        aniPlayer.Play(true, false);
    }
}

