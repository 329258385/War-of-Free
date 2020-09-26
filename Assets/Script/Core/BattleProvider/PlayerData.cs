using System;
using UnityEngine;
using System.Collections.Generic;
using Plugin;
using Solarmax;


/// <summary>
/// 简略玩家数据，用于各种功能的
/// 其中level及以上都是必选字段，即网络包一定会传过来。level以下为非必选，需要和功能制作人员确定是否已传递。
/// </summary>
[Serializable]
public class SimplePlayerData
{
	/// 以下为必选字段
	public int          userId;
	public string       name;
	public string       icon;
	public int          score;
	public int          level;

	// 以下为非必选字段
	public bool         online;
	public bool         onBattle;

	// 战报用option
	public int          scoreMod;	// 分数改变
	public int          destroyNum;	// 消灭飞船数
	public int          surviveNum;	// 剩余数量

	public int          raceId;
	public int          power;	    //体力值
    public int          money;      // 金钱

	public void Init(NetMessage.UserData sud)
	{
		// 必选字段的required
		userId          = sud.userid;
		name            = sud.name;
		icon            = sud.icon;
		score           = sud.score;
		level           = sud.level;
        //money           = sud.Money;

		// 非必选字段的optional
		online = false;
		if (sud.online) {
			online = sud.online;
		}
		onBattle = false;
		if (sud.onBattle) {
			onBattle = sud.onBattle;
		}

		scoreMod = sud.score_mod;
		destroyNum = sud.destroy_num;
		surviveNum = sud.survive_num;
        if(sud.battle_race != null )
		    raceId = sud.battle_race.race;
	}
}

/// <summary>
/// 背包
/// </summary>
public class Pack
{
	public class PackItem
	{
		public int id { get; set; }
		public int num { get; set; }
		public PackItem() {	id = 0; num = 0;	}
	}

	List<PackItem> items = new List<PackItem>();

	public void Init(NetMessage.Pack data)
	{
		items.Clear ();
		NetMessage.PackItem pi = null;
		for (int i = 0; i < data.items.Count; ++i) {
			pi = data.items[i];
			ModifyItem(pi.itemid, pi.num);
		}
	}

	public int GetItem(int id)
	{
		int ret = 0;
		for (int i = 0; i < items.Count; ++i) {
			if (items[i].id == id)
			{
				ret = items [i].num;
				break;
			}
		}
		return ret;
	}

	public void ModifyItem(int id, int num)
	{
		PackItem pi = null;
		for (int i = 0; i < items.Count; ++i) {
			if (items[i].id == id)
			{
				pi = items [i];
				break;
			}
		}
		if (pi == null) {
			pi = new PackItem ();
			items.Add (pi);
			pi.id = id;
		}
		pi.num = num;
	}

	public void Reset()
	{
		items.Clear ();
	}
}

/// <summary>
/// 玩家数据
/// </summary>
public class PlayerData
{
	/// <summary>
	/// 当前组队
	/// </summary>
	/// <value>The current team.</value>
	public Team currentTeam { get; set; }

	/// <summary>
	/// The user identifier.
	/// </summary>
	public int          userId;

	/// <summary>
	/// The name.
	/// </summary>
	public string       name;

	/// <summary>
	/// The icon.
	/// </summary>
	public string       icon;

	/// <summary>
	/// The score.
	/// </summary>
	public int          score;

	/// <summary>
	/// The level.
	/// </summary>
	public int          level;

    /// <summary>
    /// 金钱
    /// </summary>
    public int          money;

    /// <summary>
    /// 杀敌数 
    /// </summary>
    public int          destroyNum = 0;


	public string       unionName;

	/// <summary>
	/// 技能列表
	/// </summary>
	/// <value>The skills.</value>
	public List<int> castSkills = new List<int> ();

	/// <summary>
	/// 技能能量
	/// </summary>
	/// <value>The skillPower.</value>
	public int skillPower{ get; set; }

	/// <summary>
	/// 选中的种族
	/// </summary>
	public int          raceId;
	/// <summary>
	/// 选中的种族等级
	/// </summary>
	public int          raceLevel;
	/// <summary>
	/// 选中种族携带的技能
	/// </summary>
	public int[]        raceSkillId = new int[6];

	/// <summary>
	/// 服务器记录的单机关卡
	/// </summary>
	public string       singleFightLevel = string.Empty;


	/// <summary>
	/// 当前单机关卡是否为刷下一关模式
	/// </summary>
	public bool         singleFightNext = false;

    public int          mNewRaceID = -1;

    /// <summary>
    /// 角色注册时间
    /// </summary>
    public long         RegisteredtimeStamp = 0;

    public ChessItem[] chesses = new ChessItem[ChessItem.CHESS_MAX];
	public int timechestid;
	public int battlechestid;
	public Int64 timechest;
	public Int64 timechestcost;
	public int	curbattlechest;
	public int	maxbattlechest;

	// 背包数据
	public Pack pack = new Pack ();

	/// <summary>
	/// 种族数据
	/// </summary>
	public List<NetMessage.RaceData> raceList = new List<NetMessage.RaceData>();

	/// <summary>
	/// 通用保存数据
	/// </summary>
	public string[] clientStorages = new string[(int)NetMessage.ClientStorageConst.ClientStorageMaxIndex + 1];


	public int power;	//体力值

	public PlayerData()
	{
		power = 0;
	}

	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="data">Data.</param>
	public void Init(NetMessage.UserData data)
	{
		userId          = data.userid;
        name            = data.name;
        //当网络同步成功时，立刻将服务器存储名字覆盖本地名字，并存储
        if(!string.IsNullOrEmpty(name))
        {
            LocalAccountStorage.Get().name = name;
            LocalStorageSystem.Instance.SaveLocalAccount( );
        }
        icon            = data.icon;
        score           = data.score;
        //level           = data.Level;
        //money           = 0;
        //unionName       = "";
        //skillPower      = 0;

        if (data.battle_race != null )
        {

            
        }

        if (data.pack != null )
        {
            pack.Init(data.pack);
        }

        if (data.chest != null )
        {
            chesses = new ChessItem[ChessItem.CHESS_MAX];
            for (int i = 0; i < data.chest.items.Count; i++)
            {
                int slot = data.chest.items[i].slot;
                chesses[slot] = new ChessItem();
                chesses[slot].id = data.chest.items[i].id;
                if ((chesses[slot].timeout = data.chest.items[i].timeout) > 0)
                    chesses[slot].timefinish = new DateTime(1970, 1, 1).AddSeconds(data.chest.items[i].timeout);
                chesses[slot].slot = data.chest.items[i].slot;
            }
            timechestid = data.chest.chest_timeboxid;
            battlechestid = data.chest.chest_winboxid;
            timechest = data.chest.chest_gainpoint;
            timechestcost = data.chest.chest_gainconsume;
            curbattlechest = data.chest.chest_winnum;
            maxbattlechest = data.chest.chest_neednum;
        }
    }

	public void Init(PlayerData data)
	{
		userId      = data.userId;
		name        = data.name;
		icon        = data.icon;
		score       = data.score;
		level       = data.level;
        money       = data.money;
        power       = data.power;
		unionName   = "";
		skillPower  = 0;
	}


	/// <summary>
	/// 初始化种族数据
	/// </summary>
	/// <param name="raceData">Race data.</param>
	public void InitRace(IList<NetMessage.RaceData> raceDatas)
	{
		raceList.Clear ();
		raceList.AddRange (raceDatas);
		raceList.Sort ((args0, arg1) => {
			return args0.race.CompareTo(arg1.race);
		});
	}

	/// <summary>
	/// Reset this instance.
	/// </summary>
	public void Reset()
	{
		userId = -1;
		name = string.Empty;
		icon = string.Empty;
		score = 0;
		level = 0;
        raceId = 0;
		unionName = "";
		skillPower = 0;
		castSkills.Clear ();
		singleFightLevel = string.Empty;
		singleFightNext = false;
		raceList.Clear ();
		pack.Reset ();

		for (int i = 0; i < clientStorages.Length; ++i) {
			clientStorages [i] = string.Empty;
		}
	}
}

/// <summary>
/// 本地玩家
/// </summary>
public class LocalPlayer : Singleton<LocalPlayer> 
{
    /// <summary>
    /// 最大体力上限
    /// </summary>
    const int       MAX_POWER_LIMIT = 30;


	/// <summary>
	/// 本地保存的account
	/// </summary>
	private string localAccount;

	/// <summary>
	/// 玩家的数据
	/// </summary>
	public PlayerData playerData = new PlayerData();

	/// <summary>
	/// 玩家的匹配的战斗场人数数据
	/// </summary>
	public int CurBattlePlayerNum = 2;

	/// <summary>
	/// 当前选择战斗种族
	/// </summary>
	/// <value>The race.</value>
	public int battleRace { get; set; }

	/// <summary>
	/// 当前匹配玩家
	/// </summary>
	/// <value>The math player.</value>
	public List<PlayerData> mathPlayer { get; set; }

	/// <summary>
	/// 当前战斗地图
	/// </summary>
	/// <value>The battle map.</value>
	public string battleMap { get; set; }

	/// <summary>
	/// 账号被接管，被踢
	/// </summary>
	public bool isAccountTokenOver = false;
    public bool isRandomUpdate = false;

    /// <summary>
    /// 体力累计时间, 5分钟增加一点
    /// </summary>
    public float           PowerRefreshTime = 0;


 
    /// <summary>
    /// 获取本地保存的账号
    /// </summary>
    /// <returns>The local account.</returns>
    public string GetLocalAccount()
	{
		if (string.IsNullOrEmpty (localAccount)) {
			localAccount = LocalAccountStorage.Get().account;
		}
		return localAccount;
	}

	/// <summary>
	/// 生成本地账号
	/// </summary>
	/// <returns>The local account.</returns>
	public string GenerateLocalAccount(bool forceChange = false)
	{
		#if !SERVER
		localAccount = SystemInfo.deviceUniqueIdentifier;
		if (forceChange)
		{
			int rand = UnityEngine.Random.Range (0, 10000);
			localAccount = localAccount + "__force__" + rand.ToString();
		}
		LocalAccountStorage.Get().account = localAccount;
		LocalStorageSystem.Get ().NeedSaveToDisk ();
		#endif
		return localAccount;
	}


    public void ChangeMoney( int nVariety)
    {
        playerData.money += nVariety;
        EventSystem.Instance.FireEvent(EventId.UpdateMoney);
    }

    public void SetMoney( int curMoney )
    {
        playerData.money = curMoney;
        EventSystem.Instance.FireEvent(EventId.UpdateMoney);
    }

    //const float ADD_POWER_INTERVAL = 300;
    //public void Tick(float interval)
    //{
    //    PowerRefreshTime += interval;
    //    if (PowerRefreshTime >= ADD_POWER_INTERVAL)
    //    {
    //        PowerRefreshTime = 0;
    //        if ( playerData.power < MAX_POWER_LIMIT )
    //            ChangePower(1);
    //    }
    //}

    /// <summary>
    /// 处理离线的体力奖励
    /// </summary>
    //public void OperatorOffLine( int nAllSecond )
    //{
    //    if( nAllSecond > 0 )
    //    {
    //        int nMod = (int)(nAllSecond / ADD_POWER_INTERVAL);
    //        PowerRefreshTime = nAllSecond % ADD_POWER_INTERVAL;

    //        if (playerData.power < MAX_POWER_LIMIT) {
    //            if (nMod + playerData.power <= MAX_POWER_LIMIT) {
    //                playerData.power = nMod + playerData.power;
    //            } else {
    //                playerData.power = MAX_POWER_LIMIT;
    //            }
    //            EventSystem.Instance.FireEvent(EventId.UpdatePower);
    //        }
    //    }
    //}
}

