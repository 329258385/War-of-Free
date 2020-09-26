using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Solarmax;

/// <summary>
/// 队伍基本信息管理
/// </summary>
public class TeamManager : Lifecycle2
{
	static Team[] STATIC_TEAM = new Team[(int)TEAM.TeamMax]{ 
		new Team { 
			color = new Color32(0xcc, 0xcc, 0xcc, 0x00), 
			team = TEAM.Neutral,
			iconname = "avatar_bg_normal",
		},

		new Team { 
			color = new Color32(0x5f, 0xb6, 0xff, 0x00), 
			team = TEAM.Team_1,
			iconname = "avatar_bg_faction_blue",
		},

		new Team { 
			color = new Color32(0xff, 0x5d, 0x93, 0x00), 
			team = TEAM.Team_2,
			iconname = "avatar_bg_faction_red",
		},

		new Team { 
			color = new Color32(0xff, 0x8c, 0x5a, 0x00), 
			team = TEAM.Team_3,
			iconname = "avatar_bg_faction_yellow",
		},

		new Team { 
			color = new Color32(0xca, 0xff, 0x6e, 0x00), 
			team = TEAM.Team_4,
			iconname = "avatar_bg_faction_green",
		},

		new Team { 
			color = new Color32(0x99, 0x99, 0x99, 0x00), 
			team = TEAM.Team_5,
		},

		new Team { 
			color = new Color32(0x99, 0x99, 0x99, 0x00), 
			team = TEAM.Team_6,
		},

		new Team {
			color = new Color32(0xfe, 0xc5, 0x36, 0x00), 
			team = TEAM.Team_Black1,
		},
		new Team { 
			color = new Color32(0x1b, 0x92, 0x4b, 0x00), 
			team = TEAM.Team_Black2,
		},
	};

	/// <summary>
	/// 队伍管理
	/// </summary>
	/// <value>The scene manager.</value>
	public SceneManager sceneManager { get; set; }

	/// <summary>
	/// 队伍信息
	/// </summary>
	Team[] teamArray = new Team[STATIC_TEAM.Length];

	public TeamManager(SceneManager mgr)
	{
		sceneManager = mgr;
	}
		
	public bool Init()
	{
		for (int i = 0; i < teamArray.Length; i++)
		{
            
            //  {
              
            //  color       = STATIC_TEAM[i].color,
			//	team        = STATIC_TEAM[i].team,
			//	iconname    = STATIC_TEAM[i].iconname,
			//	teamManager = this
			//};

            CampConfig camp    = CampConfigConfigProvider.Instance.GetData(i.ToString());
            if( camp != null )
            {
                teamArray[i]        = new Team(camp);
                teamArray[i].team   = (TEAM)i;
                teamArray[i].color  = camp.campcolor;
                teamArray[i].teamManager = this;
            }
        }

		Release ();
		return true;
	}

	public void Tick(int frame, float interval)
	{
		//skillcd
		for (int i = 0; i < teamArray.Length; i++)
		{
			teamArray[i].UpdateEvent(frame, interval);
		}

		//SkillPower
		if (frame % 50 != 0)
			return;
		//方式二：游戏时间的增长，获得原子能力。1点／s。
		for (int i = 0; i < teamArray.Length; i++)
		{
			if (teamArray[i].playerData == null)
				continue;
			teamArray[i].playerData.skillPower++;
		}
	}

	public void Destroy()
	{
		Release ();
	}

	/// <summary>
	/// 释放资源
	/// </summary>
	public void Release()
	{
		foreach (Team team in teamArray) {
			team.Clear ();
		}
	}

	/// <summary>
	/// 获取team
	/// </summary>
	/// <returns>The team.</returns>
	/// <param name="team">Team.</param>
	public Team GetTeam(TEAM team)
	{
		return teamArray [(int)team];
	}

	public Team GetTeam(int userId)
	{
		for (int i = 0; i < teamArray.Length; i++) {

			if (teamArray [i].playerData.userId != userId)
				continue;
			return teamArray [i];
		}

		return null;
	}

	/// <summary>
	/// 获取玩家team
	/// </summary>
	/// <returns>The TEA.</returns>
	/// <param name="uid">Uid.</param>
	public TEAM GetTEAM(int userId)
	{
		Team t = GetTeam (userId);
		if (t != null)
			return t.team;
		
		return TEAM.Neutral;
	}
		
	/// <summary>
	/// 硕果仅存.检测是否是唯一存在的team
	/// </summary>
	/// <param name="eTEAM">E TEA.</param>
	public bool OnlyTeam(int eTEAM)
	{
		for (int i = 0; i < teamArray.Length; i++) 
		{
			if ((int)teamArray [i].team != eTEAM && teamArray [i].current > 0)
				return false;
		}
		return true;
	}
		
	/// <summary>
	/// 获取唯一Team，返回TEAM.TeamMax为不唯一结果。
	/// 更新：对于组队模式下，返回剩余队伍中人员最多的那个
	/// </summary>
	public int OnlyTeam()
	{
		int eTeam = (int)TEAM.TeamMax;
		for (int i = 0; i < teamArray.Length; i++) 
		{
			if (teamArray [i].current > 0) 
			{
				if (eTeam != (int)TEAM.TeamMax) 
				{
					if (teamArray [eTeam].IsFriend (teamArray [i].groupID)) {
						if (teamArray [i].current > teamArray [eTeam].current) {
							eTeam = (int)teamArray [i].team;
						}
					}else {
						return (int)TEAM.TeamMax;
					}
				} 
				else
				{
					eTeam = (int)teamArray [i].team;
				}
			}
		}
		return eTeam;
	}

	/// <summary>
	/// 增加摧毁数量
	/// </summary>
	public void AddDestory(TEAM team)
	{
		Team t = GetTeam (team);
		if (t == null)
			return;

		t.destory++;
	}


    /// <summary>
	/// 增加摧毁数量
	/// </summary>
    public void AddHitShip(TEAM team )
    {
        Team t = GetTeam(team);
        if (t == null)
            return;

        t.hitships++;
    }

    /// <summary>
    /// 统计生产飞船数
    /// </summary>
    /// <param name="team"></param>
    public void AddProduce(TEAM team, int num )
    {
        Team t = GetTeam(team);
        if (t == null)
            return;

        t.produces += num;
    }


	/// <summary>
	/// 获取摧毁飞船数组
	/// </summary>
	/// <returns>The destory array.</returns>
	public int[] GetDestoryArray()
	{
		int[] destoryArray = new int[(int)TEAM.TeamMax];

		for (int i = 0; i < (int)TEAM.TeamMax; i++) {

			Team t = GetTeam ((TEAM)i);

			if (t == null)
				continue;

			if (t.Valid()) {
				destoryArray [i] = t.destory;
			}
			else {
				destoryArray [i] = -1;
			}
		}

		return destoryArray;
	}

	/// <summary>
	/// Calculates the skill tar.
	/// </summary>
	/// <returns>The skill tar.</returns>
	/// <param name="source">Source.</param>
	/// <param name="tab">Tab.</param>
	//public TEAM CalcSkillTar(TEAM source, SkillConfig tab)
	//{
	//	TEAM act = TEAM.Neutral;
	//	switch((SKILL_TYPE)tab.skilltype)
	//	{
	//	case SKILL_TYPE.NODE_BUFF:
	//	case SKILL_TYPE.SHIP_BUFF:
	//	case SKILL_TYPE.SHIP_NOTHURT:
	//	case SKILL_TYPE.SHIP_SHADOW:
	//	case SKILL_TYPE.TEAM_BUFF:
	//	case SKILL_TYPE.SHIP_MUTEX:
	//	case SKILL_TYPE.TEAM_SHIP_ADD:
	//	case SKILL_TYPE.SHIP_RELIFE:
	//	case SKILL_TYPE.SHIP_CANWARP:
	//	case SKILL_TYPE.NODE_SHIPGETDOWN:
	//	case SKILL_TYPE.NODE_MUTEX:
	//	case SKILL_TYPE.NODE_STORM:
	//	case SKILL_TYPE.NODE_DEFENCE_CAP:
	//	case SKILL_TYPE.NODE_DOOM:
	//		{
	//			act = source;
	//			break;
	//		}
	//	case SKILL_TYPE.NODE_DEBUFF:
	//	case SKILL_TYPE.SHIP_DEBUFF_EX:
	//	case SKILL_TYPE.TEAM_PAUSE_PRO:
	//		{
	//			act = TEAM.TeamMax;
	//			break;
	//		}
	//	case SKILL_TYPE.SHIP_DEBUFF:
	//		{
	//			for (int i = 1; i < (int)TEAM.TeamMax; i++) {
	//				Team team = GetTeam ((TEAM)i);
	//				if (team.team == TEAM.Neutral)
	//					continue;
	//				if (team.team == source)
	//					continue;

	//				act = team.team;
	//				break;
	//			}
	//			break;
	//		}
	//	case SKILL_TYPE.NODE_ATTACK:
	//	case SKILL_TYPE.NODE_ICE:
	//	case SKILL_TYPE.NODE_PAUSE_PRO:
	//		{
	//			int shipcount = 0;
	//			for (int i = 1; i < (int)TEAM.TeamMax; i++) {
	//				Team team = GetTeam ((TEAM)i);
	//				if (team.team == TEAM.Neutral)
	//					continue;
	//				if (team.team == source)
	//					continue;
	//				if (team.current < shipcount)
	//					continue;
					
	//				shipcount = team.current;
	//				act = team.team;
	//			}
	//			break;
	//		}
	//	case SKILL_TYPE.NODE_PAUSE_CAP:
	//		{
	//			int shipcount = 0;
	//			for (int i = 1; i < (int)TEAM.TeamMax; i++) {
	//				Team team = GetTeam ((TEAM)i);
	//				if (team.team == TEAM.Neutral)
	//					continue;
	//				if (team.team == source)
	//					continue;
	//				if (team.current < shipcount)
	//					continue;

	//				shipcount = team.current;
	//				act = team.team;
	//			}
	//			break;
	//		}
	//	case SKILL_TYPE.TEAM_BATTLE_BUFF:
	//		{
	//			act = source;
	//			break;
	//		}
	//	case SKILL_TYPE.SHIP_DESTROY_CAP:
	//		{
	//			act = TEAM.TeamMax;
	//			break;
	//		}
	//	default:
	//		break;
	//	}

	//	return act;
	//}

	/// <summary>
	/// Adds the skill C.
	/// </summary>
	/// <param name="source">Source.</param>
	/// <param name="cd">Cd.</param>
	//public void AddSkillCD(TEAM source,SkillCoolDown cd)
	//{
	//	if (source == TEAM.TeamMax)
	//		return;
	//	teamArray [(int)source].AddSkillCD (cd);
	//}

	/// <summary>
	/// Checks the skill C.
	/// </summary>
	/// <returns>The skill C.</returns>
	/// <param name="t">T.</param>
	/// <param name="skillID">Skill I.</param>
	public float CheckSkillCD(TEAM t,int skillID)
	{
		if (t == TEAM.TeamMax)
			return 0f;
		
		return teamArray [(int)t].CheckSkillCD(skillID);
	}

    public void SetAllTeamMoveSpeed(float speedPercent = 1.0f)
    {
        //for (int i = 0; i < teamArray.Length; i++)
        //{
        //    if (teamArray[i].GetAttribute(TeamAttr.Speed).addPercent != speedPercent)
        //    {
        //        teamArray[i].SetAttribute(TeamAttr.Speed, speedPercent, false);
        //    }

        //}
    }
    public void SetAllTeamProduceSpeed(float speedPercent = 1.0f)
    {
        //for (int i = 0; i < teamArray.Length; i++)
        //{
        //    if (teamArray[i].GetAttribute(TeamAttr.ProduceSpeed).addPercent != speedPercent)
        //    {
        //        teamArray[i].SetAttribute(TeamAttr.ProduceSpeed, speedPercent, false);
        //    }
        //}
    }
    public void SetAllOccupySpeed(float speedPercent = 1.0f)
    {
        //for (int i = 0; i < teamArray.Length; i++)
        //{
        //    if (teamArray[i].GetAttribute(TeamAttr.CapturedSpeed).addPercent != speedPercent)
        //    {
        //        teamArray[i].SetAttribute(TeamAttr.CapturedSpeed, speedPercent, false);
        //        teamArray[i].SetAttribute(TeamAttr.OccupiedSpeed, speedPercent, false);
        //    }
        //}
    }

}
