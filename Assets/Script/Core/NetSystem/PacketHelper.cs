using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NetMessage;
using Plugin;
using Solarmax;
using System.IO;

/// <summary>
/// 网络相关
/// </summary>
public class PacketHelper
{
	void Log (string fmt, params object[] args)
	{
		string str = string.Format (fmt, args);
		LoggerSystem.Instance.Debug (str);
	}

	/// <summary>
	/// 注册网络消息
	/// </summary>
	public void RegisterAllPacketHandler ()
	{
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCMatch, OnMatch);
		// NetSystem.Instance.GetConnector ().RegisterHandler<SCGetUserData,SCGetUserData.Builder> ((int)NetMessage.MsgId.ID_SCMatchingInfo, this.OnMatchInfoUpdate);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCMatchFail, this.OnMatchFailed);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCStartBattle, this.OnNetStartBattle);	//
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCFrame, this.OnNetFrame);		//
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCFinishBattle, this.OnFinishBattle);	//
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCGetUserData, this.OnRequestUser);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCCreateUserData, this.OnCreateUser);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCSyncUserData, this.OnSyncUserData);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCErrCode, this.OnErrCode);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCPong, this.OnPong);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCLoadRank, this.OnLoadRank);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCReady, this.OnReady);					//

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCFriendFollow, this.OnFriendFollow);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCFriendLoad, this.OnFriendLoad);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCFriendNotify, this.OnFriendNotify);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCFriendRecommend, this.OnFriendRecommend);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCFriendSearch, this.OnFriendSearch);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCTeamCreate, this.OnTeamCreate);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCTeamInvite, this.OnTeamInvite);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCTeamUpdate, this.OnTeamUpdate);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCTeamDel, this.OnTeamDelete);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCTeamStart, this.OnTeamStart);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCTeamInviteReq, this.OnTeamInviteReq);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCTeamInviteResp, this.OnTeamInviteResponse);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCBattleReportLoad, this.OnBattleReportLoad);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCBattleReportPlay, this.onBattleReportPlay);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCBattleResume, this.OnBattleResume);

		//LeagueRank
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCGetLeagueInfo, this.OnGetLeagueInfo);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCLeagueList, this.OnGetLeagueList);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCLeagueAdd, this.OnLeagueSignUp);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCLeagueRank, this.OnLeagueRank);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCLeagueMatch, this.OnLeagueMatch);

		//
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCWatchRooms, this.OnRequestRoomList);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCJoinRoom, this.OnRequestJoinRoom); 
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCCreateRoom, this.OnRequestCreateRoom);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCQuitRoom, this.OnRequestQuitRoom); 
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCRoomRefresh, this.RoomRefresh);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCRoomListRefresh, this.RoomListRefresh);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCMatch2CurNum, this.OnMatch2CurNum);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCStartMatch2, this.ONMatchGameBack);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCStartMatch3, this.OnStarMatch3);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCMatch3Notify, this.OnMatch3Notify);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCChangeRace, this.OnChangeRace);		//
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCStartSelectRace, this.OnStartSelectRace);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCSelectRaceNotify, this.OnSelectRaceNotify);
		#if SERVER
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCRefereeReq, this.OnRefereeReq);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCRefereeStop, this.OnRefereeStop);
		#endif

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCGetRaceData, this.OnGetRaceData);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCRaceSkillLevelUp, this.OnRaceSkillLevelUp);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCPackUpdate, this.OnPackUpdate);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCGainTimerChest, this.OnOpenTimerChest);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCGainBattleChest, this.OnOpenBattleChest);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCUpdateTimerChest, this.OnUpdateTimeChest);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCUpdateBattleChest, this.OnUpdateBattleChest);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCUnlockChest, this.OnStartBox);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCGainChest, this.OnOpenBox);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCAddChest, this.OnNotifyBox);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCSingleMatch, this.OnSingleMatch);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCSetCurLevel, this.OnSetCurrentLevelResult);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCRename, this.OnChangeName);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCRaceNotify, this.OnRaceNotify);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCClientStorageLoad, this.OnClientStorageLoad);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCClientStorageSet, this.OnClientStorageSet);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCLeagueQuit, this.OnLeagueQuitResult);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCResume, this.OnReconnectResumeResult);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCKickUserNtf, this.OnKickUserNtf);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCNotify, this.OnServerNotify);

		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCStartMatchReq, this.OnStartMatchRequest);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCMatchInit, this.OnMatchInit);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCMatchUpdate, this.OnMatchUpdate);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCQuitMatch, this.OnQuitMatch);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCMatchComplete, this.OnMatchComplete);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCMatchPos, this.OnMatchPos);


		//关卡有关的
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCLoadChapters, this.OnLoadChapters);
		//NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCLoadChapter, this.OnLoadOneChapter);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCStartLevel, this.OnStartLevel);
		NetSystem.Instance.GetConnector ().RegisterHandler((int)NetMessage.MsgId.ID_SCIntAttr, this.OnIntAttr);
        NetSystem.Instance.GetConnector().RegisterHandler((int)NetMessage.MsgId.ID_SCPveRankReportLoad, this.OnRequestPveRankReport);
        NetSystem.Instance.GetConnector().RegisterHandler((int)NetMessage.MsgId.ID_SCBuyChapter, this.OnRequestBuyChapter);
        NetSystem.Instance.GetConnector().RegisterHandler((int)NetMessage.MsgId.ID_SCSetLevelScore, this.OnRequestSetLevelSorce);

        //金钱
        NetSystem.Instance.GetConnector().RegisterHandler((int)NetMessage.MsgId.ID_SCChangeMoney, this.OnMoneyChange);
        NetSystem.Instance.GetConnector().RegisterHandler((int)NetMessage.MsgId.ID_SCGenerateUrl, this.OnGenPresignedUrl);
    }

#if SERVER
	void OnRefereeStop(int msgID, PacketEvent msgBody)
	{
		Log("OnRefereeStop");
		BattleSystem.Instance.battleData.RefereeFinish = true;
	}
	void OnRefereeReq(int msgID, PacketEvent msgBody)
	{
		Log("OnRefereeOkReq {0}", BattleSystem.Instance.battleData.RefereeBusy);
		var bd = new NetMessage.CSRefereeRep.Builder();

		if (BattleSystem.Instance.battleData.RefereeBusy)
		{
			bd.SetCode(ErrCode.EC_RefereeBusy);
		}
		else 
		{
			BattleSystem.Instance.battleData.RefereeBusy = true;
			bd.SetCode(ErrCode.EC_Ok);
		}

		NetSystem.Instance.Send((int)NetMessage.MsgId.ID_CSRefereeRep, bd);
	}
#endif


    public IEnumerator ConnectServer ( bool bTips = true )
	{
		string addr = string.Empty;
		ConfigSystem.Instance.TryGetConfig ("R_SERVER", out addr);

		#if SERVER
		{
			ConfigSystem.Instance.TryGetConfig ("Referee_SERVER", out addr);
		}
		#else

		//新服务器
		#if R_SERVER
		ConfigSystem.Instance.TryGetConfig ("R_SERVER", out addr);
		#elif L_SERVER
		//老服务器
		ConfigSystem.Instance.TryGetConfig ("L_SERVER", out addr);
		#elif DEV_SERVER
		//开发服务器
		ConfigSystem.Instance.TryGetConfig ("DEV_SERVER", out addr);
		#endif

		#endif

		string[] addrs = addr.Split (':');
		string ip = addrs [0];
		int port = int.Parse (addrs [1]);

#if UNITY_EDITOR

		//if (string.IsNullOrEmpty(LocalSettingStorage.Get ().ip)) {
		LocalSettingStorage.Get ().ip = ip;
		//}
#else
		LocalSettingStorage.Get ().ip = ip;
#endif

		NetSystem.Instance.Connect (LocalSettingStorage.Get ().ip, port);

		while (NetSystem.Instance.GetConnector ().GetConnectStatus () == ConnectionStatus.CONNECTING)
			yield return 1;
		if (NetSystem.Instance.GetConnector ().GetConnectStatus () == ConnectionStatus.CONNECTED) {
			NetSystem.Instance.ping.Pong (100);
			EventSystem.Instance.FireEvent (EventId.NetworkStatus, true);
		} else {
			#if !SERVER
            if(bTips)
			    Tips.Make (Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(207), 1.0f);
			#endif

			NetSystem.Instance.Close ();
			NetSystem.Instance.ping.Pong (-1);
			EventSystem.Instance.FireEvent (EventId.NetworkStatus, false);
		}
	}

	/// <summary>
	/// 开始匹配
	/// </summary>
	public void Match (bool single, bool team, string matchId)
	{
		NetMessage.CSMatch bd = new NetMessage.CSMatch();
		if (single) {
			bd.type = NetMessage.BattleType.Melee;
		} else if (team) {
			bd.type  = NetMessage.BattleType.Group2v2;
		}
        else {
			BattleSystem.Instance.battleData.matchId = matchId;
			bd.match_id = matchId;
		}
		NetSystem.Instance.Send<NetMessage.CSMatch> ((int)NetMessage.MsgId.ID_CSMatch, bd);
	}

	/// <summary>
	/// 匹配响应
	/// </summary>
	/// <param name="msgID">Message I.</param>
	/// <param name="msgBody">Message body.</param>
	void OnMatch (int msgID, PacketEvent msgBody)
	{
        MemoryStream ms     = msgBody.Data as MemoryStream;
        SCMatch response    = ProtoBuf.Serializer.Deserialize<NetMessage.SCMatch>(ms);

		#if !SERVER
		//		UISystem.Get ().HideAllWindow();
		//		UISystem.Get ().ShowWindow("MatchWindow");

		EventSystem.Instance.FireEvent (EventId.OnMatch, response.count_down );
		#endif

	}

	/// <summary>
	/// 房间信息更新
	/// </summary>
	/// <param name="msgID">Message I.</param>
	/// <param name="msgBody">Message body.</param>
	//	void OnMatchInfoUpdate(int msgID, IMessage msgBody)
	//	{
	//		NetMessage.SCMatchingInfo matchingInfo = msgBody as NetMessage.SCMatchingInfo;
	//
	//		#if !SERVER
	//		UISystem.Get ().OnEventHandler (EventId.UpdateRoomInfo, "MatchWindow", matchingInfo.PlayerNum);
	//		#endif
	//	}

	/// <summary>
	/// 匹配失败
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	private void OnMatchFailed (int msgId, PacketEvent msg)
	{
        #if !SERVER
        // 收到此包意味着服务器已经被从匹配队列删除，所以关闭页面
        UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("StartWindow");
		#endif
	}

	/// <summary>
	/// 收到ready之后展示玩家信息，之后再收到startbattle
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	public void OnReady (int msgId, PacketEvent msg)
	{
        MemoryStream ms     = msg.Data as MemoryStream;
        NetMessage.SCReady proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCReady>(ms);
        OnReady(proto);
    }

    /// <summary>
    /// 随即队伍的队友
    /// </summary>
    private string RandomTeamMember( )
    {
        string strGroup = "0,3|1,2";
        int userInde = BattleSystem.Instance.battleData.rand.Range(0, 2) + 1;
        if (userInde == 1 )
        {
            strGroup = "0,1|2,3";
        }
        else if (userInde == 2)
        {
            strGroup = "0,2|1,3";
        }
        else if (userInde == 3)
        {
            strGroup = "0,3|1,2";
        }
        return strGroup;
    }

    public void OnReady (NetMessage.SCReady proto )
	{
        BattleSystem.Instance.SetPlayMode(true, false);
        BattleSystem.Instance.Reset ();
        BattleSystem.Instance.lockStep.replay = true;
        BattleSystem.Instance.battleData.battleSubType = proto.sub_type;

        if (proto.match_type == MatchType.MT_League)
        {
			BattleSystem.Instance.battleData.gameType   = GameType.League;
            BattleSystem.Instance.battleData.battleType = BattlePlayType.Normalize;
        }
        else if (proto.match_type == MatchType.MT_Ladder)
        {
			BattleSystem.Instance.battleData.gameType   = GameType.PVP;
            BattleSystem.Instance.battleData.battleType = BattlePlayType.Normalize;
        }
        else if (proto.match_type == MatchType.MT_Room)
        {
			BattleSystem.Instance.battleData.gameType   = GameType.PVP;
            BattleSystem.Instance.battleData.battleType = BattlePlayType.Normalize;
        }
        else if( proto.match_type == MatchType.MT_Sing )
        {
			BattleSystem.Instance.battleData.gameType   = GameType.SingleLevel;
            BattleSystem.Instance.battleData.battleType = BattlePlayType.Replay;
            BattleSystem.Instance.lockStep.replay       = false;
            BattleSystem.Instance.lockStep.replaySingle = true;
        }
        else
        {
            BattleSystem.Instance.battleData.gameType   = GameType.PVP;
            BattleSystem.Instance.battleData.battleType = BattlePlayType.Normalize;
        }

        // 设置随机种子 地图
        BattleSystem.Instance.battleData.matchId    = proto.match_id;
		LocalPlayer.Get ().battleMap                = BattleSystem.Instance.battleData.matchId;
		BattleSystem.Instance.battleData.rand.seed  = proto.random_seed;

  //      int aiStrategy = 0;
  //      LevelConfig levelCfg = LevelConfigConfigProvider.Get().GetData(proto.match_id);
  //      MapConfig mapConfig     = MapConfigProvider.Instance.GetData(proto.match_id);
  //      if (string.IsNullOrEmpty(mapConfig.defaultai))
  //          BattleSystem.Instance.battleData.aiStrategy = -1;
  //      else
  //          BattleSystem.Instance.battleData.aiStrategy = int.Parse(mapConfig.defaultai);
  //      if (BattleSystem.Instance.battleData.aiStrategy < 0)
  //      {
  //          if (levelCfg == null)
  //              BattleSystem.Instance.battleData.aiStrategy = 3;
  //          else
  //              BattleSystem.Instance.battleData.aiStrategy = AIStrategyConfigProvider.Instance.GetAIStrategyByName(levelCfg.aiType);
  //      }
  //      if (BattleSystem.Instance.battleData.aiStrategy < 0)
  //          BattleSystem.Instance.battleData.aiStrategy = 1;
  //      if( levelCfg != null )
  //      {
  //          BattleSystem.Instance.battleData.aiParam = levelCfg.aiParam;
  //          BattleSystem.Instance.battleData.dyncDiffType = levelCfg.dyncDiffType;
  //      }

  //      //组队信息,初始化组队列表
  //      int[] teamGroup = new int[proto.data.Count];
		//for (int i = 0; i < teamGroup.Length; i++)
  //      {
		//	teamGroup [i] = i;
		//}

  //      //解析组队信息
  //      // fixed add by ljp 测试代码
  //      String Group = proto.group;
  //      if(BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_1v1 )
  //      {
  //          Group = "0|1";
  //      }
  //      else if (BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_2V2)
  //      {
  //          Group = RandomTeamMember();
  //      }
  //      else if (BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_1v1v1)
  //      {
  //          Group = "0|1|2";
  //      }
  //      else if (BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_1v1v1v1)
  //      {
  //          Group = "0|1|2|3";
  //      }
  //      else if (BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_2vPC)
  //      {
  //          Group = "0,1|";
  //      }
  //      else if (BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_3vPC)
  //      {
  //          Group = "0,1,2|";
  //      }
  //      else if (BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_4vPC)
  //      {
  //          Group = "0,1,2,4|";
  //      }
  //      if (!string.IsNullOrEmpty (Group))
  //      {
  //          string[] strGroup = Group.Split ('|');
		//	for (int i = 0; i < strGroup.Length; i++)
  //          {
		//		string[] teams              = strGroup [i].Split (',');
		//		for (int j = 0; j < teams.Length; j++)
  //              {
		//			int userIndex           = int.Parse (teams [j]);
		//			teamGroup [userIndex]   = i;
		//		}
		//	}
		//	BattleSystem.Instance.battleData.teamFight = true;
		//}
  //      else
  //      {
		//	BattleSystem.Instance.battleData.teamFight = false;
		//}


  //      if( proto.match_type == MatchType.MT_Sing )
  //      {
  //          if (proto.data.Count > 1)
  //              BattleSystem.Instance.battleData.useCommonEndCondition = true;
  //          else
  //              BattleSystem.Instance.battleData.useCommonEndCondition = false;
  //      }
		//bool containLocalTeam = false;

		//// 获取玩家数据
		//NetMessage.UserData ud;
		//int index = 0;
		//// 正常选手
		//for (; index < proto.data.Count; ++index) {
		//	ud        = proto.data [index];
		//	TEAM team = (TEAM)(index + 1);
  //          if (proto.match_type == MatchType.MT_Sing )
  //          {
  //              team  = (TEAM)(ud.userid > 0 ? ud.userid : -ud.userid);
  //          }
  //          Team t    = BattleSystem.Instance.sceneManager.teamManager.GetTeam (team);
		//	// 设置组队group
		//	t.groupID = teamGroup [index];
		//	// 设置队伍数据
		//	t.playerData.Init (ud);
		//	if (ud.userid > 0)
  //          {
  //              if (proto.match_type == MatchType.MT_Sing)
  //              {
  //                  BattleSystem.Instance.battleData.currentTeam = team;
  //                  containLocalTeam = true;
  //              }

  //              else
  //              {
  //                  if (LocalPlayer.Get().playerData.userId == ud.userid)
  //                  {
  //                      BattleSystem.Instance.battleData.currentTeam = team;
  //                      containLocalTeam = true;
  //                  }
  //              }
		//		t.aiEnable = false;
		//	}
  //          else
  //          {
  //              // 所有队伍都初始化ai数据
  //              if (proto.match_type == MatchType.MT_Sing)
  //              {
  //                  // pve 回放 level 存放的是 team
  //                  aiStrategy = mapConfig.teamAiType[(int)team];
  //              }
  //              else
  //              {
  //                  //多人匹配时,先根据地图配置中的队伍ai组进行设置,如果没有设置,则取网络消息中的全局ai类型
  //                  aiStrategy = mapConfig.teamAiType[(int)team];
  //                  if (aiStrategy < 0)
  //                      aiStrategy = AIStrategyConfigProvider.Instance.GetAIStrategyByName(proto.ai_type);
  //              }
  //              if (aiStrategy < 0) aiStrategy = BattleSystem.Instance.battleData.aiStrategy;
  //              //BattleSystem.Instance.sceneManager.aiManager.AddAI(t, aiStrategy, t.playerData.level, BattleSystem.Instance.battleData.aiLevel, proto.ai_param);

  //              if (proto.match_type == MatchType.MT_Sing)
  //              {
  //                  t.playerData.name = ud.name;
  //                  t.playerData.icon = ud.icon;
  //              }
  //              else
  //              {
  //                  #if !SERVER
  //                  //t.playerData.name = AIManager.GetAIName (t.playerData.userId);
		//		    //t.playerData.icon = AIManager.GetAIIcon (t.playerData.userId);
		//		    #endif
  //              }
                   
		//		t.aiEnable = true;

		//		//Log ("加入了AI：{0}, 类型：{1}", t.playerData.name, BattleSystem.Instance.sceneManager.aiManager.aiData[(int)team].aiStrategy);
		//	}
  //      }

		//if (!containLocalTeam)
  //      {
		//	// 不包含当前队伍，则认为当前队伍为空
		//	// 此时，有两种情况，A播放热门战报，B观战
		//	BattleSystem.Instance.battleData.gameState = GameState.Watcher;
		//}
  //      else
  //      {
		//	BattleSystem.Instance.battleData.gameState = GameState.Game;
		//}

		//// 预加载资源
		//AssetManager.Get ().LoadBattleResources ();
		// 预加载特效
		//EffectManager.Get ().PreloadEffect ();
	}

	/////////////////////////////////////
	/// 此处在OnReady之后应该增加一个客户端加载完成后发送给服务器的消息，告知服务器本地准备完毕，可以开始游戏
	/// 作用：在速度慢的机器上，能够让大家在321倒计时界面不卡顿，顺利并且同时进入。
	/////////////////////////////////////

	/// <summary>
	/// 战斗开始
	/// </summary>
	/// <param name="msgID">Message I.</param>
	/// <param name="msgBody">Message body.</param>
	public void OnNetStartBattle (int msgID, PacketEvent msgBody)
	{
        MemoryStream ms = msgBody.Data as MemoryStream;
        NetMessage.SCStartBattle proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCStartBattle>(ms);
        OnNetStartBattle(proto);
    }

    public void OnNetStartBattle(NetMessage.SCStartBattle proto )
    {
        BattleSystem.Instance.sceneManager.Reset();

        // 获取玩家数据
        MapConfig matchMap = MapConfigProvider.Instance.GetData(BattleSystem.Instance.battleData.matchId);
        List<int> userIdList = new List<int>();
        List<int> teamList = new List<int>();
        for (int i = 0; i < matchMap.player_count; ++i)
        {
            TEAM team = (TEAM)(i + 1);
            Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam(team);
            t.StartTeam();
            teamList.Add(i + 1);
            userIdList.Add(t.playerData.userId);
        }


        /// 随机队伍到不同的星球
        // 创建地图
        BattleSystem.Instance.sceneManager.CreateScene(teamList, false);
        if (BattleSystem.Instance.battleData.useAI)
        {
            //BattleSystem.Instance.sceneManager.aiManager.Start(1);
        }
        else
        {
            BattleSystem.Instance.battleData.useAI = true;
        }

        UISystem.Get().HideAllWindow();
        UISystem.Get().ShowWindow("PreviewWindow");

        // lockstep 模式
        BattleSystem.Instance.lockStep.replay = BattleSystem.Instance.battleData.isReplay;
        BattleSystem.Instance.lockStep.playSpeed = 1;
    }

    /// <summary>
    /// 网络帧同步
    /// </summary>
    /// <param name="msgID">Message I.</param>
    /// <param name="msgBody">Message body.</param>
    public void OnNetFrame (int msgID, PacketEvent msgBody)
	{
        MemoryStream ms          = msgBody.Data as MemoryStream;
        NetMessage.SCFrame frame = ProtoBuf.Serializer.Deserialize<NetMessage.SCFrame>(ms);
        OnNetFrame(frame );
        return;

	}

    public void OnNetFrame(NetMessage.SCFrame frame )
    {
        BattleSystem.Instance.OnRecievedFramePacket(frame);
        return;
    }
    /// <summary>
    /// 结束战斗回复
    /// </summary>
    /// <param name="msgID">Message I.</param>
    /// <param name="msgBody">Message body.</param>
    void OnFinishBattle (int msgID, PacketEvent msgBody)
	{
		Log ("OnFinishBattle");
		#if SERVER
		BattleSystem.Instance.battleData.RefereeBusy = false;
		#endif

		#if !SERVER
        MemoryStream ms          = msgBody.Data as MemoryStream;
        NetMessage.SCFinishBattle proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCFinishBattle>(ms);
		for (int i = 0; i < proto.users.Count; ++i) {
			Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam (proto.users [i]);
			if (t == null)
				continue;
			t.scoreMod = proto.score_mods [i];
			t.resultOrder = -3 + i;
			t.resultRank = proto.rank [i];
			t.resultEndtype = proto.end_type [i];
			if (proto.mvp_num.Count > 0) {
				t.leagueMvp = proto.mvp_num [i];
			}
		}

		EventSystem.Instance.FireEvent (EventId.OnFinished, proto);
		#endif
	}



	/// <summary>
	/// 发送移动消息
	/// </summary>
	/// <param name="from">From.</param>
	/// <param name="to">To.</param>
	/// <param name="rate">Rate.</param>
	public void SendMoveMessaeg (/*Node from, Node to, float rate = -1f*/)
	{
		#if !SERVER

		//if (BattleSystem.Instance.battleData.gameState != GameState.Game)
		//	return;

		//if (from == null || to == null)
		//	return;

		//int shipNum = from.GetShipCount ((int)BattleSystem.Instance.battleData.currentTeam);
		//if (shipNum == 0)
		//	return;
		//FramePacket packet = new FramePacket ();
		//packet.type = 0;
		//packet.move = new MovePacket ();
		//packet.move.from = from.tag;
		//packet.move.to = to.tag;
		//packet.move.rate = rate == -1f ? BattleSystem.Instance.battleData.sliderRate : rate;

		//byte[] byteString           = Json.EnCodeBytes(packet);
  //      NetMessage.PbFrame pb       = new NetMessage.PbFrame();
  //      NetMessage.CSFrame build    = new NetMessage.CSFrame();
  //      pb.content  = byteString;
  //      build.frame = pb;

  //      NetSystem.Instance.Send<NetMessage.CSFrame>((int)NetMessage.MsgId.ID_CSFrame, build);
		#endif
	}


	/// <summary>
	/// 发送技能网络包
	/// </summary>
	public void SendSkillPacket (int skillId)
	{
		#if !SERVER
		if (BattleSystem.Instance.battleData.gameState != GameState.Game)
			return;

		FramePacket packet = new FramePacket ();
		packet.type = 1;
		packet.skill = new SkillPacket ();
		packet.skill.skillID = skillId;
		packet.skill.from = BattleSystem.Instance.battleData.currentTeam;
		packet.skill.to = TEAM.Neutral;
		packet.skill.tag = string.Empty;

		byte[] byteString = Json.EnCodeBytes(packet);

		NetMessage.PbFrame pb = new NetMessage.PbFrame ();
		NetMessage.CSFrame build = new NetMessage.CSFrame ();

		pb.content  = byteString;
		build.frame =  pb;
		NetSystem.Instance.Send<NetMessage.CSFrame> ((int)NetMessage.MsgId.ID_CSFrame, build);
		#endif
	}

	/// <summary>
	/// 向服务器请求玩家数据，如果本地无账号则新建账号
	/// </summary>
	/// <param name="needChangeAccount">If set to <c>true</c> need change account.</param>
	public void RequestUser (bool needChangeAccount = false)
	{
        string account = LocalPlayer.Get ().GetLocalAccount ();
		if (string.IsNullOrEmpty (account)) {
			account = LocalPlayer.Get ().GenerateLocalAccount ();
		} else if (needChangeAccount) {
			account = LocalPlayer.Get ().GenerateLocalAccount ();
		}

        NetMessage.CSGetUserData proto = new NetMessage.CSGetUserData();
        proto.account       = account;
        proto.app_version   = UpdateSystem.Instance.GetAppVersion ();
        proto.imei_md5      = EngineSystem.Instance.GetUUID ();
        proto.channel       = ThirdPartySystem.Instance.GetChannel ();
        proto.device_model  = EngineSystem.Instance.GetDeviceModel ();
        proto.os_version    = EngineSystem.Instance.GetOS ();
        NetSystem.Instance.Send<NetMessage.CSGetUserData> ((int)NetMessage.MsgId.ID_CSGetUserData, proto);
    }

    /// <summary>
    /// 请求玩家数据的数据处理
    /// </summary>
    private void OnRequestUser(int msgId, PacketEvent msg)
    {
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCGetUserData proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCGetUserData>(ms);
        if (proto.errcode == ErrCode.EC_Ok || proto.errcode == ErrCode.EC_NeedResume)
        {// 如果成功，则包含玩家数据
            NetMessage.UserData ud = proto.data;
            // 这样子就把数据赋值到内存中吧，放在BattleManager中存着先
            LocalPlayer.Get().playerData.Init(ud);
            LocalPlayer.Get().playerData.InitRace(proto.race);
            LocalPlayer.Get().isAccountTokenOver = false;
            // 登录统计
            ThirdPartySystem.Instance.OnLogin();
        }

        if (!string.IsNullOrEmpty(proto.cur_level))
        {
            // 当前购买关卡
            string str                                      = proto.cur_level;
            LocalPlayer.Get().playerData.singleFightLevel   = str;
            // 如果本地记录保存有数据，则将本地数据设置为记录，并重新上传服务器。
            string localLevel                               = LocalAccountStorage.Get().singleCurrentLevel;
            if (!string.IsNullOrEmpty(localLevel))
            {
                LocalPlayer.Get().playerData.singleFightLevel = localLevel;

            }
        }
        if (proto.now > 0 )
        {
            // 设置服务器时间
            TimeSystem.Instance.SetServerTime(proto.now );
        }


        #if !SERVER
        EventSystem.Instance.FireEvent(EventId.RequestUserResult, proto.errcode);
        #endif
    }


    /// <summary>
    /// 请求虚拟角色单机
    /// </summary>
    /// <param name="needChangeAccount"></param>
    public void RequestUserSingle( bool bFire = true )
    {
        string account = LocalPlayer.Get().GetLocalAccount();
        if (string.IsNullOrEmpty(account))
        {
            account     = LocalPlayer.Get().GenerateLocalAccount();
        }

        /// Init LocalPlayer userid = 0
        {
            int index       = UnityEngine.Random.Range(0, 10);
            string icon     = "";
            PlayerData pd   = new PlayerData();

            if (bFire)
            {
                pd.userId   = 1;
            }

            pd.icon         = LocalAccountStorage.Get().icon;
            pd.name         = LocalAccountStorage.Get().name;
            LocalPlayer.Get().playerData.Init(pd);
            LocalPlayer.Get().isAccountTokenOver = false;

            // 处理离线时间,暂时不考虑时间作弊的情况,策划要求
            //long timeStamp = LocalAccountStorage.Get().timeStamp;
            //DateTime Start = new DateTime(1970, 1, 1);
            //Start = Start.AddSeconds(timeStamp);
            //TimeSpan ts = DateTime.Now - Start;
            //LocalPlayer.Get().OperatorOffLine((int)ts.TotalSeconds);
        }


        /// 金钱交验, 冲值得金钱本地无法交验
        //{
        //    int nCheckMoney = LocalAccountStorage.Get().GetDailysRewardMoney() +
        //                      LevelDataHandler.Get().GetLevelsRewardMoney();

        //    int nBuyChapter = LevelDataHandler.Get().GetBuyChapterMoney();
        //    int nCurMoney   = LocalPlayer.Get().playerData.money;
        //    if( nCurMoney > nCheckMoney - nBuyChapter )
        //    {
        //        LocalPlayer.Get().playerData.money = nCheckMoney;
        //        LocalAccountStorage.Get().money    = nCheckMoney;
        //    }
        //}

        // init level
        {
            // 如果本地记录保存有数据，则将本地数据设置为记录，并重新上传服务器。
            string localLevel = LocalAccountStorage.Get().singleCurrentLevel;
            string guideLevel = LocalAccountStorage.Get().guideFightLevel;
            LocalPlayer.Get().playerData.singleFightLevel = localLevel;
        }

        if (bFire)
        {
            EventSystem.Instance.FireEvent(EventId.RequestUserResult, 1);
        }
    }

    
	/// <summary>
	/// 注册新玩家网络
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="iconPath">Icon path.</param>
	public void CreateUser (string name, string iconPath)
	{
		string account = LocalPlayer.Get ().GetLocalAccount ();
		if (string.IsNullOrEmpty (account)) {
			Debug.LogError ("注册时本地账号为空");
		}

		NetMessage.CSCreateUserData proto = new CSCreateUserData();
		proto.account = account;
		proto.name    = name;
		proto.icon    = iconPath;
		NetSystem.Instance.Send<NetMessage.CSCreateUserData>((int)NetMessage.MsgId.ID_CSCreateUserData, proto);
	}


    /// <summary>
    /// 创建玩家的数据处理
    /// </summary>
    /// <param name="msgId">Message identifier.</param>
    /// <param name="msg">Message.</param>
    private void OnCreateUser (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCCreateUserData proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCCreateUserData>(ms);
        if (proto.errcode == ErrCode.EC_Ok) {// 如果成功，则包含玩家数据
			NetMessage.UserData ud = proto.data;
			LocalPlayer.Get ().playerData.Init (ud);

			// 注册统计
			ThirdPartySystem.Instance.OnRegister ();
		}

		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.CreateUserResult, proto.errcode);
		#endif
	}

    public void CreateUserSingle(string name, string iconPath)
    {
        string account = LocalPlayer.Get().GetLocalAccount();
        if (string.IsNullOrEmpty(account))
        {
            Debug.LogError("注册时本地账号为空");
        }

        LocalAccountStorage.Get().name = name;
        LocalAccountStorage.Get().icon = iconPath;
        EventSystem.Instance.FireEvent(EventId.CreateUserResult, 1);
    }

    private void OnSyncUserData (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCSyncUserData proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCSyncUserData>(ms);
        if (proto != null) {
			LocalPlayer.Get ().playerData.level = proto.data.level;
			LocalPlayer.Get ().playerData.score = proto.data.score;
		}

		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.SyncUserData);
		#endif
	}

	/// <summary>
	/// 切换头像
	/// </summary>
	/// <param name="iconPath">Icon path.</param>
	public void ChangeIcon (string iconPath)
	{
		// 设置本地的头像
		LocalPlayer.Get ().playerData.icon = iconPath;
		NetMessage.CSSetIcon proto = new NetMessage.CSSetIcon();
		proto.icon = iconPath;
		NetSystem.Instance.Send<NetMessage.CSSetIcon> ((int)NetMessage.MsgId.ID_CSSetIcon, proto);
	}

	/// <summary>
	/// 统一的errcode回复处理
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	private void OnErrCode (int msgId, PacketEvent msg)
	{

	}

	public void PingNet ()
	{
		NetMessage.CSPing proto = new CSPing();
		proto.timestamp = DateTime.Now.ToBinary ();
		NetSystem.Instance.Send<NetMessage.CSPing> ((int)NetMessage.MsgId.ID_CSPing, proto);
	}

	/// <summary>
	/// 心跳检测
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	private void OnPong (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCPong proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCPong>(ms);
		TimeSpan ts = DateTime.Now - DateTime.FromBinary (proto.timestamp);
		float millseconds = (float)ts.TotalMilliseconds;
		// 发送给界面
		NetSystem.Instance.ping.Pong (millseconds);

		EventSystem.Instance.FireEvent (EventId.PingRefresh, millseconds);
		#endif
	}

	/// <summary>
	/// 请求排行数据
	/// </summary>
	/// <param name="start">Start.</param>
	public void LoadRank (int start)
	{
		NetMessage.CSLoadRank proto = new CSLoadRank();
		proto.start = start;
		NetSystem.Instance.Send<NetMessage.CSLoadRank> ((int)NetMessage.MsgId.ID_CSLoadRank, proto);
	}

	private void OnLoadRank (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCLoadRank proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCLoadRank>(ms);

		List <PlayerData> rankData = new List<PlayerData> ();
		for (int i = 0; i < proto.data.Count; ++i) {
			PlayerData pd = new PlayerData ();
			pd.Init (proto.data [i]);
			rankData.Add (pd);
		}

		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.LoadRankList, rankData, proto.start, proto.self);
		#endif
	}

	/// <summary>
	/// 关注好友，follow是否关注
	/// </summary>
	public void FriendFollow (int userId, bool follow)
	{
		NetMessage.CSFriendFollow proto = new CSFriendFollow();
		proto.userid = userId;
		proto.follow = follow;
		NetSystem.Instance.Send<NetMessage.CSFriendFollow>((int)NetMessage.MsgId.ID_CSFriendFollow, proto);
	}

	private void OnFriendFollow (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCFriendFollow proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCFriendFollow>(ms);
  
		#if !SERVER
		if (proto.err == ErrCode.EC_Ok) {
            SimplePlayerData d = new SimplePlayerData();
            d.Init(proto.data);
            if (proto.follow )
            {
                bool bFollow = FriendDataHandler.Get().IsIsFollowEX(d.userId);
                FriendDataHandler.Get().AddMyFollow(d, bFollow);
            }
            else
            {
                FriendDataHandler.Get().DelMyFollow(d);
                FriendDataHandler.Get().SetMutualFollow(d.userId, false);
            }
        }
		EventSystem.Instance.FireEvent (EventId.OnFriendFollowResult, proto.userid, proto.follow, proto.err);
     
		#endif
	}

	/// <summary>
	/// 加载关注列表，cared：true表示自己关注的人，false表示关注自己的人
	/// </summary>
	public void FriendLoad (int start, bool myfollow)
	{
		NetMessage.CSFriendLoad proto = new CSFriendLoad ();
		proto.start     = start;
		proto.myfollow  = myfollow;
		NetSystem.Instance.Send<NetMessage.CSFriendLoad> ((int)NetMessage.MsgId.ID_CSFriendLoad, proto);
	}

	private void OnFriendLoad (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCFriendLoad proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCFriendLoad>(ms);
		List<SimplePlayerData> datas = new List<SimplePlayerData> ();
		for (int i = 0; i < proto.data.Count; ++i) {
			SimplePlayerData d = new SimplePlayerData ();
			d.Init (proto.data [i]);
            if (proto.myfollow)
            {
                FriendDataHandler.Get().AddMyFollow(d, proto.follow_status[i]);

            }
            else
            {
                FriendDataHandler.Get().AddFollower(d, proto.follow_status[i]);
            }
		}

		EventSystem.Instance.FireEvent (EventId.OnFriendLoadResult, proto.start, proto.myfollow);
		#endif
	}

	/// <summary>
	/// 推送被关注信息,mark:true被关注消息，false被取消关注消息
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	private void OnFriendNotify (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCFriendNotify proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCFriendNotify>(ms);
		SimplePlayerData data = new SimplePlayerData ();
		data.Init (proto.data);
        if( proto.follow )
        {
            bool bfollow = FriendDataHandler.Get().IsMyFollowEX(data.userId);
            FriendDataHandler.Get().AddFollower(data, bfollow);
            FriendDataHandler.Get().SetMutualMyFollow(data.userId, true);
        }
        else
        {
            FriendDataHandler.Get().DelFollower(data);
            FriendDataHandler.Get().SetMutualMyFollow(data.userId, false);
        }

	    EventSystem.Instance.FireEvent (EventId.OnFriendNotifyResult, data, proto.follow);
		#endif
	}

	public void FriendRecommend (int start)
	{
		NetMessage.CSFriendRecommend proto = new CSFriendRecommend();
		proto.start = start;
		NetSystem.Instance.Send<NetMessage.CSFriendRecommend>((int)NetMessage.MsgId.ID_CSFriendRecommend, proto);
	}

	private void OnFriendRecommend (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCFriendRecommend proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCFriendRecommend>(ms);
        
		List<SimplePlayerData> datas = new List<SimplePlayerData> ();
		for (int i = 0; i < proto.data.Count; ++i) {
			SimplePlayerData d = new SimplePlayerData ();
			d.Init (proto.data [i]);
			datas.Add (d);
		}
		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnFriendRecommendResult, proto.start, datas);
		#endif
	}

	public void FriendSearch (string name, int userId)
	{
		NetMessage.CSFriendSearch proto = new CSFriendSearch();
		proto.name      = name;
		proto.userid    = userId;
		NetSystem.Instance.Send<NetMessage.CSFriendSearch> ((int)NetMessage.MsgId.ID_CSFriendSearch, proto);
	}

	private void OnFriendSearch (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCFriendSearch proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCFriendSearch>(ms);
        
		SimplePlayerData data = null;
		bool follow = false;
        int follownum = 0;
        int fensinum  = 0;
        int mvpnum = 33;
        int battlenum = 89;
		if (proto.data != null ) {
			data = new SimplePlayerData ();
			data.Init (proto.data);
			follow      = proto.followed;
            follownum   = proto.following_count;
            fensinum    = proto.followers_count;
            mvpnum      = proto.data.mvp_count;
            battlenum   = proto.data.battle_count;
		}

		#if !SERVER
        EventSystem.Instance.FireEvent(EventId.OnFriendSearchResult, data, follow, follownum, fensinum, mvpnum, battlenum );
		#endif
	}

	/// <summary>
	/// 创建队伍
	/// </summary>
	/// <param name="is2v2">If set to <c>true</c> is2v2.</param>
	/// <param name="is3v3">If set to <c>true</c> is3v3.</param>
	public void TeamCreate (bool is2v2, bool is3v3)
	{
		//#if !SERVER
		//TeamInviteData.Get ().Reset ();
		//NetMessage.CSTeamCreate proto = new CSTeamCreate();
		//if (is2v2) {
		//	proto.type = BattleType.Group2v2;
		//} else if (is3v3) {
		//	proto.type = BattleType.Group3v3;
		//}

		//NetSystem.Instance.Send <NetMessage.CSTeamCreate>((int)NetMessage.MsgId.ID_CSTeamCreate, proto);
		//#endif
	}

	/// <summary>
	/// 创建队伍回复
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	private void OnTeamCreate (int msgid, PacketEvent msg)
	{
  //      MemoryStream ms = msg.Data as MemoryStream;
  //      NetMessage.SCTeamCreate proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCTeamCreate>(ms);
        
		//#if !SERVER
		//// 组队创建房间成功
		//if (proto.code == NetMessage.ErrCode.EC_Ok) {
		//	TeamInviteData.Get ().isLeader = true;
		//	UISystem.Get ().HideWindow ("StartWindow");
		//	UISystem.Get ().ShowWindow ("TeamWindow");
		//} else {
		//	Tips.Make (Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(914), 1);
		//}
		//#endif
	}

	/// <summary>
	/// 邀请玩家
	/// </summary>
	/// <param name="userId">User identifier.</param>
	public void TeamInvite (int userId)
	{
		NetMessage.CSTeamInvite proto = new CSTeamInvite();
		proto.userId  = userId;
		NetSystem.Instance.Send<NetMessage.CSTeamInvite>((int)NetMessage.MsgId.ID_CSTeamInvite, proto);
	}

	/// <summary>
	/// 邀请对房主的反馈
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	private void OnTeamInvite (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCTeamInvite proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCTeamInvite>(ms);

		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnTeamInviteResult, proto.code, proto.userId);
		#endif
	}

	/// <summary>
	/// 房间信息更新
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	private void OnTeamUpdate (int msgId, PacketEvent msg)
	{
		//#if !SERVER
  //      MemoryStream ms = msg.Data as MemoryStream;
  //      NetMessage.SCTeamUpdate proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCTeamUpdate>(ms);
		
		//TeamInviteData.Get ().battleType = proto.type;
		//TeamInviteData.Get ().version    = proto.version;

		//TeamInviteData.Get ().teamPlayers.Clear ();
		//for (int i = 0; i < proto.simUsers.Count; ++i) {
		//	SimplePlayerData spd = new SimplePlayerData ();
		//	spd.Init (proto.simUsers [i]);
		//	TeamInviteData.Get ().teamPlayers.Add (spd);
		//}


		//EventSystem.Instance.FireEvent (EventId.OnTeamUpdate);
		//#endif
	}

	/// <summary>
	/// 离开队伍
	/// </summary>
	/// <param name="leaderId">Leader identifier.</param>
	public void TeamLeave (int leaderId)
	{
		NetMessage.CSTeamLeave proto = new CSTeamLeave();
		proto.leaderId = leaderId;
		NetSystem.Instance.Send<NetMessage.CSTeamLeave> ((int)NetMessage.MsgId.ID_CSTeamLeave, proto);
	}

	/// <summary>
	/// 队伍删除
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	private void OnTeamDelete (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCTeamDel proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCTeamDel>(ms);
        
		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnTeamDelete, proto.code, proto.leaderId);
		#endif
	}

	/// <summary>
	/// 组队开始
	/// </summary>
	public void TeamStart ()
	{
		NetMessage.CSTeamStart proto = new CSTeamStart ();
		NetSystem.Instance.Send<NetMessage.CSTeamStart> ((int)NetMessage.MsgId.ID_CSTeamStart, proto);
	}

	/// <summary>
	/// 组队开始回复
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	private void OnTeamStart (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCTeamStart proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCTeamStart>(ms);
        
		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnTeamStart, proto.code, proto.leaderId);
		#endif
	}

	/// <summary>
	/// 对被邀请人的邀请提示
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	private void OnTeamInviteReq (int msgId, PacketEvent msg)
	{
		//#if !SERVER
  //      MemoryStream ms = msg.Data as MemoryStream;
  //      NetMessage.SCTeamInviteReq proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCTeamInviteReq>(ms);
		
		//TeamInviteData.Get ().Reset ();
		//SimplePlayerData leader = new SimplePlayerData ();
		//leader.Init (proto.leader);


		//// 此处应该弹出TeamInvite窗口
		//if (BattleSystem.Instance.battleData.gameState != GameState.Game) {
		//	UISystem.Get ().ShowWindow ("TeamNotifyWindow");
		//	EventSystem.Instance.FireEvent (EventId.OnTeamInviteRequest, proto.type, leader, proto.timestamp);
		//}
		//#endif
	}

	/// <summary>
	/// 被邀请人对邀请的操作
	/// </summary>
	/// <param name="leaderId">Leader identifier.</param>
	/// <param name="timeStamp">Time stamp.</param>
	public void TeamInviteResponse (bool accept, int leaderId, int timeStamp)
	{
		NetMessage.CSTeamInviteResp proto = new CSTeamInviteResp();
		proto.leaderId =  leaderId;
		proto.timestamp=  timeStamp;
		proto.accept   =  accept;

		NetSystem.Instance.Send<NetMessage.CSTeamInviteResp> ((int)NetMessage.MsgId.ID_CSTeamInviteResp, proto);
	}

	/// <summary>
	/// 邀请的结果
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	private void OnTeamInviteResponse (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCTeamInviteResp proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCTeamInviteResp>(ms);

		// 此消息两个地方都会受到，队长收到，表示玩家对邀请的反馈
		// 被邀请玩家收到，根据状态，成功就打开组队

		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnTeamInviteResponse, proto.code, proto.userId);
		#endif
	}

	/// <summary>
	/// 请求战报
	/// </summary>
	public void BattleReportLoad (bool self, int start)
	{
		NetMessage.CSBattleReportLoad proto = new CSBattleReportLoad ();
		proto.self = self;
		proto.start = start;
		NetSystem.Instance.Send<NetMessage.CSBattleReportLoad> ((int)NetMessage.MsgId.ID_CSBattleReportLoad, proto);
	}

	/// <summary>
	/// 加载战报回复
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	private void OnBattleReportLoad (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCBattleReportLoad proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCBattleReportLoad>(ms);

		List<BattleReportData> reportList = new List<BattleReportData> ();
		for (int i = 0; i < proto.report.Count; ++i) {
			BattleReportData brd = new BattleReportData ();
			brd.Init (proto.report [i]);
			reportList.Add (brd);
		}

		EventSystem.Instance.FireEvent (EventId.OnBattleReportLoad, false, proto.start, reportList);
		#endif
	}

	/// <summary>
	/// 请求播放战报
	/// </summary>
	public void BattleReportPlay (BattleReportData brd)
	{

		NetMessage.CSBattleReportPlay proto = new CSBattleReportPlay ();
		proto.battleid = brd.id;
		NetSystem.Instance.Send<NetMessage.CSBattleReportPlay> ((int)NetMessage.MsgId.ID_CSBattleReportPlay, proto);
	}

	/// <summary>
	/// 播放战报回复
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	public void onBattleReportPlay (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCBattleReportPlay proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCBattleReportPlay>(ms);
		PbSCFrames frames = proto.report;
		AssetManager.Get ().LoadBattleResources ();
		EventSystem.Instance.FireEvent (EventId.OnBattleReportPlay, frames);
		#endif
	}

	
	/// <summary>
	/// 请求返回战斗
	/// </summary>
	public void BattleResume (int frame)
	{
		NetMessage.CSBattleResume proto = new CSBattleResume ();
		proto.startFrameNo = frame;

		NetSystem.Instance.Send<NetMessage.CSBattleResume> ((int)NetMessage.MsgId.ID_CSBattleResume, proto);
	}

	/// <summary>
	/// 返回战斗包，这个包中可能包含ready和start，但是不是必须，需要根据当前状态进行判断进行何种操作。
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	public void OnBattleResume (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCBattleResume proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCBattleResume>(ms);
       
		#if !SERVER
		PbSCFrames frames = proto.report;

		AssetManager.Get ().LoadBattleResources ();

		// 此时需要判断当前是否在战斗中
		GameState state = BattleSystem.Instance.battleData.gameState;
		if (state == GameState.Game || state == GameState.GameWatch || state == GameState.Watcher) {
			// 战斗中只添加frame包
			for (int i = 0; i < frames.frames.Count; ++i) {
				SCFrame scf = frames.frames [i];
				OnNetFrame (scf);
			}
			// 直接执行到最后
			BattleSystem.Instance.lockStep.RunToFrame (BattleSystem.Instance.GetCurrentFrame () / 5 + frames.frames.Count);
			BattleSystem.Instance.sceneManager.SilentMode (true);
			BattleSystem.Instance.battleData.resumingFrame = (BattleSystem.Instance.GetCurrentFrame () / 5 + frames.frames.Count) * 5;
			UISystem.Instance.ShowWindow ("ResumingWindow");
		}
        else {
			BattleSystem.Instance.SetPlayMode (true, false);
			// 从ready开始解包
			OnReady (frames.ready);
			// start
			OnNetStartBattle (frames.start);
			// frame
			for (int i = 0; i < frames.frames.Count; ++i) {
				SCFrame scf = frames.frames [i];
				OnNetFrame (scf);
			}

            int FramesCount = frames.frames.Count;
            BattleSystem.Instance.lockStep.runFrameCount = 20;
			// 直接执行到最后
			BattleSystem.Instance.lockStep.RunToFrame (FramesCount);
			BattleSystem.Instance.sceneManager.SilentMode (true);
			BattleSystem.Instance.battleData.resumingFrame = (FramesCount + FramesCount / 200) * 5;
		}
		if (UISystem.Get ().IsWindowVisible ("CommonDialogWindow")) {
			UISystem.Get ().HideWindow ("CommonDialogWindow");
		}

		#endif
	}

	/// <summary>
	/// 获取锦标赛详情
	/// </summary>
	public void RequestLeagueInfo ()
	{
		NetMessage.CSGetLeagueInfo proto = new CSGetLeagueInfo();
		NetSystem.Instance.Send<NetMessage.CSGetLeagueInfo> ((int)NetMessage.MsgId.ID_CSGetLeagueInfo, proto);
	}

	/// <summary>
	/// 锦标赛详情回复
	/// </summary>
	void OnGetLeagueInfo (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCGetLeagueInfo proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCGetLeagueInfo>(ms);
		
		if (proto.code != ErrCode.EC_Ok) {
			// 不在锦标赛中，请求锦标赛列表
			RequestLeagueList (0);
		}
        else
        {
			EventSystem.Instance.FireEvent (EventId.OnGetLeagueInfoResult, proto.league, proto.self);
		}
		#endif
	}

	/// <summary>
	/// 获取锦标赛列表
	/// </summary>
	/// <param name="leagueID">League I.</param>
	public void RequestLeagueList (int index)
	{
		NetMessage.CSLeagueList proto = new CSLeagueList();
		proto.start = index;

		NetSystem.Instance.Send<NetMessage.CSLeagueList> ((int)NetMessage.MsgId.ID_CSLeagueList, proto);
	}

	/// <summary>
	/// 锦标赛列表回复
	/// </summary>
	void OnGetLeagueList (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCLeagueList proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCLeagueList>(ms);
	
		EventSystem.Instance.FireEvent (EventId.OnLeagueListResult, proto.start, proto.league_info);
		#endif
	}

	/// <summary>
	/// 报名
	/// </summary>
	/// <param name="leagueID">League I.</param>
	public void RequestLeagueSignUp (string leagueID)
	{
		NetMessage.CSLeagueAdd proto = new CSLeagueAdd ();
		proto.league_id = leagueID;

		NetSystem.Instance.Send<NetMessage.CSLeagueAdd> ((int)NetMessage.MsgId.ID_CSLeagueAdd, proto);
	}

	/// <summary>
	/// 报名锦标赛回复
	/// </summary>
	void OnLeagueSignUp (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCLeagueAdd proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCLeagueAdd>(ms);
	
	
		// 报名成功，则进入到自己的竞标赛状态
		if (proto.code == ErrCode.EC_Ok) {
			EventSystem.Instance.FireEvent (EventId.OnGetLeagueInfoResult, proto.league, proto.self);
		} else {
			EventSystem.Instance.FireEvent (EventId.OnLeagueSignUpResult, proto.self, proto.league_id);
		}
		#endif
	}

	/// <summary>
	/// 获得排名
	/// </summary>
	/// <param name="leagueID">League I.</param>
	/// <param name="index">Index.</param>
	public void RequestLeagueRank (string leagueID, int index)
	{
		NetMessage.CSLeagueRank proto = new CSLeagueRank ();
		proto.league_id = (leagueID);
		proto.start = (index);

		NetSystem.Instance.Send<NetMessage.CSLeagueRank> ((int)NetMessage.MsgId.ID_CSLeagueRank, proto);
	}

	/// <summary>
	/// 获得排名回复
	/// </summary>
	void OnLeagueRank (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCLeagueRank proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCLeagueRank>(ms);

		EventSystem.Instance.FireEvent (EventId.OnLeagueRankResult, proto.start, proto.members, proto.self, proto.self_rank);
		#endif
	}

	/// <summary>
	/// 锦标赛匹配
	/// </summary>
	/// <param name="leagueID">League I.</param>
	public void MatchLeague (string leagueID)
	{
		NetMessage.CSLeagueMatch proto = new CSLeagueMatch ();
		proto.league_id = (leagueID);

		NetSystem.Instance.Send<NetMessage.CSLeagueMatch> ((int)NetMessage.MsgId.ID_CSLeagueMatch, proto);
	}

	/// <summary>
	/// 锦标赛匹配回复,发完这个回复后紧接着发match
	/// </summary>
	void OnLeagueMatch (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCLeagueMatch proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCLeagueMatch>(ms);

		if (proto.code == ErrCode.EC_Ok) {
			UISystem.Get ().HideWindow ("CustomSelectWindowNew");
			UISystem.Get ().HideWindow ("LeagueWindow");
			FakeStartBattle ("PVP");
			UISystem.Get ().ShowWindow ("PVPWaitWindow");
		}

		EventSystem.Instance.FireEvent (EventId.OnLeagueMatchResult, proto.code, proto.count_down);

		#endif
	}

	/// <summary>
	/// 退出league
	/// </summary>
	public void QuitLeague ()
	{
		NetMessage.CSLeagueQuit proto = new CSLeagueQuit ();
		NetSystem.Instance.Send<NetMessage.CSLeagueQuit> ((int)NetMessage.MsgId.ID_CSLeagueQuit, proto);
	}

	void OnLeagueQuitResult (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCLeagueQuit proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCLeagueQuit>(ms);

		if (proto.code == ErrCode.EC_Ok) {
			RequestLeagueInfo ();
		} else {
			Tips.Make (LanguageDataProvider.GetValue(752));
		}
		#endif
	}

	/// <summary>
	/// Requests the room list.
	/// </summary>
	/// <param name="players">Players.</param>
	/// <param name="start">Start.</param>
	public void RequestRoomList (int players)
	{
		NetMessage.CSWatchRooms proto = new CSWatchRooms ();
		proto.playernum = (players);
		NetSystem.Instance.Send<NetMessage.CSWatchRooms> ((int)NetMessage.MsgId.ID_CSWatchRooms, proto);
	}

	/// <summary>
	/// Raises the request room list event.
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnRequestRoomList (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCWatchRooms proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCWatchRooms>(ms);

		#if!SERVER
		EventSystem.Instance.FireEvent (EventId.OnGetRoomList, proto.rooms);
		#endif
	}

	public void RequestUnWatchRooms ()
	{
		NetMessage.CSUnWatchRooms proto = new CSUnWatchRooms ();
		NetSystem.Instance.Send<NetMessage.CSUnWatchRooms> ((int)NetMessage.MsgId.ID_CSUnWatchRooms, proto);
	}

	/// <summary>
	/// Requests the join room.
	/// </summary>
	/// <param name="roomid">Roomid.</param>
	public void RequestJoinRoom (int roomid)
	{
		NetMessage.CSJoinRoom proto = new CSJoinRoom ();
		proto.roomid = (roomid);
		NetSystem.Instance.Send<NetMessage.CSJoinRoom>((int)NetMessage.MsgId.ID_CSJoinRoom, proto);
	}

	/// <summary>
	/// Raises the request join room event.
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnRequestJoinRoom (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCJoinRoom proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCJoinRoom>(ms);

        if (proto.code != ErrCode.EC_Ok)
			return;

		BattleSystem.Instance.battleData.matchId = proto.room.matchid;
		#if!SERVER
		EventSystem.Instance.FireEvent (EventId.OnJoinRoom, proto.data );
		#endif
	}

	/// <summary>
	/// Requests the create room.
	/// </summary>
	/// <param name="matchid">Matchid.</param>
	public void RequestCreateRoom (string matchid)
	{
		NetMessage.CSCreateRoom proto = new CSCreateRoom ();
		proto.matchid = (matchid);
		NetSystem.Instance.Send<NetMessage.CSCreateRoom> ((int)NetMessage.MsgId.ID_CSCreateRoom, proto);
	}

	/// <summary>
	/// Raises the request create room event.
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnRequestCreateRoom (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCCreateRoom proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCCreateRoom>(ms);

        if (proto.code != ErrCode.EC_Ok)
			return;
		BattleSystem.Instance.battleData.matchId = proto.room.matchid;
		#if!SERVER
		EventSystem.Instance.FireEvent (EventId.OnCreateRoom, proto.data);
		#endif
	}

	/// <summary>
	/// Requests the quit room.
	/// </summary>
	public void RequestQuitRoom ()
	{
		NetMessage.CSQuitRoom proto = new CSQuitRoom ();
		NetSystem.Instance.Send<NetMessage.CSQuitRoom> ((int)NetMessage.MsgId.ID_CSQuitRoom, proto);
	}

	/// <summary>
	/// Raises the request quit room event.
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnRequestQuitRoom (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCQuitRoom proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCQuitRoom>(ms);

        if (proto.code != ErrCode.EC_Ok)
			return;

		#if !SERVER
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("StartWindow");
		#endif
	}

	/// <summary>
	/// Rooms the refresh.
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void RoomRefresh (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCRoomRefresh proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCRoomRefresh>(ms);
        
		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnRoomRefresh, proto.data);
		#endif
	}

	void RoomListRefresh (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCRoomListRefresh proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCRoomListRefresh>(ms);
        
		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnRoomListREfresh, proto.roomid, proto.playernum);
		#endif
	}

	void OnMatch2CurNum (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCMatch2CurNum proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCMatch2CurNum>(ms);

		LocalPlayer.Get ().CurBattlePlayerNum = proto.playernum;

		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnRoomListREfresh, proto.playernum);
		#endif
	}

	public void MatchGame2 ()
	{
		BattleSystem.Instance.battleData.gameType = GameType.PVP;
		NetMessage.CSStartMatch2 proto = new CSStartMatch2 ();
		NetSystem.Instance.Send<NetMessage.CSStartMatch2> ((int)NetMessage.MsgId.ID_CSStartMatch2, proto);
	}

	void ONMatchGameBack (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCStartMatch2 proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCStartMatch2>(ms);

        if (proto.code != ErrCode.EC_Ok)
			return;
		#if !SERVER
		UISystem.Get ().ShowWindow ("WaitWindow");
		EventSystem.Instance.FireEvent (EventId.OnStartMatch2);
		UISystem.Get ().HideWindow ("SelectRaceWindow");
		#endif
	}

	/// <summary>
	/// 发送改变种族消息
	/// </summary>
	/// <param name="race">Race.</param>
	public void SendSelectRace (int index)
	{
		CSChangeRace builder = new CSChangeRace ();
		builder.race =  (index);
		NetSystem.Instance.Send<NetMessage.CSChangeRace> ((int)NetMessage.MsgId.ID_CSChangeRace, builder);
	}

	public void SendRaceEntry ()
	{
		CSCommitRace builder = new CSCommitRace ();
		NetSystem.Instance.Send<NetMessage.CSCommitRace> ((int)NetMessage.MsgId.ID_CSCommitRace, builder);
	}

	/// <summary>
	/// 请求匹配
	/// </summary>
	public void StartMatch3 (bool bHavRace = false)
	{
		BattleSystem.Instance.SetPlayMode (true, false);
		BattleSystem.Instance.battleData.gameType = GameType.PVP;
		StartMatchReq (MatchType.MT_Ladder, string.Empty, NetMessage.CooperationType.CT_1v1v1v1, 4);
	}

	/// <summary>
	/// 开始匹配
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnStarMatch3 (int msgid, PacketEvent msg)
	{
        //开始匹配
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCStartMatch3 message = ProtoBuf.Serializer.Deserialize<NetMessage.SCStartMatch3>(ms);

        
		#if !SERVER
		if (message.code != ErrCode.EC_Ok) {
			//错误信息
			Tips.Make (Tips.TipsType.FlowUp, string.Format (LanguageDataProvider.GetValue(111), message.code), 1);
			return;
		}

		// 开始模拟pvp进入的游戏局

        UISystem.Get().HideWindow("CustomSelectWindowNew");
		UISystem.Get ().HideWindow ("StartWindow");
		FakeStartBattle ("PVP");
		UISystem.Get ().ShowWindow ("PVPWaitWindow");
		#endif
	}

	/// <summary>
	/// 更新匹配信息
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnMatch3Notify (int msgid, PacketEvent msg)
	{
        //更新匹配信息
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCMatch3Notify message = ProtoBuf.Serializer.Deserialize<NetMessage.SCMatch3Notify>(ms);

        BattleSystem.Instance.battleData.matchId = message.matchid;
		LocalPlayer.Get ().battleMap = message.matchid;
		List<PlayerData> playerList = new List<PlayerData> ();

		playerList.Add (null);
		playerList.Add (null);
		playerList.Add (null);
		playerList.Add (null);

		for (int i = 0; i < message.user.Count; ++i) {
			PlayerData pd = new PlayerData ();
			pd.Init (message.user [i]);
			int index = message.useridx [i];

			playerList [index] = pd;

			if (pd.userId > 0) {
				// 真实玩家

			} else {
				// AI
				//pd.name = AIManager.GetAIName (pd.userId);
				//pd.icon = AIManager.GetAIIcon (pd.userId);
			}
		}

		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnMatch3Notify, playerList);
		#endif

		//记录其他玩家数据
		LocalPlayer.Get ().mathPlayer = playerList;
	}

	/// <summary>
	/// 改变种族成功 
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnChangeRace (int msgid, PacketEvent msg)
	{
        //变更种族
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCChangeRace message = ProtoBuf.Serializer.Deserialize<NetMessage.SCChangeRace>(ms);
        
		#if !SERVER
		if (message.code != ErrCode.EC_Ok) {
			Tips.Make (Tips.TipsType.FlowUp, string.Format (LanguageDataProvider.GetValue(111), message.code), 1);
			return;
		}

		//刷新自己
		//UISystem.Get ().OnEventHandler (EventId.OnRoomListREfresh, "SelectRaceWindow");
		#endif
	}

	/// <summary>
	/// 进入选择种族
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnStartSelectRace (int msgid,PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCStartSelectRace proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCStartSelectRace>(ms);
        OnStartSelectRace(proto);
    }


    void OnStartSelectRace (NetMessage.SCStartSelectRace proto )
	{
		BattleSystem.Instance.Reset ();

		#if !SERVER
		UISystem.Get ().ShowWindow ("SelectRaceWindow");
		UISystem.Get ().HideWindow ("PVPWaitWindow");

		EventSystem.Instance.FireEvent (EventId.OnStartSelectRace, proto.races);
		#endif
	}
	/// <summary>
	/// 更新房间信息
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnSelectRaceNotify (int msgid, PacketEvent msg)
	{
        //更新房间信息
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCSelectRaceNotify message = ProtoBuf.Serializer.Deserialize<NetMessage.SCSelectRaceNotify>(ms);
        OnSelectRaceNotify(message);
    }

    void OnSelectRaceNotify (NetMessage.SCSelectRaceNotify message)
	{
        //更新房间信息
		#if !SERVER
		//刷新自己
		EventSystem.Instance.FireEvent (EventId.OnRoomListREfresh, message.user, message.race, message.ok );
		#endif
		Debug.Log ("OnSelectRaceNotify");
	}

	/// <summary>
	/// 请求种族数据
	/// </summary>
	public void RequestRaceData ()
	{
		NetMessage.CSGetRaceData proto = new CSGetRaceData ();
		NetSystem.Instance.Send<NetMessage.CSGetRaceData> ((int)NetMessage.MsgId.ID_CSGetRaceData, proto);

	}


	public void RequestNoticeData ()
	{
		NetMessage.CSClientStorageLoad proto = new CSClientStorageLoad ();
		NetSystem.Instance.Send<NetMessage.CSClientStorageLoad> ((int)NetMessage.MsgId.ID_CSClientStorageLoad, proto);
	}

	public void RequestSetNoticeData (int idx, string value)
	{
		NetMessage.CSClientStorageSet proto = new CSClientStorageSet ();
		proto.index.Add (idx);
		proto.value.Add (value);
		//proto.SetValue(idx, value);
		NetSystem.Instance.Send<NetMessage.CSClientStorageSet> ((int)NetMessage.MsgId.ID_CSClientStorageSet, proto);
	}

	/// <summary>
	/// 种族信息回复
	/// </summary>
	private void OnGetRaceData (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCGetRaceData proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCGetRaceData>(ms);

		LocalPlayer.Get ().playerData.InitRace (proto.races);

		//UISystem.Get ().ShowWindow ("RaceWindow");
		EventSystem.Instance.FireEvent (EventId.OnGetRaceData, proto.races);


		#endif
	}

	/// <summary>
	/// 请求种族技能升级
	/// </summary>
	public void RequestRaceSkillLevelUp (int race, int skillIndex)
	{
		NetMessage.CSRaceSkillLevelUp proto = new CSRaceSkillLevelUp ();
		proto.cur_race = (race);
		proto.skill_index = (skillIndex);
		NetSystem.Instance.Send<NetMessage.CSRaceSkillLevelUp> ((int)NetMessage.MsgId.ID_CSRaceSkillLevelUp, proto);

	}

	/// <summary>
	/// 宗族技能升级回复
	/// </summary>
	private void OnRaceSkillLevelUp (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCRaceSkillLevelUp proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCRaceSkillLevelUp>(ms);

		EventSystem.Instance.FireEvent (EventId.OnRaceSkillLevelUp, proto.cur_race, proto.cur_race, proto.race_level, proto.skill_index, proto.new_skillid);
		#endif

	}

	/// <summary>
	/// 背包数据更新
	/// </summary>
	private void OnPackUpdate (int msgid, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCPackUpdate proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCPackUpdate>(ms);

		NetMessage.PackItem pi = null;
		for (int i = 0; i < proto.modified.Count; ++i) {
			pi = proto.modified [i];

			LocalPlayer.Get ().playerData.pack.ModifyItem (pi.itemid, pi.num);
		}
		#if !SERVER
		EventSystem.Instance.FireEvent (EventId.OnCoinSync);
		#endif
	}


	/// <summary>
	/// Starts the box.
	/// </summary>
	/// <param name="index">Index.</param>
	public void StartBox (int index)
	{
		NetMessage.CSUnlockChest proto = new CSUnlockChest ();
		proto.slot = index;

		NetSystem.Instance.Send<NetMessage.CSUnlockChest> ((int)NetMessage.MsgId.ID_CSUnlockChest, proto);
	}

	private void NotifyChestErrCode (ErrCode code)
	{
		#if !SERVER
		switch (code) {
		case ErrCode.EC_ChestLocked:
			Tips.Make (Tips.TipsType.FlowUp, LanguageDataProvider.GetValue (630), 1.0f);
			//UISystem.Get ().ShowDialogWindow ( 1, "宝箱未解锁");
			break;
		case ErrCode.EC_ChestNoExist:
			Tips.Make (Tips.TipsType.FlowUp, LanguageDataProvider.GetValue (631), 1.0f);
			//UISystem.Get ().ShowDialogWindow ( 1, "错误的宝箱");
			break;
		case ErrCode.EC_ChestJewelNotEnough:
			Tips.Make (Tips.TipsType.FlowUp, LanguageDataProvider.GetValue (632), 1.0f);
			//UISystem.Get ().ShowDialogWindow ( 1, "钻石不足");
			break;
		case ErrCode.EC_ChestTimeNotEnough:
			Tips.Make (Tips.TipsType.FlowUp, LanguageDataProvider.GetValue (633), 1.0f);
			//UISystem.Get ().ShowDialogWindow ( 1, "CD中……");
			break;
		case ErrCode.EC_ChestWinNumNotEnough:
			Tips.Make (Tips.TipsType.FlowUp, LanguageDataProvider.GetValue (634), 1.0f);
			//UISystem.Get ().ShowDialogWindow ( 1, "胜利场次不足");
			break;
		default:
			Tips.Make (Tips.TipsType.FlowUp, code.ToString (), 1.0f);
			//UISystem.Get ().ShowDialogWindow ( 1, proto.Code.ToString());
			break;
		}
		#endif
	}

	/// <summary>
	/// Raises the start box event.
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnStartBox (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCUnlockChest proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCUnlockChest>(ms);

		if (proto.code != ErrCode.EC_Ok) {
			NotifyChestErrCode (proto.code);
		} else {
			if (proto.time2unlock > 0 ) {
				int slot = proto.slot;
				ChessItem[] chests = LocalPlayer.Get ().playerData.chesses;
				if ((chests [slot].timeout = proto.time2unlock) > 0)
					chests [slot].timefinish = new DateTime (1970, 1, 1).AddSeconds (proto.time2unlock);
			}
			EventSystem.Instance.FireEvent (EventId.OnChestNotify);
		}
		#endif
	}

	/// <summary>
	/// Opens the box.
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="useGold">If set to <c>true</c> use gold.</param>
	public void OpenBox (int index, bool useGold = false)
	{
		NetMessage.CSGainChest proto = new CSGainChest ();
		proto.slot = index;
		proto.use_jewel = useGold;

		NetSystem.Instance.Send<NetMessage.CSGainChest> ((int)NetMessage.MsgId.ID_CSGainChest, proto);
	}

	/// <summary>
	/// Raises the open box event.
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnOpenBox (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCGainChest proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCGainChest>(ms);


		if (proto.code != ErrCode.EC_Ok) {

			NotifyChestErrCode (proto.code);

		} else {
			UISystem.Get ().ShowWindow ("ChestWindow");
			EventSystem.Instance.FireEvent (EventId.OnChestNotify, proto);
		}
		#endif
	}

	/// <summary>
	/// Raises the notify box event.
	/// </summary>
	/// <param name="msgid">Msgid.</param>
	/// <param name="msg">Message.</param>
	void OnNotifyBox (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCAddChest proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCAddChest>(ms);


        if (proto.added != null ) {

			int slot = proto.added.slot;
			LocalPlayer.Get ().playerData.chesses [slot] = new ChessItem ();
			LocalPlayer.Get ().playerData.chesses [slot].id = proto.added.id;
			if ((LocalPlayer.Get ().playerData.chesses [slot].timeout = proto.added.timeout) > 0)
				LocalPlayer.Get ().playerData.chesses [slot].timefinish = new DateTime (1970, 1, 1).AddSeconds (proto.added.timeout);
			LocalPlayer.Get ().playerData.chesses [slot].slot = proto.added.slot;
			EventSystem.Instance.FireEvent (EventId.OnChestNotify);
		}
		#endif
	}

	public void OpenLeftBox (int type)
	{
		if (type == 0) {
			NetMessage.CSGainTimerChest proto = new CSGainTimerChest ();

			NetSystem.Instance.Send<NetMessage.CSGainTimerChest> ((int)NetMessage.MsgId.ID_CSGainTimerChest, proto);
		} else {
			NetMessage.CSGainBattleChest proto = new CSGainBattleChest ();

			NetSystem.Instance.Send<NetMessage.CSGainBattleChest> ((int)NetMessage.MsgId.ID_CSGainBattleChest, proto);
		}
	}

	void OnOpenTimerChest (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCGainTimerChest proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCGainTimerChest>(ms);


		if (proto.code != ErrCode.EC_Ok) {

			NotifyChestErrCode (proto.code);
		} else {
			LocalPlayer.Get ().playerData.timechest = proto.time_out;
			Debug.Log (string.Format ("OnOpenTimerChest:{0}", proto.time_out));
			EventSystem.Instance.FireEvent (EventId.OnChestNotify);
			UISystem.Get ().ShowWindow ("ChestWindow");
			EventSystem.Instance.FireEvent (EventId.OnChestTime, proto);
		}
		#endif
	}

	void OnOpenBattleChest (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCGainBattleChest proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCGainBattleChest>(ms);


		if (proto.code != ErrCode.EC_Ok) {

			NotifyChestErrCode (proto.code);

		} else {
			LocalPlayer.Get ().playerData.curbattlechest = (int)proto.num;
			Debug.Log (string.Format ("OnOpenBattleChest:{0}", proto.num));
			EventSystem.Instance.FireEvent (EventId.OnChestNotify);
			UISystem.Get ().ShowWindow ("ChestWindow");
			EventSystem.Instance.FireEvent (EventId.OnChestBattle, proto);
		}
		#endif
	}

	void OnUpdateTimeChest (int msgid, PacketEvent msg)
	{
        #if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCUpdateTimerChest proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCUpdateTimerChest>(ms);
        
		LocalPlayer.Get ().playerData.timechest = proto.chest_gainpoint;
		Debug.Log (string.Format ("OnUpdateTimeChest:{0}", proto.chest_gainpoint));

		EventSystem.Instance.FireEvent (EventId.OnChestNotify);
		#endif
	}

	void OnUpdateBattleChest (int msgid, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCUpdateBattleChest proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCUpdateBattleChest>(ms);

		LocalPlayer.Get ().playerData.curbattlechest = proto.chest_winnum;
		Debug.Log (string.Format ("OnUpdateBattleChest:{0}", LocalPlayer.Get ().playerData.curbattlechest));

		EventSystem.Instance.FireEvent (EventId.OnChestNotify);
		#endif
	}

	public void RequestGMCommand (string cmd)
	{
		NetMessage.CSGMCmd proto = new CSGMCmd ();
		proto.cmd = (cmd);
		NetSystem.Instance.Send<NetMessage.CSGMCmd>((int)NetMessage.MsgId.ID_CSGMCmd, proto);
	}

	public void RequestSingleMatch (string matchId, GameType etype ,bool isFireEvent = true)
	{
		BattleSystem.Instance.Reset ();
		BattleSystem.Instance.SetPlayMode (false, true);
        BattleSystem.Instance.battleData.gameType = etype;

		// 地图
		BattleSystem.Instance.battleData.matchId = matchId;
		LocalPlayer.Get ().battleMap = BattleSystem.Instance.battleData.matchId;
		// 设置随机种子, 用作播放使用
		BattleSystem.Instance.battleData.rand.seed = System.DateTime.UtcNow.Millisecond;
        int saveSeed = BattleSystem.Instance.battleData.rand.seed;
        BattleSystem.Instance.battleData.teamFight = false;

        List<int> camptions = new List<int>();
        MapConfig map = MapConfigProvider.Instance.GetData(matchId);
        if (map != null)
        {
            foreach (MapBuildingConfig item in map.mbcList)
            {
                camptions.Add(item.camption);
            }
        }

        // 玩家数据
        //int playerAdd = 0;
        //int aiStrategy = 0;
        //MapConfig mapConfig = MapConfigProvider.Instance.GetData (matchId);
        //LevelConfig levelConfig = LevelConfigConfigProvider.Instance.GetData(matchId);
        //int playerTeam = levelConfig.playerTeam < 1 ? 1 : levelConfig.playerTeam;    //根据配置决定玩家阵营
        //if (string.IsNullOrEmpty(mapConfig.defaultai))
        //    BattleSystem.Instance.battleData.aiStrategy = -1;
        //else
        //    BattleSystem.Instance.battleData.aiStrategy = int.Parse(mapConfig.defaultai);
        //if (BattleSystem.Instance.battleData.aiStrategy < 0)
        //{
        //    LevelConfig levelCfg = LevelConfigConfigProvider.Get().GetData(matchId);
        //    if (levelCfg == null)
        //        BattleSystem.Instance.battleData.aiStrategy = 3;
        //    else
        //        BattleSystem.Instance.battleData.aiStrategy = AIStrategyConfigProvider.Instance.GetAIStrategyByName(levelCfg.aiType);
        //}
        if (BattleSystem.Instance.battleData.aiStrategy < 0)
            BattleSystem.Instance.battleData.aiStrategy = 1;

        //设置胜利类型
  //      BattleSystem.Instance.battleData.winType = levelConfig.winType;
  //      BattleSystem.Instance.battleData.winTypeParam1 = levelConfig.winTypeParam1;
  //      BattleSystem.Instance.battleData.winTypeParam2 = levelConfig.winTypeParam2;

  //      BattleSystem.Instance.battleData.runWithScript = false;

  //      List<int> userIdList = new List<int> ();
		//for (int i = 0; i < (int)TEAM.Team_Black2; ++i) {
  //          if(camptions.Contains(i+1))
  //          {
  //              Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam((TEAM)(i + 1));
  //              if (t.team == (TEAM)(playerTeam))   //根据配置决定玩家阵营
  //              {
  //                  t.playerData.Init(LocalPlayer.Get().playerData);
  //                  BattleSystem.Instance.battleData.currentTeam = t.team;
  //              }
  //              else
  //              {
  //                  t.playerData.userId = -10000 - i;
  //                  //t.playerData.name = AIManager.GetAIName(t.playerData.userId);
  //                 // t.playerData.icon = AIManager.GetAIIcon(t.playerData.userId);
  //                  // 加入ai数据
  //                  aiStrategy = mapConfig.teamAiType[i + 1];
  //                  if (aiStrategy < 0) aiStrategy = BattleSystem.Instance.battleData.aiStrategy;
  //                  //BattleSystem.Instance.sceneManager.aiManager.AddAI(t, aiStrategy, t.playerData.level, BattleSystem.Instance.battleData.aiLevel);
  //                  t.aiEnable = true;

  //                  //LoggerSystem.Instance.Info(string.Format("加入了AI：{0}, 类型：{1}", t.playerData.name, BattleSystem.Instance.sceneManager.aiManager.aiData[i+1].aiStrategy));
  //              }

  //              userIdList.Add(t.playerData.userId);
  //              playerAdd++;
  //              if (playerAdd >= mapConfig.player_count) break;
  //          }
  //      }

		#if !SERVER
		// 状态
		BattleSystem.Instance.battleData.gameState = GameState.Game;
		// 预加载资源
		AssetManager.Get ().LoadBattleResources ();
		// 预加载特效
		//EffectManager.Get ().PreloadEffect ();
		#endif

		BattleSystem.Instance.sceneManager.Reset ();

		// 创建地图
		//BattleSystem.Instance.sceneManager.CreateScene (userIdList);

		if (BattleSystem.Instance.battleData.useAI) {
			// 启动ai
			//BattleSystem.Instance.sceneManager.aiManager.Start (1);
		} else {
			BattleSystem.Instance.battleData.useAI = true;
		}

		// lockstep 模式
		BattleSystem.Instance.lockStep.replay = true;
		BattleSystem.Instance.lockStep.playSpeed = 1;
        BattleSystem.Instance.SetPause(true);
       
	}

	private void OnSingleMatch (int msgid, PacketEvent msg)
	{

        #if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCSingleMatch proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCSingleMatch>(ms);

        if (proto.code != ErrCode.EC_Ok) {
			Tips.Make (LanguageDataProvider.GetValue(209) + proto.code.ToString ());
			return;
		}

		//UISystem.Get().HideWindow("CustomSelectWindow");
		#endif
	}

	/// <summary>
	/// 假装开始战场
	/// </summary>
	/// <param name="mapId">Map identifier.</param>
	private void FakeStartBattle (string mapId)
	{
		BattleSystem.Instance.battleData.isFakeBattle = true;
		BattleSystem.Instance.battleData.matchId = mapId;

		// 设置同阵营
		for (int i = 0; i < 4; ++i) {
			Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam ((TEAM)(i + 1));
			t.groupID = 99;
		}

		// 预加载资源
		#if !SERVER
		AssetManager.Get ().FakeLoadBattleResources ();
		#endif
		BattleSystem.Instance.sceneManager.CreateScene (null);

		BattleSystem.Instance.lockStep.replay = true;
		BattleSystem.Instance.lockStep.playSpeed = 1;

		BattleSystem.Instance.sceneManager.FadePlanet (true, 0.1f);
		
		BattleSystem.Instance.StartLockStep ();

		List<Packet> packets = new List<Packet> ();
		for (int i = 0; i < 10 * 60; ++i) {
			BattleSystem.Instance.lockStep.AddFrame (i + 1, packets.ToArray ());
		}
	}

	/// <summary>
	/// 设置当前通关关卡
	/// </summary>
	/// <param name="map">Map.</param>
	public void RequestSetCurrentLevel (string matchId, string guidId, string guidlevel)
	{
        string seriziedStr = string.Format("{0},{1},{2}", matchId, guidId, guidlevel );

		NetMessage.CSSetCurLevel proto = new CSSetCurLevel ();
		proto.cur_level = (seriziedStr);
		NetSystem.Instance.Send<NetMessage.CSSetCurLevel> ((int)NetMessage.MsgId.ID_CSSetCurLevel, proto);
	}

	private void OnSetCurrentLevelResult (int msgid, PacketEvent msg)
	{
		//#if !SERVER
		//NetMessage.SCSetCurLevel proto = msg as SCSetCurLevel;
		//if (proto.Code == ErrCode.EC_Ok)
  //      {
		//	LocalAccountStorage.Get ().singleCurrentLevel = string.Empty;
		//	LocalStorageSystem.Instance.NeedSaveToDisk ();
		//}
		//#endif
	}

	/// <summary>
	/// 改名字
	/// </summary>
	/// <param name="name">Name.</param>
	public void ChangeName (string name)
	{
		NetMessage.CSRename proto = new CSRename ();
		proto.name = (name);
		NetSystem.Instance.Send<NetMessage.CSRename> ((int)NetMessage.MsgId.ID_CSRename, proto);
	}

	private void OnChangeName (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCRename proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCRename>(ms);
		EventSystem.Instance.FireEvent (EventId.OnRename, proto.code);
		#endif
	}

	private void OnRaceNotify (int msgId, PacketEvent msg)
	{
#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCRaceNotify proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCRaceNotify>(ms);
		if (proto.raceid.Count < 1)
			return;

		int nRaceID = proto.raceid[0];
		LocalPlayer.Get ().playerData.mNewRaceID = nRaceID;
#endif
	}

	/// <summary>
	/// 请求客户端保存数据
	/// tips：其中包含所有客户端相关的杂碎数据
	/// </summary>
	public void LoadClientStorage ()
	{
		NetMessage.CSClientStorageLoad proto = new CSClientStorageLoad ();
		NetSystem.Instance.Send<NetMessage.CSClientStorageLoad> ((int)NetMessage.MsgId.ID_CSClientStorageLoad, proto);
	}

	private void OnClientStorageLoad (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCClientStorageLoad proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCClientStorageLoad>(ms);

        for (int i = 0; i < proto.values.Count; ++i) {
			LocalPlayer.Get ().playerData.clientStorages [i] = proto.values [i];
		}

		if (proto.values.Count >= 3) {
			EventSystem.Instance.FireEvent (EventId.OnStorageLoaded, proto.values[(int)(ClientStorageConst.ClientStorageRedPoints)]);
		}
		#endif
	}

	/// <summary>
	/// 请求设置客户端保存数据
	/// </summary>
	public void SetClientStorage (int storageIndex, string vv)
	{
		NetMessage.CSClientStorageSet proto = new CSClientStorageSet ();
		proto.index.Add (storageIndex);
		proto.value.Add (vv);
		NetSystem.Instance.Send<NetMessage.CSClientStorageSet>((int)NetMessage.MsgId.ID_CSClientStorageSet, proto);
	}

	private void OnClientStorageSet (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCClientStorageSet proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCClientStorageSet>(ms);
		if (proto.code == ErrCode.EC_Ok) {
			for (int i = 0; i < proto.index.Count; ++i) {
				int index = proto.index [i];
				string v = proto.value [i];
				LocalPlayer.Get ().playerData.clientStorages [index] = v;
			}
		}
		#endif
	}

	/// <summary>
	/// 重连恢复战斗
	/// </summary>
	public void ReconnectResume (int frame = -2)
	{
		NetMessage.CSResume proto = new CSResume();
		if (frame > -2) {
			proto.startFrameNo = (frame);
		}
		NetSystem.Instance.Send<NetMessage.CSResume> ((int)NetMessage.MsgId.ID_CSResume, proto);
	}

    /// <summary>
	/// 重连恢复战斗
	/// </summary>
	public void RequestCancelBattle()
    {
        FramePacket packet  = new FramePacket();
        packet.type         = 2;
        packet.giveup       = new GiveUpPacket();
        packet.giveup.team  = TEAM.Team_1;
        byte[] bytestr      = Json.EnCodeBytes(packet);
        NetMessage.PbFrame pb = new PbFrame();
        NetMessage.CSFrame build = new CSFrame();

        pb.content  = (bytestr);
        build.frame = (pb);

        NetSystem.Instance.Send<NetMessage.CSFrame>((int)NetMessage.MsgId.ID_CSFrame, build);
    }

    /// <summary>
    /// 重连恢复战斗回复
    /// </summary>
    private void OnReconnectResumeResult (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCResume proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCResume>(ms);
		
		if (proto.code != ErrCode.EC_Ok) {
			BattleSystem.Instance.Reset ();
			UISystem.Get ().HideAllWindow ();
            UISystem.Get().ShowWindow("StartWindow");
            return;
		}

        // 判断各种状态
        BattleSystem.Instance.battleData.battleSubType = proto.sub_type;
        if (proto.match != null ) {
			// 匹配状态
			if (proto.match.typ == MatchType.MT_Ladder || proto.match.typ == MatchType.MT_League)
			{
				UISystem.Instance.HideAllWindow ();
				FakeStartBattle ("PVP");
				UISystem.Get ().ShowWindow ("PVPWaitWindow");
				OnMatchInit (proto.match );
			}
			else if (proto.match.typ == MatchType.MT_Room)
			{
				UISystem.Instance.HideAllWindow ();
				UISystem.Instance.ShowWindow ("RoomWaitWindow");
				OnMatchInit (proto.match);
			}
		} else if (proto.start != null ) {
			// 选种族状态
			UISystem.Instance.HideAllWindow ();
			OnStartSelectRace ( proto.start );
			OnSelectRaceNotify (proto.notify );
		} else if (proto.report != null ) {
			// 战斗中状态
			PbSCFrames frames = proto.report;

			AssetManager.Get ().LoadBattleResources ();

			// 此时需要判断当前是否在战斗中
			GameState state = BattleSystem.Instance.battleData.gameState;
			if (state == GameState.Game || state == GameState.GameWatch || state == GameState.Watcher) {
				// 战斗中只添加frame包
				for (int i = 0; i < frames.frames.Count; ++i) {
					SCFrame scf = frames.frames [i];
					OnNetFrame (scf);
				}
				// 直接执行到最后
				BattleSystem.Instance.lockStep.RunToFrame (BattleSystem.Instance.GetCurrentFrame () / 5 + frames.frames.Count);
				BattleSystem.Instance.sceneManager.SilentMode (true);
				BattleSystem.Instance.battleData.resumingFrame = (BattleSystem.Instance.GetCurrentFrame () / 5 + frames.frames.Count) * 5;
				UISystem.Instance.ShowWindow ("ResumingWindow");
			} else {
				BattleSystem.Instance.SetPlayMode (true, false);

				BattleSystem.Instance.battleData.resumingFrame = (frames.frames.Count + frames.frames.Count / 200) * 5; // modify for jira-491

				// 从ready开始解包
				OnReady (frames.ready);
				// start
				OnNetStartBattle (frames.start);
				// frame
				for (int i = 0; i < frames.frames.Count; ++i) {
					SCFrame scf = frames.frames [i];
					OnNetFrame (scf);
				}
				BattleSystem.Instance.lockStep.runFrameCount = 20;
				// 直接执行到最后
				BattleSystem.Instance.lockStep.RunToFrame (frames.frames.Count);
				BattleSystem.Instance.sceneManager.SilentMode (true);
				UISystem.Instance.ShowWindow ("ResumingWindow");
			}
		}

		if (UISystem.Get ().IsWindowVisible ("CommonDialogWindow")) {
			UISystem.Get ().HideWindow ("CommonDialogWindow");
		}

		#endif
	}

	/// <summary>
	/// 玩家被踢跳线
	/// </summary>
	private void OnKickUserNtf (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCKickUserNtf proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCKickUserNtf>(ms);
		
		if (proto.code == ErrCode.EC_AccountTakenOver) {
			// 设定当前不走断线重连
			LocalPlayer.Get ().isAccountTokenOver = true;
			// 被踢后主动断线
			NetSystem.Instance.Close ();

			string device = proto.device_model;

			UISystem.Instance.ShowWindow ("CommonDialogWindow");
			// 提示是否重连
			EventSystem.Instance.FireEvent (EventId.OnCommonDialog, 1, LanguageDataProvider.Format (505, device)
				, new EventDelegate (() => {

				// 标记可以重连，然后重连
				LocalPlayer.Get ().isAccountTokenOver = false;
				NetSystem.Instance.DisConnectedCallback ();

			}));
			
		}

		#endif
	}

	/// <summary>
	/// 服务器通知
	/// </summary>
	private void OnServerNotify (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCNotify proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCNotify>(ms);

        if (proto.typ == NotifyType.NT_Popup) {
			// 弹窗
			UISystem.Instance.ShowWindow ("CommonDialogWindow");
			EventSystem.Instance.FireEvent (EventId.OnCommonDialog, 1, proto.text);
		} else if (proto.typ == NotifyType.NT_Scroll) {
			// 滚动条
			Tips.Make (Tips.TipsType.FlowLeft, proto.text, 2.0f);
			Tips.Make (Tips.TipsType.FlowLeft, proto.text, 2.0f);
		} else if (proto.typ == NotifyType.NT_Error) {
			// 通用错误
			Tips.Make (Tips.TipsType.FlowUp, proto.text, 1.0f);
		}

		#endif
	}

	/// <summary>
	/// 开始匹配
	/// 参数1，游戏类型；参数2，游戏类型的额外参数（房间id、联赛id等）
	/// </summary>
	public void StartMatchReq (NetMessage.MatchType type, string misc_id, NetMessage.CooperationType cType, int nPlayerNum )
	{
		if (type == MatchType.MT_Ladder) {
			BattleSystem.Instance.SetPlayMode (true, false);
			BattleSystem.Instance.battleData.gameType = GameType.PVP;
		}
        else if (type == MatchType.MT_League)
        {
		
		}
        else if (type == MatchType.MT_Room) {
			
		}

		NetMessage.CSStartMatchReq proto = new CSStartMatchReq ();
		proto.typ = (type);
        proto.cType = (cType);
        proto.nPlayerNum = (nPlayerNum);
		if (!string.IsNullOrEmpty (misc_id))
        {
			proto.misc_id = (misc_id);
		}
		proto.has_race = (false);
		NetSystem.Instance.Send<NetMessage.CSStartMatchReq>((int)NetMessage.MsgId.ID_CSStartMatchReq, proto);
	}
		
	/// <summary>
	/// 开始战斗的回复
	/// </summary>
	private void OnStartMatchRequest (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCStartMatchReq proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCStartMatchReq>(ms);
        BattleSystem.Instance.battleData.battleSubType = proto.cType;
        if (proto.typ == MatchType.MT_Ladder) {
			#if !SERVER
			if (proto.code != ErrCode.EC_Ok)
            {
				Tips.Make (Tips.TipsType.FlowUp, string.Format (LanguageDataProvider.GetValue(111), proto.code), 1);
				return;
			}

            // 开始模拟pvp进入的游戏局
            if (BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_2V2)
            {
                UISystem.Get().HideWindow("Room2V2Window");
            }
            else
            {
                UISystem.Get().HideWindow("PvPRoomWindow");
            }
			//FakeStartBattle ("PVP");
			UISystem.Get ().ShowWindow ("PVPWaitWindow");
			#endif
		}
        else if (proto.typ == MatchType.MT_League)
        {
			#if !SERVER
			if (proto.code == ErrCode.EC_Ok) {
                UISystem.Get ().HideWindow ("LeagueWindow");
				FakeStartBattle ("PVP");
				UISystem.Get ().ShowWindow ("PVPWaitWindow");
			}
			EventSystem.Instance.FireEvent (EventId.OnLeagueMatchResult, proto.code );

			#endif
		}
        else if (proto.typ == MatchType.MT_Room)
        {
			EventSystem.Instance.FireEvent (EventId.OnStartMatchResult, proto.code);
		}
	}

	/// <summary>
	/// 房间初始化消息
	/// </summary>
	private void OnMatchInit (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCMatchInit proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCMatchInit>(ms);
        OnMatchInit(proto);
    }

    private void OnMatchInit( NetMessage.SCMatchInit proto )
    {
        string matchId = proto.matchid;
        NetMessage.MatchType matchType = proto.typ;

        if (matchType == MatchType.MT_Ladder)
        {
            EventSystem.Instance.FireEvent(EventId.OnMatchInit, matchId, proto.miscid, proto.user, proto.useridx, proto.countdown, proto.nPlayerNum);
        }
        else if (matchType == MatchType.MT_League)
        {
            EventSystem.Instance.FireEvent(EventId.OnMatchInit, matchId, proto.miscid, proto.user, proto.useridx, proto.countdown, proto.nPlayerNum);
        }
        else if (matchType == MatchType.MT_Room)
        {
            EventSystem.Instance.FireEvent(EventId.OnMatchInit, matchId, proto.miscid, proto.user, proto.useridx, proto.countdown, proto.nPlayerNum);
        }
    }

    /// <summary>
    /// 房间更新消息
    /// </summary>
    private void OnMatchUpdate (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCMatchUpdate proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCMatchUpdate>(ms);

		NetMessage.MatchType matchType = proto.typ;
		if (proto.typ == MatchType.MT_Ladder) {
			EventSystem.Instance.FireEvent (EventId.OnMatchUpdate, proto.user_added, proto.index_added, proto.index_deled);
		} else if (proto.typ == MatchType.MT_League) {
			EventSystem.Instance.FireEvent (EventId.OnMatchUpdate, proto.user_added, proto.index_added, proto.index_deled);
		} else if (proto.typ == MatchType.MT_Room) {
			if (proto.masterid > 0 ) {
				// 房主更新
				EventSystem.Instance.FireEvent (EventId.OnMatchUpdate, proto.user_added, proto.index_added, proto.index_deled, proto.kick, proto.change_from, proto.change_to, proto.masterid);
			} else {
				EventSystem.Instance.FireEvent (EventId.OnMatchUpdate, proto.user_added, proto.index_added, proto.index_deled, proto.kick, proto.change_from, proto.change_to);
			}
		}
	}

	/// <summary>
	/// 房间开始，房主发动
	/// </summary>
	public void MatchComplete ()
	{
		NetMessage.CSMatchComplete proto = new CSMatchComplete ();

		NetSystem.Instance.Send<NetMessage.CSMatchComplete>((int)NetMessage.MsgId.ID_CSMatchComplete, proto);
	}

	private void OnMatchComplete (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCMatchComplete proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCMatchComplete>(ms);
       

		if (proto.code == ErrCode.EC_Ok) {

			UISystem.Get ().HideWindow ("RoomWaitWindow");
			FakeStartBattle ("PVP");
			UISystem.Get ().ShowWindow ("PVPWaitWindow");
		} else {
			#if !SERVER
			Tips.Make (Tips.TipsType.FlowUp, LanguageDataProvider.Format (901, proto.code), 1.0f);
			#endif
		}
	}

	/// <summary>
	/// 房间中玩家位置移动
	/// </summary>
	public void RequestMatchMovePos (int userId, int toIndex)
	{
		NetMessage.CSMatchPos proto = new CSMatchPos ();
		proto.userid = (userId);
		proto.index = (toIndex);

		NetSystem.Instance.Send<NetMessage.CSMatchPos>((int)NetMessage.MsgId.ID_CSMatchPos, proto);
	}

	private void OnMatchPos (int msgId, PacketEvent msg)
	{
		#if !SERVER
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCMatchPos proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCMatchPos>(ms);
       

		if (proto.code == ErrCode.EC_Ok) {
		
		} else {
			Tips.Make (LanguageDataProvider.GetValue (911));
		}
		#endif
	}

	/// <summary>
	/// 退出、踢出匹配，房间
	/// </summary>
	public void QuitMatch (int userId = -1)
	{
		NetMessage.CSQuitMatch proto = new CSQuitMatch ();
		if (userId > 0) {
			proto.userid = (userId);
		}

		NetSystem.Instance.Send<NetMessage.CSQuitMatch> ((int)NetMessage.MsgId.ID_CSQuitMatch, proto);
	}

	/// <summary>
	/// 退出匹配房间
	/// </summary>
	private void OnQuitMatch (int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCQuitMatch proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCQuitMatch>(ms);

		EventSystem.Instance.FireEvent (EventId.OnMatchQuit, proto.code);
	}

	/// <summary>
	/// 请求获取章节信息
	/// </summary>
	/// <param name="levelID">Level I.</param>
	public void RequestChapters()
	{
		NetMessage.CSLoadChapters proto = new CSLoadChapters ();
		NetSystem.Instance.Send<NetMessage.CSLoadChapters> ((int)NetMessage.MsgId.ID_CSLoadChapters, proto);
	}

	/// <summary>
	/// 请求获取章节信息
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	private void OnLoadChapters(int msgId, PacketEvent msg)
	{
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCLoadChapters proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCLoadChapters>(ms);
		EventSystem.Instance.FireEvent (EventId.OnLoadChaptersResult, proto);
	}

	/// <summary>
	/// 请求获取关卡信息
	/// </summary>
	/// <param name="levelID">Level I.</param>
	//public void RequestOneChapter(string chapterId)
	//{
	//	NetMessage.CSLoadChapter.Builder proto = new CSLoadChapter.Builder ();
	//	proto.SetChapter (chapterId);
	//	NetSystem.Instance.Send ((int)NetMessage.MsgId.ID_CSLoadChapter, proto);
	//}

	/// <summary>
	/// 请求获取关卡信息
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	//private void OnLoadOneChapter(int msgId, IMessage msg)
	//{
	//	NetMessage.SCLoadChapter proto = msg as NetMessage.SCLoadChapter;
	//	EventSystem.Instance.FireEvent (EventId.OnLoadOneChapterResult, proto);
	//}

	/// <summary>
	/// 开始关卡
	/// </summary>
	/// <param name="levelId">Level identifier.</param>
	public void RequestStartLevel (string levelId)
	{
		NetMessage.CSStartLevel proto = new CSStartLevel ();
		proto.level_name = (levelId);
		NetSystem.Instance.Send<NetMessage.CSStartLevel>((int)NetMessage.MsgId.ID_CSStartLevel, proto);
	}
	private void OnStartLevel (int msgId, PacketEvent msg)
	{
		//NetMessage.SCStartLevel proto = msg as NetMessage.SCStartLevel;
		//EventSystem.Instance.FireEvent (EventId.OnStartLevelResult, proto);
    }

	
	/// <summary>
	/// Raises the int attr event.
	/// </summary>
	/// <param name="msgId">Message identifier.</param>
	/// <param name="msg">Message.</param>
	private void OnIntAttr(int msgId, PacketEvent msg)
	{
		//NetMessage.SCIntAttr proto = msg as NetMessage.SCIntAttr;
		//LocalPlayer.Get ().playerData.UpdateFromNetMsg (proto);
		//EventSystem.Instance.FireEvent (EventId.UpdatePower, proto);
	}

    
    /// <summary>
    /// 金钱变化了
    /// </summary>
    private void OnMoneyChange( int msgId, PacketEvent msg )
    {
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCChangeMoney proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCChangeMoney>(ms);
   
        LocalPlayer.Get().SetMoney( proto.nCurMoney);
    }


    /// <summary>
	/// 通关后，改变星星数量
	/// </summary>
	public void GenPresignedUrl(string objectName, string method, string contentType,string file, int eventId )
    {
        NetMessage.CSGenerateUrl proto = new CSGenerateUrl();
        proto.objectname    = objectName;
        proto.method        = method;
        proto.contenttype   = contentType;
        proto.file          = file;
        proto.eventId       = eventId;

        NetSystem.Instance.Send<NetMessage.CSGenerateUrl>((int)NetMessage.MsgId.ID_CSGenerateUrl, proto);
    }

    /// <summary>
    /// 改变星星数量回复
    /// </summary>
    /// <param name="msgId">Message identifier.</param>
    /// <param name="msg">Message.</param>
    private void OnGenPresignedUrl(int msgId, PacketEvent msg)
    {
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCGenerateUrl proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCGenerateUrl>(ms);

        EventSystem.Instance.FireEvent((EventId)proto.eventId, proto);
    }



    /// pve 排行
	public void RequestSetLevelSorce(string levelId, string account, int score )
    {
        NetMessage.CSSetLevelScore proto = new CSSetLevelScore();
        proto.level_name = (levelId);
        proto.accountid  = (account);
        proto.score = (score);
        NetSystem.Instance.Send<NetMessage.CSSetLevelScore>((int)NetMessage.MsgId.ID_CSSetLevelScore, proto);
    }

    /// pve 排行响应
    public void OnRequestSetLevelSorce(int msgId, PacketEvent msg) {
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCSetLevelScore proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCSetLevelScore>(ms);

        //if (proto.upLoad) {
        //    UserSyncSysteam.Get().GenPveUploadUrl(LocalAccountStorage.Get().account, proto.level_name);
        //}
    }

    public void RequestBuyChapter(string chapter )
    {
        NetMessage.CSBuyChapter proto = new CSBuyChapter();
        proto.chapter = chapter;
        NetSystem.Instance.Send<NetMessage.CSBuyChapter>((int)NetMessage.MsgId.ID_CSBuyChapter, proto);
    }


    public void OnRequestBuyChapter( int msgId, PacketEvent msg )
    {
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCBuyChapter proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCBuyChapter>(ms);

        if( proto.code == ErrCode.EC_Ok )
        {
            EventSystem.Instance.FireEvent(EventId.OnHaveNewChapterUnlocked, proto.chapter);
        }
    }

    public void RequestPveRankReport(string levelId)
    {
        NetMessage.CSPveRankReportLoad proto = new CSPveRankReportLoad();
        proto.levelId = levelId;
        NetSystem.Instance.Send<NetMessage.CSPveRankReportLoad>((int)NetMessage.MsgId.ID_CSPveRankReportLoad, proto);
    }


    private void OnRequestPveRankReport(int msgId, PacketEvent msg)
    {
        MemoryStream ms = msg.Data as MemoryStream;
        NetMessage.SCPveRankReportLoad proto = ProtoBuf.Serializer.Deserialize<NetMessage.SCPveRankReportLoad>(ms);


        string LevelId = proto.levelId;
        List<FriendSimplePlayer> reportList = new List<FriendSimplePlayer>();
        for (int i = 0; i < proto.report.Count; ++i)
        {
            FriendSimplePlayer brd = new FriendSimplePlayer();
            brd.Init(proto.report[i]);
            reportList.Add(brd);
        }

        EventSystem.Instance.FireEvent(EventId.OnPveRankReportLoad,true , reportList);
    }
}

