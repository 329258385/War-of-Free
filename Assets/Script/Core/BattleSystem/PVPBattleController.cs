using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plugin;
using Solarmax;
using NetMessage;


public class BattleEndData
{
	public TEAM team;
	public string userName;
	public int userId;
	public EndType endType; //  
	public int endFrame;
	public int destroy;
	public int survive;
}

/// <summary>
/// 多人战斗的控制器
/// </summary>
public class PVPBattleController : IBattleController
{
    public SceneManager sceneManager;
    private BattleData battleData;
	private BattleSystem battleSystem;

	private int tickEndCount = 0;
	private List<BattleEndData> battleEndDatas = new List<BattleEndData>();
	private int diedTeamCount = 0;

	private int battleTimeMax = 0;

	private int survivorTimeMax = 0;    //失去所有星球后的生存时间

    private bool speedUp = false;
    private Vector3 center = Vector3.zero;
    private string centerPlanet = "";
    //private Node bombTarget = null;
    private int lastElapsed = 0;
    private int narrowtimes = -2;

    private bool hasSentAccelerateEvent = false;
    private bool hasSentBoomEvent = false;

    private bool hasTriggerClearBarrier = false;

    public PVPBattleController(BattleData bd, BattleSystem bs)
	{
		battleData = bd;
		battleSystem = bs;
	}

	public bool Init()
	{
		tickEndCount = 0;
		diedTeamCount = 0;

		battleTimeMax = int.Parse(GameVariableConfigProvider.Instance.GetData(4)) * 60;
		survivorTimeMax = int.Parse(GameVariableConfigProvider.Instance.GetData(6));

        speedUp = false;
        center = Vector3.zero;
        centerPlanet = "";
        //bombTarget = null;
        lastElapsed = 0;
        narrowtimes = -2;
        hasTriggerClearBarrier = false;

        return true;
	}


    public void Tick(int frame, float interval)
	{
        if (battleSystem.battleData.battleType == BattlePlayType.Replay)
        {
            if(battleSystem.battleData.useCommonEndCondition)
                UpdateEndSingle(frame, interval);
            else
                UpdateEndAllOccupiedSingle(frame, interval);
        }
        else
        {
            UpdateEnd(frame, interval);
        }
        UpdateEndOtherWinLoseType(frame, interval);
        UpdateResuming(frame, interval);
        if ((battleSystem.battleData.gameType == GameType.PVP) && (battleSystem.battleData.battleType != BattlePlayType.Replay))
        {
            UpdateBattleSpeed();
            UpdateBombPlanet();
        }

    }

	public void Destroy()
	{

	}

	public void Reset()
	{
		tickEndCount = 0;
		battleEndDatas.Clear();
	}

	public void OnRecievedFramePacket(NetMessage.SCFrame frame)
	{
        List<Packet> list = new List<Packet>();
		for (int i = 0; i < frame.users.Count; i++) {

			int uid = frame.users[i];
            NetMessage.PbFrames pbs = frame.frames[i];
			if (pbs == null || pbs.frames.Count == 0)
				continue;

			for (int k = 0; k < pbs.frames.Count; k++)
            {
				NetMessage.PbFrame pb = pbs.frames[k];
				if (pb == null)
					continue;

				Packet move     = new Packet();
                if(BattleSystem.Instance.battleData.battleType == BattlePlayType.Replay )
                {
                    move.team = (TEAM)(uid);
                }
                else
                {
                    move.team = (TEAM)(uid + 1);
                }
                
				move.packet     = Json.DeCode<FramePacket>(pb.content);
				list.Add(move);
			}
		}
		battleSystem.lockStep.AddFrame(frame.frameNum, list.ToArray());
        list.Clear();
        list = null;
	}

    public void OnRecievedScriptFrame(NetMessage.PbSCFrames frames)
    {
        for (int i = 0; i < frames.frames.Count; )
        {
            List<Packet> list = new List<Packet>();
            int FrameNum = frames.frames[i].frameNum;
            for (int j = i; j < frames.frames.Count; ++j)
            {
                NetMessage.SCFrame frame = frames.frames[j];
                if (frame.frameNum == FrameNum)
                {
                    i++;
                    NetMessage.PbFrames pbs = frame.frames[0];
                    if (pbs == null || pbs.frames.Count == 0)
                        continue;
                    for (int k = 0; k < pbs.frames.Count; k++)
                    {
                        NetMessage.PbFrame pb = pbs.frames[k];
                        if (pb == null)
                            continue;
                        Packet move = new Packet();
                        move.team = TEAM.Neutral;
                        move.packet = Json.DeCode<FramePacket>(pb.content);
                        list.Add(move);
                    }
                }
                else
                    break;
            }
            battleSystem.lockStep.AddFrame(FrameNum, list.ToArray());
            list.Clear();
            list = null;
        }
    }


    public void OnRunFramePacket(FrameNode frameNode)
	{
		battleSystem.sceneManager.RunFramePacket(frameNode);
	}

	public void OnPlayerMove(/*Node from, Node to, float percent*/)
	{
		//NetSystem.Instance.helper.SendMoveMessaeg(from, to, percent);
	}

	public void PlayerGiveUp()
	{
		SendGiveUpFrame();
	}

    public void OnPlayerDiedTimeout(TEAM t, int frame)
    {
        //List<Node> nodes = battleSystem.sceneManager.nodeManager.GetUsefulNodeList();
        //foreach (var node in nodes)
        //{
        //    node.BombShip(t, TEAM.Neutral, 1.0f);
        //}
        //List<Ship> ships = battleSystem.sceneManager.shipManager.GetFlyShip(t);
        //for (int i = 0; i < ships.Count; i++)
        //{
        //    ships[i].Bomb();
        //}
        OnPlayerDiedOrGiveUp(EndType.ET_Dead, t, frame);
    }


    public void OnPlayerGiveUp(TEAM giveUpTeam)
	{
		OnPlayerDiedOrGiveUp(EndType.ET_Giveup, giveUpTeam, battleSystem.GetCurrentFrame());
	}

	public void OnPlayerDirectQuit(TEAM team)
	{
		QuitBattle();
	}

    private void OnPlayerDiedOrGiveUpSingle(EndType type, TEAM team, int frame)
    {
        // 检查他在不在结果列表，在的话，不再加入了
        bool inList = false;
        for (int i = 0; i < battleEndDatas.Count; ++i)
        {
            if (battleEndDatas[i].team == team)
            {
                inList = true;
                break;
            }
        }
        if (!inList)
        {
            Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam(team);
            OnPlayerEnterEnd(t, type, frame);
        }

        //队伍判断,是否只存在一个队伍
        int teamNum = 1;
        for (int i = 0; i < (int)TEAM.TeamMax; ++i)
        {
            Team wint = battleSystem.sceneManager.teamManager.GetTeam((TEAM)i);
            if (wint != null && wint.Valid() && !wint.isEnd)
            {
                for (int j = i + 1; j < (int)TEAM.TeamMax; ++j)
                {
                    Team frinedTeam = battleSystem.sceneManager.teamManager.GetTeam((TEAM)j);
                    if (frinedTeam != null && frinedTeam.Valid() && !frinedTeam.isEnd && !wint.IsFriend(frinedTeam.groupID))
                    {
                        teamNum++;
                        break;
                    }
                }
                if (teamNum != 1)
                    break;
#if !SERVER
                Debug.Log("加入最后一个队伍:" + wint.team + "    name:" + wint.playerData.name);
#endif
                OnPlayerEnterEnd(wint, EndType.ET_Win, frame);
                battleData.winTEAM = wint.team;
                break;
            }
        }

        //// 判断是不是总人数-1，如果是，则将第一名加入
        //if (battleEndDatas.Count == battleData.currentPlayers - 1)
        //{
        //    for (int i = 0; i < (int)TEAM.TeamMax; ++i)
        //    {
        //        Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
        //        if (t != null && t.Valid() && !t.isEnd)
        //        {
        //            OnPlayerEnterEnd(t, EndType.ET_Win, frame);
        //            battleData.winTEAM = t.team;
        //            break;
        //        }
        //    }
        //}

        // 出现结果，结束战斗
        if (teamNum == 1)
        {
            QuitBattle(true);
        }
    }

    /// <summary>
    /// 检查是否到达结果
    /// </summary>
    private void UpdateEnd(int frame, float dt)
	{
		if (++tickEndCount == 5) {
			// 先查询谁死了
			for (int i = 0; i < (int)TEAM.TeamMax; ++i) {
				Team t = battleSystem.sceneManager.teamManager.GetTeam((TEAM)i);
				if (t != null && t.Valid() && !t.isEnd) {
					if (t.CheckDead()) {
						// 死亡
						OnPlayerDiedOrGiveUp(EndType.ET_Dead, t.team, frame);
					}
					if (t.GetLoseAllMatrixTime(dt * 5) > survivorTimeMax) {
                        // 生存失败，直接走投降逻辑
                        OnPlayerDiedTimeout(t.team, frame);
					}
				}
			}

			if (diedTeamCount != battleData.currentPlayers - 1)
				tickEndCount = 0;
		}

		// 限制超时时间
		if (battleSystem.sceneManager.GetBattleTime() > battleTimeMax) {
			// 当前所有没有死亡的都给个Timout
			for (int i = 0; i < (int)TEAM.TeamMax; ++i) {
				Team t = battleSystem.sceneManager.teamManager.GetTeam((TEAM)i);
				if (t != null && t.Valid() && !t.isEnd) {
					if (!t.CheckDead()) {
						OnPlayerEnterEnd(t, EndType.ET_Timeout, frame);
					}
				}
			}

			// 结束
			QuitBattle(true);
		}
	}

    /// <summary>
    /// 检查是否到达结果
    /// </summary>
    private void UpdateEndSingle(int frame, float dt)
    {
        if (++tickEndCount == 5)
        {
            // 先查询谁死了
            for (int i = 0; i < (int)TEAM.TeamMax; ++i)
            {
                Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
                if (t != null && t.Valid() && !t.isEnd)
                {
                    if (t.CheckDead())
                    {
                        // 死亡
                        OnPlayerDiedOrGiveUpSingle(EndType.ET_Dead, t.team, frame);
                    }
                }
            }

            if (battleEndDatas.Count != battleData.currentPlayers)
                tickEndCount = 0;
        }
    }

    private void UpdateEndOtherWinLoseType(int frame, float interval)
    {
        if (++tickEndCount == 5)
        {
            bool quit = false;
            if (BattleSystem.Instance.battleData.winType == "occupy")
            {
                //int eTeam = BattleSystem.Instance.sceneManager.nodeManager.OccupiedSomeone((int)battleData.currentTeam, BattleSystem.Instance.battleData.winTypeParam1, BattleSystem.Instance.battleData.winTypeParam2);
                //if (eTeam > 0)
                //{
                //    battleData.winTEAM = (TEAM)eTeam;
                //    quit = true;
                //}
            }

            else if (BattleSystem.Instance.battleData.winType == "killnum")
            {
                for (int i = 0; i < (int)TEAM.TeamMax; ++i)
                {
                    Team wint = BattleSystem.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
                    if (wint != null && wint.hitships >= int.Parse(BattleSystem.Instance.battleData.winTypeParam1))
                    {
                        battleData.winTEAM = wint.team;
                        quit = true;
                    }
                }
            }

            if (quit)
            {
                QuitBattle(true);
                return;
            }

            tickEndCount = 0;
        }
    }


    private void UpdateEndOtherWinLoseTypeSingle(int frame, float interval)
    {
        if (++tickEndCount == 5)
        {
            bool quit = false;
            if (BattleSystem.Instance.battleData.winType == "occupy")
            {
                //int eTeam = BattleSystem.Instance.sceneManager.nodeManager.OccupiedSomeone((int)battleData.currentTeam, BattleSystem.Instance.battleData.winTypeParam1, BattleSystem.Instance.battleData.winTypeParam2);
                //if (eTeam == (int)battleData.currentTeam)
                //    quit = true;
            }
            else if (BattleSystem.Instance.battleData.winType == "alive")
            {
                if (BattleSystem.Instance.sceneManager.GetBattleTime() >= float.Parse(BattleSystem.Instance.battleData.winTypeParam1))
                    quit = true;
            }
            else if (BattleSystem.Instance.battleData.winType == "score")
            {
            }
            else if (BattleSystem.Instance.battleData.winType == "killnum")
            {
                if (BattleSystem.Instance.sceneManager.teamManager.GetTeam(battleData.currentTeam).hitships >= int.Parse(BattleSystem.Instance.battleData.winTypeParam1))
                    quit = true;
            }

            if (quit)
            {
                battleData.winTEAM = battleData.currentTeam;
                QuitBattle(true);
                return;
            }

            tickEndCount = 0;
        }
    }
    /// <summary>
    /// 所有全部销毁
    /// </summary>
    private void UpdateEndAllOccupiedSingle(int frame, float interval)
    {
        if (++tickEndCount == 5)
        {

            //if (BattleSystem.Instance.sceneManager.nodeManager.AllOccupied((int)battleData.currentTeam))
            //{
            //    battleData.winTEAM = battleData.currentTeam;
            //    QuitBattle(true);
            //    return;
            //}

            tickEndCount = 0;
        }
    }

   
    private void OnPlayerEnterEnd(Team t, EndType type, int frame)
	{
		BattleEndData data = new BattleEndData();
		data.team = t.team;
		data.userName = t.playerData.name;
		data.userId = t.playerData.userId;
		data.destroy = t.destory;
		data.survive = t.current;
		data.endType = type;
		data.endFrame = frame;
		t.isEnd = type == EndType.ET_Dead;

		battleEndDatas.Add(data);
	}

	public void OnPlayerDiedOrGiveUp(EndType type, TEAM team, int frame)
	{
		//Debug.LogFormat ("PlayerDiedOrGiveUp      type:{0}, team:{1}, frame{2}", type, team, frame);

		// 当前队伍
		Team t = battleSystem.sceneManager.teamManager.GetTeam(team);

		// 如果结果中已经有当前种类和当前队伍的信息时，不需要再处理
		bool haveEnd = false;
		for (int i = 0; i < battleEndDatas.Count; ++i) {
			BattleEndData d = battleEndDatas[i];
			if (d.team == team && d.endType == type) {
				haveEnd = true;
				break;
			}
		}
		if (haveEnd) {
#if !SERVER
			LoggerSystem.Instance.Error(string.Format("PlayerDiedOrGiveUp  Name:{0} Team:{1} type:{2}  frame:{3}", t.playerData.name, team, type, frame));
#endif
			return;
		}

		// 不管在已经有没有结果，都加入。区分giveup和died
		OnPlayerEnterEnd(t, type, frame);

		// 如果是放弃，则使用AI
		if (type == EndType.ET_Giveup) {
			t.aiEnable = true;
#if !SERVER
			LoggerSystem.Instance.Info(string.Format("玩家:{0} 投降，使用AI替代", t.playerData.name));
#endif
		}

		// 死亡队伍计数
		if (type == EndType.ET_Dead)
			++diedTeamCount;
        //队伍判断,是否只存在一个队伍
        int teamNum = 1;
        for (int i = 0; i < (int)TEAM.TeamMax; ++i)
        {
            Team wint = battleSystem.sceneManager.teamManager.GetTeam((TEAM)i);
            if (wint != null && wint.Valid() && !wint.isEnd)
            {
                for (int j = i+1; j < (int)TEAM.TeamMax; ++j)
                {
                    Team frinedTeam = battleSystem.sceneManager.teamManager.GetTeam((TEAM)j);
                    if (frinedTeam != null && frinedTeam.Valid() && !frinedTeam.isEnd && !wint.IsFriend(frinedTeam.groupID))
                    {
                        teamNum++;
                        break;
                    }
                }
                if (teamNum != 1)
                    break;
#if !SERVER
                Debug.Log("加入最后一个队伍:" + wint.team + "    name:" + wint.playerData.name);
#endif
                OnPlayerEnterEnd(wint, EndType.ET_Win, frame);
                battleData.winTEAM = wint.team;
                break;
            }
        }


//        // 如果已经有三个队伍都是died，则将最后一个加入
//        if (diedTeamCount == battleData.currentPlayers - 1)
//        {
//			for (int i = 0; i < (int)TEAM.TeamMax; ++i) {
//				Team wint = battleSystem.sceneManager.teamManager.GetTeam((TEAM)i);
//				if (wint != null && wint.Valid() && !wint.isEnd) {
//#if !SERVER
//					Debug.Log("加入最后一个队伍:" + wint.team + "    name:" + wint.playerData.name);
//#endif
//					OnPlayerEnterEnd(wint, EndType.ET_Win, frame);
//					battleData.winTEAM = wint.team;
//					break;
//				}
//			}
//		}

		// 如果还是有三个队伍died，其实上一步已经把第四个加入了结果，此时结束就好了
		if (teamNum == 1)
		{
			QuitBattle(true);

		}
		else
		{
			if (team == battleData.currentTeam)
			{
				// 投降则进入结束流程，正常死亡则进观战
				if (type == EndType.ET_Giveup)
				{
					QuitBattle();
				}
				else
				{
#if !SERVER
					BattleSystem.Instance.battleData.gameState = GameState.GameWatch;
#endif
					EventSystem.Instance.FireEvent(EventId.OnSelfDied);
				}
			}
		}

		// 界面刷新
		if (type == EndType.ET_Giveup)
		{
			EventSystem.Instance.FireEvent(EventId.PlayerGiveUp, team);
		}
		else if (type == EndType.ET_Dead)
		{
			EventSystem.Instance.FireEvent(EventId.PlayerDead, team);
		}
	}

	// 发送
	private void SendGiveUpFrame()
	{
#if !SERVER
		FramePacket packet  = new FramePacket ();
		packet.type         = 2;
		packet.giveup       = new GiveUpPacket ();
		packet.giveup.team  = battleData.currentTeam;
		byte[] bytestr      = Json.EnCodeBytes (packet);
		NetMessage.PbFrame pb = new PbFrame ();
		NetMessage.CSFrame build = new CSFrame ();

		pb.content = bytestr;
		build.frame = pb;

		NetSystem.Instance.Send<NetMessage.CSFrame> ((int)NetMessage.MsgId.ID_CSFrame, build);
#endif
	}


	// 发送退出战斗指令
	public void QuitBattle(bool finish = false)
	{
		//Debug.LogFormat ("QuitBattle   finished:{0}", finish);

		battleSystem.StopLockStep();

        #if !SERVER
		if (UISystem.Instance.IsWindowVisible ("ResumingWindow"))
		UISystem.Instance.HideWindow ("ResumingWindow");

		if (finish)
		{
			UISystem.Get ().ShowWindow ("BattleEndWindow");
			// 闪白
			Team winTeam = BattleSystem.Instance.sceneManager.teamManager.GetTeam (battleData.winTEAM);
			//Debug.Log ("闪白的队伍:" + winTeam.team + "    name:" + winTeam.playerData.name);
			Color winColor = winTeam.color;
			BattleSystem.Instance.sceneManager.ShowWinEffect (winTeam, winColor);
			EventSystem.Instance.FireEvent (EventId.OnFinishedColor, winTeam.color, winTeam);
		}
		else
		{
			// 观战模式战斗中退出不需要结果界面
			if (battleData.gameState != GameState.Watcher)
			{
				UISystem.Get ().ShowWindow ("BattleEndWindow");
			}
		}
        #endif

		if (battleData.isReplay)
		{
			
		}
		else
		{
			// 直接从这儿设置状态为战斗结束
			battleSystem.battleData.gameState = GameState.GameEnd;

			//发送消息
			NetMessage.CSQuitBattle quit = new NetMessage.CSQuitBattle();
			//System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			for (int i = 0; i < battleEndDatas.Count; ++i) {
				BattleEndData data = battleEndDatas[i];
				NetMessage.EndEvent e = new NetMessage.EndEvent();
				e.userid = data.userId;
				e.end_type = data.endType;
				e.end_frame = data.endFrame;
				e.end_destroy = data.destroy;
				e.end_survive = data.survive;
				quit.events.Add(e);

				//sb.Append (string.Format ("[{0},{1},{2},{3}];", data.userName, data.userId, data.endType, data.endFrame));
			}
			NetSystem.Instance.Send<NetMessage.CSQuitBattle>((int)NetMessage.MsgId.ID_CSQuitBattle, quit);
		}
	}

	public void UpdateResuming (int frame, float interval)
	{
		if (battleData.silent && battleSystem.lockStep.messageCount < 5) {
			battleData.resumingFrame = -1;
			battleSystem.sceneManager.SilentMode (false);
#if !SERVER
			UISystem.Instance.HideWindow ("ResumingWindow");
#endif
		}
	}

    void UpdateBattleSpeed()
    {
        if (speedUp)
            return;

        int elapsed = Mathf.RoundToInt(BattleSystem.Instance.sceneManager.GetBattleTime());
        if (elapsed >= 60 * 3)
        {
            speedUp = true;
            FramePacket packet = new FramePacket();
            packet.type = 3;
            byte[] bytestr = Json.EnCodeBytes(packet);
            NetMessage.PbFrame pb = new PbFrame();
            NetMessage.CSFrame build = new CSFrame();
            pb.content = bytestr;
            build.frame = pb;
            NetSystem.Instance.Send<NetMessage.CSFrame>((int)NetMessage.MsgId.ID_CSFrame, build);
            EventSystem.Instance.FireEvent(EventId.OnPVPBattleAccelerate, null);
        }
    }

    void UpdateBombPlanet()
    {
        //List<Node> nodes = BattleSystem.Instance.sceneManager.nodeManager.GetUsefulNodeList();
        //if (string.IsNullOrEmpty(centerPlanet))
        //{
        //    float mindist = 100.0f;
        //    float[] mindists = new float[(3)];
        //    mindists[0] = mindists[1] = mindists[2] = 100.0f;
        //    string[] centerPlanets = new string[(3)];
        //    Vector3[] centers = new Vector3[(3)];
        //    foreach (var node in nodes)
        //    {
        //        float dist = (node.GetPosition() - Vector3.zero).magnitude;
        //        if (mindists[0] == 100.0f)
        //        {
        //            mindists[0] = dist;
        //            centerPlanets[0] = node.tag;
        //            centers[0] = node.GetPosition();
        //        }
        //        else
        //        {
        //            if (dist <= mindists[0])
        //            {
        //                mindists[2] = mindists[1];
        //                centerPlanets[2] = centerPlanets[1];
        //                centers[2] = centers[1];
        //                mindists[1] = mindists[0];
        //                centerPlanets[1] = centerPlanets[0];
        //                centers[1] = centers[0];
        //                mindists[0] = dist;
        //                centerPlanets[0] = node.tag;
        //                centers[0] = node.GetPosition();
        //            }
        //            else
        //            {
        //                if (mindists[1] == 100.0f)
        //                {
        //                    mindists[1] = dist;
        //                    centerPlanets[1] = node.tag;
        //                    centers[1] = node.GetPosition();
        //                }
        //                else
        //                {
        //                    if (dist <= mindists[1])
        //                    {
        //                        mindists[2] = mindists[1];
        //                        centerPlanets[2] = centerPlanets[1];
        //                        centers[2] = centers[1];
        //                        mindists[1] = dist;
        //                        centerPlanets[1] = node.tag;
        //                        centers[1] = node.GetPosition();
        //                    }
        //                    else
        //                    {
        //                        if (mindists[2] == 100.0f || dist < mindists[2])
        //                        {
        //                            mindists[2] = dist;
        //                            centerPlanets[2] = node.tag;
        //                            centers[2] = node.GetPosition();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (mindists[0] == 100.0f && mindists[1] == 100.0f && mindists[2] == 100.0f)
        //        return;
        //    int hit = BattleSystem.Instance.battleData.rand.Range(0, 2);
        //    Debug.Log("RandomSprite7");
        //    if (mindists[hit] == 100.0f)
        //    {
        //        if (mindists[(hit + 1) % 3] == 100.0f)
        //            if (mindists[(hit + 2) % 3] == 100.0f)
        //                return;
        //            else
        //                hit = (hit + 2) % 3;
        //        else
        //            hit = (hit + 1) % 3;
        //    }
        //    mindist = mindists[hit];
        //    centerPlanet = centerPlanets[hit];
        //    center = centers[hit];
        //}
        //if (string.IsNullOrEmpty(centerPlanet))
        //    return;
        //int elapsed = Mathf.RoundToInt(BattleSystem.Instance.sceneManager.GetBattleTime());
        ////if (elapsed < 10)
        //if (elapsed < 60 * 5)
        //    return;
        ////五分钟时，发送清除所有障碍的消息,只发送一次
        //if (hasTriggerClearBarrier == false)
        //{
        //    hasTriggerClearBarrier = true;
        //    FramePacket packet = new FramePacket();
        //    packet.type = 13;
        //    byte[] bytestr = Json.EnCodeBytes(packet);
        //    NetMessage.PbFrame pb = new PbFrame();
        //    NetMessage.CSFrame build = new CSFrame();
        //    pb.content = bytestr;
        //    build.frame = pb;
        //    NetSystem.Instance.Send<NetMessage.CSFrame>((int)NetMessage.MsgId.ID_CSFrame, build);
        //}
        //if (nodes.Count <= 1)
        //    return;
        //if (hasSentBoomEvent == false) {
        //    hasSentBoomEvent = true;
        //    EventSystem.Instance.FireEvent(EventId.OnPVPBattleBoom, null);
        //}

        //if (lastElapsed == 0 || (elapsed - lastElapsed) >= 5)
        //{
        //    lastElapsed = elapsed;
        //    if (narrowtimes == -1)
        //    {
        //        narrowtimes = 0;
        //    }
        //    if (bombTarget == null)
        //    {
        //        //找到要销毁的星球
        //        float maxdist = 0.0f;
        //        float[] maxdists = new float[(3)];
        //        maxdists[0] = maxdists[1] = maxdists[2] = 0.0f;
        //        string[] bombTargets = new string[(3)];
        //        foreach (var node in nodes)
        //        {
        //            float dist = (node.GetPosition() - center).magnitude;
        //            if (maxdists[0] == 0.0f)
        //            {
        //                maxdists[0] = dist;
        //                bombTargets[0] = node.tag;
        //            }
        //            else
        //            {
        //                if (dist >= maxdists[0])
        //                {
        //                    maxdists[2] = maxdists[1];
        //                    bombTargets[2] = bombTargets[1];
        //                    maxdists[1] = maxdists[0];
        //                    bombTargets[1] = bombTargets[0];
        //                    maxdists[0] = dist;
        //                    bombTargets[0] = node.tag;
        //                }
        //                else
        //                {
        //                    if (maxdists[1] == 0.0f)
        //                    {
        //                        maxdists[1] = dist;
        //                        bombTargets[1] = node.tag;
        //                    }
        //                    else
        //                    {
        //                        if (dist >= maxdists[1])
        //                        {
        //                            maxdists[2] = maxdists[1];
        //                            bombTargets[2] = bombTargets[1];
        //                            maxdists[1] = dist;
        //                            bombTargets[1] = node.tag;
        //                        }
        //                        else
        //                        {
        //                            if (maxdists[2] == 0.0f || dist > maxdists[2])
        //                            {
        //                                maxdists[2] = dist;
        //                                bombTargets[2] = node.tag;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (maxdists[0] == 0.0f && maxdists[1] == 0.0f && maxdists[2] == 0.0f)
        //            return;
        //        int hit = BattleSystem.Instance.battleData.rand.Range(0, 2);
        //        Debug.Log("RandomSprite8");
        //        if (maxdists[hit] == 0.0f)
        //        {
        //            if (maxdists[(hit + 1) % 3] == 0.0f)
        //                if (maxdists[(hit + 2) % 3] == 0.0f)
        //                    return;
        //                else
        //                    hit = (hit + 2) % 3;
        //            else
        //                hit = (hit + 1) % 3;
        //        }
        //        maxdist = maxdists[hit];
        //        bombTarget = BattleSystem.Instance.sceneManager.nodeManager.GetNode(bombTargets[hit]);
        //        if (bombTarget != null)
        //        {
        //            //播放倒计时特效
        //            FramePacket packet = new FramePacket();
        //            packet.type = 16;
        //            packet.effect = new DriftEffect();
        //            packet.effect.tag = bombTarget.tag;
        //            packet.effect.effect = "Eff_XJ_Djs";
        //            packet.effect.time = 10f;
        //            byte[] bytestr = Json.EnCodeBytes(packet);
        //            NetMessage.PbFrame pb = new PbFrame();
        //            NetMessage.CSFrame build = new CSFrame();
        //            pb.content = bytestr;
        //            build.frame  = pb;
        //            NetSystem.Instance.Send<NetMessage.CSFrame>((int)NetMessage.MsgId.ID_CSFrame, build);
        //            //EffectManager.Get().PlayDriftSkillEffect(bombTarget, "Eff_XJ_Djs", 10f);
        //            narrowtimes = -1;
        //        }
        //    }
        //}
        //if (narrowtimes >= 0 && bombTarget != null)
        //{
        //    if (narrowtimes == 0)
        //    {
        //        FramePacket packet = new FramePacket();
        //        packet.type = 6;
        //        packet.effect = new DriftEffect();
        //        packet.effect.tag = bombTarget.tag;
        //        packet.effect.effect = "EFF_XJ_Boom_1";
        //        packet.effect.time = 6f;
        //        packet.effect.scale = bombTarget.GetWidth() / 0.2f;
        //        byte[] bytestr = Json.EnCodeBytes(packet);
        //        NetMessage.PbFrame pb = new PbFrame();
        //        NetMessage.CSFrame build = new CSFrame();
        //        pb.content = bytestr;
        //        build.frame = pb;
        //        NetSystem.Instance.Send<NetMessage.CSFrame>((int)NetMessage.MsgId.ID_CSFrame, build);
        //        //EffectManager.Get().PlayDriftSkillEffect(bombTarget, "EFF_XJ_Boom_1", 6f);
        //    }
        //    if (bombTarget.GetWidth() > 0.02f)
        //    {
        //        //bombTarget.SetScale(bombTarget.GetScale() * 0.95f);
        //    }
        //    narrowtimes++;
        //    if (narrowtimes >= 90)
        //    {
        //        FramePacket packet = new FramePacket();
        //        packet.type = 4;
        //        packet.bomb = new PlanetBomb();
        //        packet.bomb.tag = bombTarget.tag;
        //        byte[] bytestr = Json.EnCodeBytes(packet);
        //        NetMessage.PbFrame pb = new PbFrame();
        //        NetMessage.CSFrame build = new CSFrame();
        //        pb.content = bytestr;
        //        build.frame = pb;
        //        NetSystem.Instance.Send<NetMessage.CSFrame>((int)NetMessage.MsgId.ID_CSFrame, build);
        //        bombTarget = null;
        //        narrowtimes = -2;
        //    }
        //}
    }
}

