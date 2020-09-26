using UnityEngine;
using System.Collections.Generic;
using Solarmax;

public class ResultWindow : BaseWindow
{
	public UILabel result;
	public GameObject infoTemplate;
	public GameObject[] posRoots; //1v1, 1v1v1, 1v1v1v1, 2v2
    public GameObject[] btnAttention;

    public UIPlayAnimation aniPlayer;	// 动画
	public GameObject mvpSprite;

	public GameObject downGo;
	public GameObject confirmBtn;
	public GameObject chestObj;
	public UISprite chestIcon;
	public UILabel chestLabel;
	public UILabel chestNumLabel;
	public GameObject lineObj;
	public GameObject moneyObj;
	public UILabel moneyLabel;

	private string mapId;


    private bool IsNeedPlayMvpEffect = false;
	public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.OnFinished);
        RegisterEvent(EventId.OnFriendFollowResult);
        for (int i = 0; i < 4; i++ )
            btnAttention[i].GetComponent<UIEventListener>().onClick += OnAttentionClick;

            return true;
	}

	public override void OnShow()
	{
        base.OnShow();
        //EffectManager.Instance.Destroy(); //清空所有的特效

        chestObj.SetActive (false);
		lineObj.SetActive (false);
		moneyObj.SetActive (false);
		// 地图名
		mapId = LocalPlayer.Get().battleMap;

		// 隐藏所有位置节点
		for (int i = 0; i < posRoots.Length; ++i)
		{
			posRoots [i].SetActive (false);
		}

		// 根据类型和人数判断选用哪种模式
		SetPage();

		// 进入动画
        PlayAnimation("ResultWindow_h2");

		// 隐藏战斗的节点
		BattleSystem.Instance.battleData.root.SetActive(false);
	}

	public override void OnHide()
	{
		// 隐藏战斗的节点
		BattleSystem.Instance.battleData.root.SetActive(true);
	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnFinished) {
			NetMessage.SCFinishBattle proto = args[0] as NetMessage.SCFinishBattle;
			if (proto == null)
				return;

            int rewardCount = 0;
            if (proto.rewards.Count > 0)
            {
                moneyObj.SetActive(true);
                moneyLabel.text = string.Format("X{0}", proto.rewards[0].num);
                moneyObj.SetActive(false);
                ++rewardCount;
            }
            if (rewardCount == 2)
            {
                lineObj.SetActive(true);
            }
            else if (rewardCount == 1)
            {
                chestObj.transform.localPosition = moneyObj.transform.localPosition = lineObj.transform.localPosition;
            }
		}

        if (eventId == EventId.OnFriendFollowResult)
        {
            // 关注好友结果
            int userId = (int)args[0];
            bool follow = (bool)args[1]; // 关注或者取消关注行动
            NetMessage.ErrCode errCode = (NetMessage.ErrCode)args[2];
            if (errCode == NetMessage.ErrCode.EC_Ok)
            {
                Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(815), 1.0f);
                for (int i = 0; i < 4; i++)
                {
                    UILabel ID = btnAttention[i].transform.Find("useid").GetComponent<UILabel>();
                    if (ID == null || string.IsNullOrEmpty(ID.text))
                        continue;

                    int useID = int.Parse(ID.text);
                    if (userId == useID)
                        btnAttention[i].SetActive(false);
                }
            }
        }
	}


    public void OnPlayerFistAniamionEnd()
    {
        Invoke("PlayerFistAniamion", 0.5f);
    }


    private void PlayerFistAniamion()
    {
        PlayMvpEffect();
    }

	/// <summary>
	/// 设置页面信息
	/// </summary>
	private void SetPage()
	{
		MapConfig map           = MapConfigProvider.Instance.GetData (mapId);
		List<Team> groupTeams   = new List<Team> ();
		// 胜利方
		Team selfTeam =  BattleSystem.Instance.sceneManager.teamManager.GetTeam (BattleSystem.Instance.battleData.currentTeam);
		Team winTeam = null;
		bool timeout = false;
		CalculateWinTeam (map, out winTeam, out timeout);

        // 胜利方的友方
        bool showwin = false;
		for (int i = 0; i < map.player_count; ++i) {
			TEAM T = (TEAM)(i + 1);
			Team t =  BattleSystem.Instance.sceneManager.teamManager.GetTeam (T);
			if (t != winTeam && winTeam.IsFriend (t.groupID)) 
			{
				groupTeams.Add (t);
			} 
            else if (t == winTeam)
            {
                groupTeams.Add (t);
            }
		}
		if (groupTeams.Count > 0) {
			showwin = true;
			groupTeams.Sort ((arg0, arg1) => {
				int ret = arg0.resultOrder.CompareTo (arg1.resultOrder);
				if (ret == 0)
					ret = arg0.scoreMod.CompareTo (arg1.scoreMod);
				if (ret == 0)
					ret = arg0.destory.CompareTo (arg1.destory);
				return -ret;
			});
		}

		// 失败方
        List<Team> failTeams = new List<Team>();
        for (int i = 0; i < map.player_count; ++i)
        {
            TEAM T = (TEAM)(i + 1);
            Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam(T);
            if (t != winTeam && !winTeam.IsFriend(t.groupID) && !groupTeams.Contains(t))
            {
                failTeams.Add(t);
            }
        }
		failTeams.Sort((arg0, arg1) => {
			int ret = arg0.resultOrder.CompareTo (arg1.resultOrder);
			if (ret == 0)
				ret = arg0.scoreMod.CompareTo (arg1.scoreMod);
			if (ret == 0)
				ret = arg0.hitships.CompareTo (arg1.hitships);
			return -ret;
		});

		groupTeams.AddRange (failTeams);
		if (groupTeams.Contains (selfTeam))
		{
			// 此场战斗中包含自己
			// 判断mvp还是胜利
			if (timeout)
            {
				// 平局
				result.text = LanguageDataProvider.GetValue(110);
			}
            else
            {
				if (selfTeam == winTeam)
                {
					// 注意此种逻辑只能在胜方包含己方时，因为重播如果有可能win和self都是neutral
					// mvp
					result.text = LanguageDataProvider.GetValue (105);
				}
                else if (selfTeam.scoreMod > 0)
                {
					// 胜利
					result.text = LanguageDataProvider.GetValue (100);
				} else {
					// 失败
					result.text = LanguageDataProvider.GetValue (101);
				}
			}
			// 音效
			if (selfTeam.scoreMod > 0) {
				AudioManger.Get ().PlayEffect ("onPVPvictory");
			} else {
				AudioManger.Get ().PlayEffect ("onPVPdefeated");
			}
		}
		else
		{
			// 此场战斗不包含自己
			result.text = LanguageDataProvider.GetValue(106);
		}
        
		if (BattleSystem.Instance.battleData.gameType == GameType.PVP) {
			downGo.SetActive (true);
			confirmBtn.gameObject.SetActive (false);

            if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_2V2)
            {
                if (selfTeam.scoreMod > 0)
                {
                    // 胜利
                    result.text = LanguageDataProvider.GetValue(100);
                }
                else
                {
                    // 失败
                    result.text = LanguageDataProvider.GetValue(101);
                }
            }
            else
            {
                // 设置名次
                if (selfTeam.resultRank >= 0)
                {
                    string[] ranks = LanguageDataProvider.GetValue(107).Split(',');
                    int rank = map.player_count - selfTeam.resultRank - 1;/// note: resultRank最后一名是0，第一名是3，和一二三四有个倒序关系
                    result.text = LanguageDataProvider.Format(108, ranks[rank]);
                }
                else
                {
                    result.text = LanguageDataProvider.Format(106);
                }
            }
        }
        else {
			downGo.SetActive (false);
			confirmBtn.gameObject.SetActive (true);

			// 设置名次
			if (selfTeam.resultEndtype == NetMessage.EndType.ET_Win)
            {
				result.text = LanguageDataProvider.Format (109, selfTeam.leagueMvp);
			}
            else
            {
                if (selfTeam.resultRank >= 0)
                { 
                    string[] ranks  = LanguageDataProvider.GetValue (107).Split (',');
    				int rank        = map.player_count - selfTeam.resultRank - 1;/// note: resultRank最后一名是0，第一名是3，和一二三四有个倒序关系
	    			result.text     = LanguageDataProvider.Format (108, ranks [rank]);
                }
                else
                {
                    result.text     = LanguageDataProvider.Format(106);
                }
            }
        }

		// 根据类型和人数判断选用哪种模式
		for (int i = 0; i < groupTeams.Count; ++i)
        {
			Team t = groupTeams [i];
			SetPosInfo (i, t, t.scoreMod, t.hitships, winTeam);
		}

        if (winTeam.team != TEAM.Neutral)
        {
            IsNeedPlayMvpEffect = true;
        }

        float totalTime = BattleSystem.Instance.sceneManager.GetBattleTime();
        string matchType = "1";
        if( BattleSystem.Instance.battleData.gameType != GameType.PVP || BattleSystem.Instance.battleData.gameType != GameType.League )
        {
            matchType = "0";
        }
        Flurry.Instance.FlurryPVPBattleEndEvent(matchType, mapId, selfTeam.scoreMod.ToString(), selfTeam.hitships.ToString(), selfTeam.destory.ToString(), totalTime.ToString());
	}

	/// <summary>
	/// 计算赢的队伍
	/// </summary>
	/// <returns>The window team.</returns>
	private void CalculateWinTeam (MapConfig map, out Team win, out bool timeout)
	{
		win = BattleSystem.Instance.sceneManager.teamManager.GetTeam (TEAM.Neutral);
		timeout = false;
		for (int i = 0; i < map.player_count; ++i) {
			TEAM T = (TEAM)(i + 1);
			Team t =  BattleSystem.Instance.sceneManager.teamManager.GetTeam (T);

			if (t.resultEndtype == NetMessage.EndType.ET_Win) {
				win = t;
			} else if (t.resultEndtype == NetMessage.EndType.ET_Timeout) {
				timeout = true;
			}
		}
	}

	private GameObject GetPosRoot()
	{
		MapConfig map = MapConfigProvider.Instance.GetData (mapId);

		GameObject root = null;
		if (BattleSystem.Instance.battleData.teamFight) {
			root = posRoots [3];
		} else {
			root = posRoots [map.player_count - 2];
		}

		return root;
	}

	private void SetPosInfo(int pos, Team team, int score, int destroy, Team winTeam )
	{
		if (pos >= posRoots.Length) 
		{
			return;
		}

		GameObject info = posRoots[pos];
		info.SetActive (true);
		Color color     = team.color;
		color.a         = 1.0f;

		UISprite icon   = info.transform.Find ("icon").GetComponent<UISprite> ();
        UISprite iconbg = info.transform.Find ("icon_g").GetComponent<UISprite>();
		UISprite kuang  = info.transform.Find ("kuang").GetComponent<UISprite> ();
		UILabel name    = info.transform.Find ("name").GetComponent<UILabel> ();
		UILabel sco     = info.transform.Find ("score").GetComponent<UILabel> ();
		UILabel des     = info.transform.Find ("num").GetComponent<UILabel> ();
		UISprite win    = info.transform.Find ("winbg").GetComponent<UISprite> ();
        UISprite mvp    = info.transform.Find ("mvp").GetComponent<UISprite>();

        if (winTeam.IsFriend(team.groupID))
        {
            win.spriteName = "result_victory_bg";
            mvp.gameObject.SetActive(true);
        }
        else
        {
            win.spriteName = "result_failure_bg";
            mvp.gameObject.SetActive(false);
        }

        if (team.playerData.raceId <= 0)
        {
            iconbg.spriteName   = "";
            icon.spriteName     = team.playerData.icon;
        }
        

		kuang.spriteName = team.iconname;
		name.text = team.playerData.name;
		name.color = color;

        GameObject attention = info.transform.Find("attention").gameObject;
        Team selfTeam = BattleSystem.Instance.sceneManager.teamManager.GetTeam(BattleSystem.Instance.battleData.currentTeam);
        if (selfTeam.team != team.team && team.playerData.userId > 0 && !FriendDataHandler.Get().IsMyFollowEX(team.playerData.userId))
        {
            attention.SetActive(true);
            UILabel UseID = attention.transform.Find("useid").GetComponent<UILabel>();
            if ( UseID != null )
            {
                UseID.text = team.playerData.userId.ToString();
            }
        }
        else
        {
            attention.SetActive(false);
            UILabel UseID = attention.transform.Find("useid").GetComponent<UILabel>();
            if (UseID != null)
            {
                UseID.text = "";
            }
        }

		string str;
		if (score > 0)
			str = string.Format ("+{0}", score);
		else
			str = score.ToString ();
		sco.text = str;
		des.text = destroy.ToString();
	}

	private void PlayAnimation( string strAni )
	{
		// 播放进入动画
        aniPlayer.clipName = strAni;
        aniPlayer.resetOnPlay = true;
        aniPlayer.Play(true, false);
	}


    private void PlayMvpEffect()
    {
        if (!IsNeedPlayMvpEffect)
            return;

         PlayAnimation("ResultWindow_h2MVP");
         IsNeedPlayMvpEffect = false;
    }


	public void OnGoHomeClick()
	{
		BattleSystem.Instance.Reset ();
		BattleSystem.Instance.battleData.resumingFrame = -1;
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("StartWindow");
	}

	public void OnShareClick()
	{
		Debug.LogWarning ("You have click the share button!");
	}

	public void OnLeagueConfirmClick ()
	{
		// 回到联赛主页面
		BattleSystem.Instance.Reset ();
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("CustomSelectWindowNew");
		EventSystem.Instance.FireEvent (EventId.OnManualSelectLeaguePage);
	}

    public void OnAttentionClick( GameObject go )
    {
        if (go == null)
            return;

        UILabel name = go.transform.Find("useid").GetComponent<UILabel>();
        if (name == null || string.IsNullOrEmpty(name.text))
            return;

        int useID = int.Parse(name.text);
        NetSystem.Instance.helper.FriendFollow(useID, true);
    }
}

