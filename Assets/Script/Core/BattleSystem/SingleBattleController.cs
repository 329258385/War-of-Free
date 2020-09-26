using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plugin;
using Solarmax;
using NetMessage;






/// <summary>
/// 单人战斗管理器
/// </summary>
public class SingleBattleController : IBattleController
{
    private BattleSystem        battleSystem;
    public SceneManager         sceneManager;
    public BattleData           battleData;
    private int                 tickEndCount;
    private List<BattleEndData> battleEndDatas = new List<BattleEndData>();
    private Packet[]            emptyPackets = null;
    private int                 emptyPacketId = 0;

    private bool useCommonEndCondition = true;

    public SingleBattleController(BattleData bd, BattleSystem bs )
    {
        battleData      = bd;
        battleSystem    = bs;
    }

    public bool Init()
    {
        tickEndCount = 0;

        List<Packet> packets = new List<Packet>();
        emptyPackets = packets.ToArray();
        emptyPacketId = 0; // 外部已增加了第一个帧包，则此处使用前++加入第二个
        battleSystem.battleData.battleType = BattlePlayType.Normalize;
        // 加入第一个
        BattleSystem.Instance.lockStep.AddFrame(++emptyPacketId, emptyPackets);

        // 判断一下用那种侦测结束方式
        int validTeamCount = 0;
        for (int i = 0; i < (int)TEAM.TeamMax; ++i)
        {
            Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
            if (t != null && t.Valid())
            {
                ++validTeamCount;
            }
        }

        if( LocalPlayer.Get().playerData.currentTeam != null )
        {
            battleData.currentTeam = LocalPlayer.Get().playerData.currentTeam.team;
        }
        
        useCommonEndCondition   = validTeamCount > 1;
        ThirdPartySystem.Instance.OnStartPve(battleData.matchId);

        return true;
    }

    public void Tick(int frame, float interval)
    {
        if (useCommonEndCondition)
        {
            UpdateEnd(frame, interval);
        }
        else
        {
            UpdateEndAllOccupied(frame, interval);
        }

        UpdateEndOtherWinLoseType(frame, interval);


        // 自驱动型的单机，判断剩余包数量是不是为0，为0，则继续加入包
        if (BattleSystem.Instance.lockStep.messageCount < 2)
        {
            BattleSystem.Instance.lockStep.AddFrame(++emptyPacketId, emptyPackets);
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
        for (int i = 0; i < frame.users.Count; i++)
        {
            int uid = frame.users[i];
            NetMessage.PbFrames pbs = frame.frames[i];
            if (pbs == null || pbs.frames.Count == 0)
                continue;

            for (int k = 0; k < pbs.frames.Count; k++)
            {
                NetMessage.PbFrame pb = pbs.frames[k];
                if (pb == null)
                    continue;

                Packet move = new Packet();
                move.team   = (TEAM)(uid + 1);
                move.packet = Json.DeCode<FramePacket>(pb.content);
                list.Add(move);
            }
        }
        battleSystem.lockStep.AddFrame(frame.frameNum, list.ToArray());
    }

    public void OnRecievedScriptFrame(NetMessage.PbSCFrames frame)
    { }

    public void OnRunFramePacket(FrameNode frameNode)
    {
        battleSystem.sceneManager.RunFramePacket(frameNode);
    }

    public void OnPlayerMove(/*Node from, Node to, float percent*/)
    {
        //{
        //    int shipNum = from.GetShipCount((int)BattleSystem.Instance.battleData.currentTeam);
        //    if (shipNum == 0)
        //        return;

        //    FramePacket packet  = new FramePacket();
        //    packet.type         = 0;
        //    packet.move         = new MovePacket();
        //    packet.move.from    = from.tag;
        //    packet.move.to      = to.tag;
        //    packet.move.rate    = percent == -1f ? BattleSystem.Instance.battleData.sliderRate : percent;

        //    byte[] byteString   = Json.EnCodeBytes(packet);


        //    NetMessage.PbFrame fpb = new NetMessage.PbFrame();
        //    fpb.content = byteString;
        //    ReplayCollectManager.Get().AddReplayFrame(fpb);
        //}

        //from.nodeManager.MoveTo(from, to, battleData.currentTeam, percent);
    }

    public void PlayerGiveUp()
    {
        QuitBattle(false);
    }

    public void OnPlayerGiveUp(TEAM giveUpTeam)
    {
        
    }

    public void OnPlayerDirectQuit(TEAM team)
    {

    }

    /// <summary>
    /// 检查是否到达结果
    /// </summary>
    private void UpdateEnd(int frame, float dt)
    {
        if (++tickEndCount == 5)
        {
            // 先查询谁死了
            for (int i = 0; i < (int)TEAM.TeamMax; ++i)
            {
                Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
                if (t != null && t.Valid() && !t.isEnd)
                {
                    if (t.CheckPVEDead())
                    {
                        // 死亡
                        OnPlayerDiedOrGiveUp(EndType.ET_Dead, t.team, frame);
                    }
                }
            }

            if (battleEndDatas.Count != battleData.currentPlayers)
                tickEndCount = 0;
        }
    }

    private void OnPlayerEnterEnd(Team t, EndType type, int frame)
    {
        BattleEndData data = new BattleEndData();
        data.team = t.team;
        data.userId = t.playerData.userId;
        data.destroy = t.destory;
        data.endType = type;
        data.endFrame = frame;
        t.isEnd = true;

        battleEndDatas.Add(data);
    }

    public void OnPlayerDiedOrGiveUp(EndType type, TEAM team, int frame)
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

        // 判断是不是总人数-1，如果是，则将第一名加入
        if (battleEndDatas.Count == battleData.currentPlayers - 1)
        {
            for (int i = 0; i < (int)TEAM.TeamMax; ++i)
            {
                Team t = BattleSystem.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
                if (t != null && t.Valid() && !t.isEnd)
                {
                    OnPlayerEnterEnd(t, EndType.ET_Win, frame);
                    battleData.winTEAM = t.team;
                    break;
                }
            }
        }

        // 出现结果，结束战斗
        if (battleEndDatas.Count == battleData.currentPlayers)
        {
            QuitBattle(true);
        }
    }

    // 发送退出战斗指令
    public void QuitBattle(bool finish = false)
    {
#if !SERVER
        
        BattleSystem.Instance.StopLockStep();


        if (BattleSystem.Instance.battleData.gameType != GameType.TestLevel &&
            BattleSystem.Instance.battleData.gameType != GameType.SingleLevel)
        {
            // 单机关卡需要上报完成当前关卡
            if (battleData.currentTeam == battleData.winTEAM &&
                LocalPlayer.Get().playerData.singleFightNext)
            {
                
            }
            else
            {
                // 只有在创关卡的时候才设置
                if (LocalPlayer.Get().playerData.singleFightNext && finish)
                {
                    LocalLevelStorage.Get().Levelfails = LocalLevelStorage.Get().Levelfails + 1;
                    LocalLevelStorage.Get().SetLevelInfo(battleData.matchId);
                }
            }
        }


        if (finish)
        {
            // 闪白
            Team winTeam = BattleSystem.Instance.sceneManager.teamManager.GetTeam(battleData.winTEAM);
            Color winColor = winTeam.color;
            BattleSystem.Instance.sceneManager.ShowWinEffect(winTeam, winColor);

            Debug.Log("win team ***************************hitships: " + winTeam.hitships + "    destroy ******************:  " + winTeam.destory);
            UISystem.Get().ShowWindow("BattleEndWindow");
            //EventSystem.Instance.FireEvent(EventId.VictionaryWinTeam, winTeam);
            EventSystem.Instance.FireEvent(EventId.OnFinished);
            EventSystem.Instance.FireEvent(EventId.OnFinishedColor, winColor, winTeam);

            if (battleData.winTEAM != battleData.currentTeam)
            {
                Team ownTeam    = BattleSystem.Instance.sceneManager.teamManager.GetTeam(BattleSystem.Instance.battleData.currentTeam);
                int destorys    = ownTeam.destory;
                int hitships    = ownTeam.hitships;
                int produce     = ownTeam.produces;
                float totalTime = BattleSystem.Instance.sceneManager.GetBattleTime();
                Flurry.Instance.FlurryBattleEndEvent(battleData.matchId, "2", "0", "0", hitships.ToString(), destorys.ToString(), totalTime.ToString());
            }
            //sdk
            ThirdPartySystem.Instance.OnFinishPve(battleData.matchId);
        }
        else
        {
            // 记录失败事件
            Team ownTeam    = BattleSystem.Instance.sceneManager.teamManager.GetTeam(BattleSystem.Instance.battleData.currentTeam);
            int destorys    = ownTeam.destory;
            int hitships    = ownTeam.hitships;
            int produce     = ownTeam.produces;
            float totalTime = BattleSystem.Instance.sceneManager.GetBattleTime();
            Flurry.Instance.FlurryBattleEndEvent(battleData.matchId, "1", "0", "0", hitships.ToString(), destorys.ToString(), totalTime.ToString());


            // 关闭窗口退出
            UISystem.Get().HideAllWindow();
            BattleSystem.Instance.BeginFadeOut();

            //ShipFadeManager.Get().SetFadeType(ShipFadeManager.FADETYPE.OUT, 0.1f);
            //UISystem.Get().FadeBattle(false, new EventDelegate(() =>
            //{
            //    if (BattleSystem.Instance.battleData.gameType == GameType.Single)
            //    {
            //        BattleSystem.Instance.Reset();
            //        UISystem.Get().ShowWindow("CustomSelectWindow");
            //    }

            //    if (BattleSystem.Instance.battleData.gameType == GameType.TestLevel)
            //    {
            //        BattleSystem.Instance.Reset();
            //        UISystem.Get().ShowWindow("CustomTestLevelWindow");
            //    }

            //    if (BattleSystem.Instance.battleData.gameType == GameType.SingleLevel)
            //    {
            //        BattleSystem.Instance.Reset();
            //        UISystem.Get().ShowWindow("LobbyWindowView");
            //    }
            //    if (BattleSystem.Instance.battleData.gameType == GameType.PayLevel)
            //    {
            //        BattleSystem.Instance.Reset();
            //        UISystem.Get().ShowWindow("ChapterWindow");
            //        EventSystem.Instance.FireEvent(EventId.OnSingleBattleEnd, 0);
            //    }
            //}));
            //sdk
            ThirdPartySystem.Instance.OnFailPve(battleData.matchId);
        }
        // 扣除5点体力
        //LocalPlayer.Get().ChangePower(-5);
       
#endif
    }

    //重新挑战关卡
    public void Retry()
    {
        //停止帧逻辑
        BattleSystem.Instance.StopLockStep();
        // 记录失败事件
        Team ownTeam = BattleSystem.Instance.sceneManager.teamManager.GetTeam(BattleSystem.Instance.battleData.currentTeam);
        int destorys = ownTeam.destory;
        int hitships = ownTeam.hitships;
        int produce = ownTeam.produces;
        float totalTime = BattleSystem.Instance.sceneManager.GetBattleTime();
        Flurry.Instance.FlurryBattleEndEvent(battleData.matchId, "1", "0", "0", hitships.ToString(), destorys.ToString(), totalTime.ToString());
        //第三方打点
        ThirdPartySystem.Instance.OnFailPve(battleData.matchId);

        //重新发送单机关卡请求
        NetSystem.Instance.helper.RequestSingleMatch(battleData.matchId, GameType.SingleLevel,false);
        //进入战斗，直接显示所有星球和飞船
        BattleSystem.Instance.sceneManager.FadePlanet(true, 0f);
        //ShipFadeManager.Get().SetFadeType(ShipFadeManager.FADETYPE.IN, 0f);
        //启动帧逻辑
        BattleSystem.Instance.StartLockStep();
    }

    /// <summary>
    /// 所有全部销毁
    /// </summary>
    private void UpdateEndAllOccupied(int frame, float interval)
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

    private void UpdateEndOtherWinLoseType(int frame, float interval)
    {
        if (++tickEndCount == 5) // 0.1f 刷新一次
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

}
