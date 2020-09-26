using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plugin;
using Solarmax;

public enum GameState
{
	Init,
	Game,
	GameWatch,
	Watcher,
	GameEnd,
}

public enum GameType
{
	Single = 1,
	PVP = 2,
	League = 3,
    PayLevel = 4,
    TestLevel = 5,
	SingleLevel = 6,
}


public enum BattlePlayType
{
    Normalize = 1,  // 正常战斗,包含 pve pvp 
    Replay = 2,  // 战斗回放
}


public class BattleData : Lifecycle2
{
	public GameObject root;
	public GameState gameState = GameState.Init;
	public GameType gameType = GameType.PVP;
    public BattlePlayType battleType = BattlePlayType.Normalize;
    public NetMessage.CooperationType battleSubType = NetMessage.CooperationType.CT_Null;

    public Rand rand;

	public string matchId;
	public int difficultyLevel;		//难度等级。用于单机关卡 
	public int aiLevel;	//AI 级别。用于单机关卡
    public int aiStrategy;
    public int aiParam;
    public string dyncDiffType;
    public string winType;
    public string winTypeParam1;
    public string winTypeParam2;
    public string loseType;
    public string loseTypeParam1;
    public string loseTypeParam2;

    public MapConfig currentTable;

	public TEAM currentTeam;

	public TEAM winTEAM;

	public float sliderRate;

	public bool isFakeBattle = false;

	public bool isReplay = false;

	public bool teamFight = false;

	public bool useAI = true;

    public bool useCommonEndCondition = false;

    public bool runWithScript = false;

    public bool isLevelReplay = false;

    public bool vertical { 
		//get { return currentTable != null ? currentTable.vertical : false; } 
		get { return false;}
		set { if (currentTable == null) return;
			currentTable.vertical = value; }
	}

	public int currentPlayers 
	{
		get { return currentTable != null ? currentTable.player_count : 0; }
		set { if (currentTable == null) return;
			currentTable.player_count = value; }
	}
	
	public string mapAudio 
	{
		get { return currentTable != null ? currentTable.audio : null; }
		set { if (currentTable == null) return;
 			currentTable.audio = value; }
	}

    public string defaultAI
    {
        get { return currentTable != null ? currentTable.defaultai : null; }
        set
        {
            if (currentTable == null) return;
            currentTable.defaultai = value;
        }
    }

    public string teamAI
    {
        get { return currentTable != null ? currentTable.teamAITypes : null; }
        set
        {
            if (currentTable == null) return;
            currentTable.teamAITypes = value;
        }
    }

    public bool mapEdit = false;

	#if SERVER
	public bool RefereeBusy = false;
	public bool RefereeFinish = false;
	#endif

	public bool silent = false;
	public int resumingFrame = -1;

	public BattleData ()
	{
		rand = new Rand (0);

	}

	public bool Init ()
	{
		gameState = GameState.Init;
		gameType = GameType.SingleLevel;
        battleSubType = NetMessage.CooperationType.CT_Null;

        matchId = string.Empty;
		//difficultyLevel = 0; // 这个不要清理了吧，难度和星星展示还是要区分开
		currentTable = null;
		currentTeam = TEAM.Neutral;
		winTEAM = TEAM.Neutral;
		sliderRate = 1.0f;
		isFakeBattle = false;
		isReplay = false;
		teamFight = false;
		//mapEdit = false;

		silent = false;
        //resumingFrame = -1; // modify for jira-491
        aiStrategy = -1;

        return true;
	}

	public void Tick (int frame, float interval)
	{
		
	}

	public void Destroy ()
	{
		
	}

}

