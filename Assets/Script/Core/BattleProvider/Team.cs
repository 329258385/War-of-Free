using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Solarmax;

public class Team 
{
	/// <summary>
	/// team管理器 
	/// </summary>
	/// <value>The manager.</value>
	public TeamManager teamManager { get; set; }

	/// <summary>
	/// 队伍的玩家数据
	/// </summary>
	/// <value>The player data.</value>
	public PlayerData playerData { get; set; }

	/// <summary>
	/// 队伍对应的ai数据
	/// </summary>
	/// <value>The ai data.</value>
	//public AIData aiData { get; set; }

	/// <summary>
	/// ai是否启动
	/// </summary>
	public bool aiEnable = false;

	/// <summary>
	/// 当前队伍颜色
	/// </summary>
	public Color color { get; set; }


	public string iconname { get; set; }

	/// <summary>
	/// 当前队伍
	/// </summary>
	/// <value>The team.</value>
	public TEAM team { get; set;}

	/// <summary>
	/// 血量上限
	/// </summary>
	/// <value>The hp max.</value>
	//public float hpMax { get {return GetAttributeFloat (TeamAttr.HpMax); } }

	/// <summary>
	/// 攻击力
	/// </summary>
	/// <value>The attack.</value>
	//public float attack { get {return GetAttributeFloat(TeamAttr.Attack); } }

	/// <summary>
	/// 移动速度
	/// </summary>
	//public float speed {
	//	get { return GetAttributeFloat (TeamAttr.Speed); }
	//}

	/// <summary>
	/// 当前飞船数量
	/// </summary>
	/// <value>The current.</value>
	public int current {
		get { return 1/*GetAttributeInt (TeamAttr.Population)*/; }
	}

	public bool isDead
	{
		get
		{
			if (current <= 0 && currentMax < 0) {
				Debug.LogErrorFormat ("捕获到最大人口数为负值, contact fangjun, Team:{0}, id:{1}, currentMax:{2}", this.team, playerData.userId, currentMax);
			}
				
			return current <= 0 && currentMax <= 0; 
		}
	}
	/// <summary>
	/// 飞船上限
	/// </summary>
	public int currentMax {
		get { return 1/*GetAttributeInt (TeamAttr.PopulationMax)*/; }
	}

    /// <summary>
    /// 自己销毁的飞船数量
    /// </summary>
    /// <value>The destory.</value>
    public int      destory = 0;

    /// <summary>
    /// 自己击杀的飞船数量 
    /// </summary>
    public int      hitships = 0;

    /// <summary>
    /// 生产数量
    /// </summary>
    public int      produces = 0;

	/// <summary>
	/// 组队ID
	/// </summary>
	/// <value>The destory.</value>
	public int groupID { get; set; }

	/// <summary>
	/// 结果队伍积分
	/// </summary>
	/// <value>The score mod.</value>
	public int scoreMod { get; set; }

	/// <summary>
	/// 结果序列，从-3， -2， -1， 0。默认为0，所以排序按照resultorder，scoremode，destroy依次排序
	/// </summary>
	/// <value>The result order.</value>
	public int resultOrder { get; set; }

	/// <summary>
	/// 结果排名
	/// </summary>
	public int resultRank { get; set; }
	/// <summary>
	/// 结果类型
	/// </summary>
	public NetMessage.EndType resultEndtype { get; set; }

	/// <summary>
	/// 联赛mvp
	/// </summary>
	/// <value>The league mvp.</value>
	public int leagueMvp { get; set; }

	/// <summary>
	/// 是否已经死亡了，(单机标记投降和死亡都是End，pvp死亡才是End）
	/// </summary>
	public bool isEnd { get; set; }

	///// <summary>
	///// 队伍cd
	///// </summary>
	///// <value>The score mod.</value>
	//List<SkillCoolDown> cdList = new List<SkillCoolDown>();

	///// <summary>
	///// 隐藏人口
	///// </summary>
	//public bool hidePopution { get { return GetAttributeInt(TeamAttr.HidePop) > 0; } }

	///// <summary>
	///// 飞行隐身
	///// </summary>
	//public bool hideFly     { get { return GetAttributeInt(TeamAttr.HideFly) > 0; } }

	///// <summary>
	///// 技能buff逻辑
	///// </summary>
	//public NewSkillBuffLogic skillBuffLogic;

	///// <summary>
	///// 队伍属性
	///// </summary>
	//private AttributeObject[] attribute = new AttributeObject[(int)TeamAttr.MAX];

	public bool attributeChanged = false;

	/// <summary>
	/// 失去所有建筑时候的时间点
	/// </summary>
	private float m_loseAllMatrixTime = 0.0f;

    /// <summary>
    /// 队伍基本属性的静态配置
    /// </summary>
    private CampConfig      mCampConfig;


	/// <summary>
	/// 初始化
	/// </summary>
	public Team()
	{
		
	}

    public Team( CampConfig cfg )
    {
        mCampConfig             = cfg;
        playerData              = new PlayerData();
        playerData.Reset();
        playerData.currentTeam  = this;
        Clear();
    }


	/// <summary>
	/// 清理数据
	/// </summary>
	public void Clear()
	{
//		uid = -1;
		playerData.Reset ();
		destory         = 0;
        hitships        = 0;
        produces        = 0;
        scoreMod        = 0;
		resultOrder     = 0;
		resultRank      = -1;
		resultEndtype   = NetMessage.EndType.ET_Dead;
		leagueMvp       = 0;
		isEnd           = false;
		groupID         = -1;
		aiEnable        = false;
  //      cdList.Clear ();

		//skillBuffLogic.Destroy ();
		//for (int i = 0; i < attribute.Length; ++i) {
		//	attribute [i].Reset ();
		//}
  //      // 初始化属性
  //      SetAttributeBase(TeamAttr.Speed, mCampConfig.speed);
  //      SetAttributeBase(TeamAttr.Attack, mCampConfig.attack);  // 修改为25.采用时间来计算。即 25Damage／s
  //      SetAttributeBase(TeamAttr.HpMax, mCampConfig.maxhp);
  //      SetAttributeBase(TeamAttr.CapturedSpeed, mCampConfig.capturedspeed);
  //      SetAttributeBase(TeamAttr.OccupiedSpeed, mCampConfig.occupiedspeed);
  //      SetAttributeBase(TeamAttr.ProduceSpeed, mCampConfig.producespeed);
  //      SetAttributeBase(TeamAttr.BeCapturedSpeed, mCampConfig.becapturedspeed);


  //      SetAttributeBase (TeamAttr.Speed, 1.2f);
		//SetAttributeBase (TeamAttr.Attack, 10);	// 修改为25.采用时间来计算。即 25Damage／s
		//SetAttributeBase (TeamAttr.HpMax, 100);
		//SetAttributeBase (TeamAttr.CapturedSpeed, 1);
		//SetAttributeBase (TeamAttr.OccupiedSpeed, 1);
		//SetAttributeBase (TeamAttr.ProduceSpeed, 1);
		//SetAttributeBase (TeamAttr.BeCapturedSpeed, 1);
	}

	public void StartTeam()
	{
        playerData.skillPower = 0;
		//cdList.Clear ();
	}

	public bool IsFriend(int group)
	{
		if (groupID == -1)
			return false;
		if (group == -1)
			return false;
		if (groupID != group)
			return false;
		return true;
	}

	/// <summary>
	/// 浅copy
	/// </summary>
	public object Clone()
	{
		return this.MemberwiseClone();
	}

	/// <summary>
	/// 判断当前队伍是否有效，是否为有数值的队伍
	/// </summary>
	public bool Valid()
	{
		//return playerData.userId != -1 && team != TEAM.Team_Black1 && team != TEAM.Team_Black2;
		return playerData.userId != -1;
	}

	/// <summary>
	/// Updates the event.
	/// </summary>
	/// <param name="frame">Frame.</param>
	/// <param name="dt">Dt.</param>
	public void UpdateEvent(int frame, float dt)
	{
		// 技能cd时间
		//for (int i = 0; i < cdList.Count;) 
		//{
		//	cdList [i].cd -= dt;
		//	if (cdList [i].cd <= 0)
		//		cdList.RemoveAt (i);
		//	else
		//		i ++;
		//}

		checkLoseAllMatrix (dt);

		//skillBuffLogic.Tick (frame, dt);
	}

	/// <summary>
	/// Adds the skill C.
	/// </summary>
	/// <param name="cd">Cd.</param>
	//public void AddSkillCD(SkillCoolDown cd)
	//{
	//	cdList.Add (cd);
	//}

	/// <summary>
	/// Checks the skill C.
	/// </summary>
	/// <returns>The skill C.</returns>
	/// <param name="skillID">Skill I.</param>
	public float CheckSkillCD(int skillID)
	{
		//for (int i = 0; i < cdList.Count; i++) 
		//{
		//	if (cdList [i].skillID == skillID)
		//		return cdList [i].cd;
		//}

		return 0f;
	}

	/// <summary>
	/// 判断队伍是否死亡
	/// 条件：当前人口为0，当前无拥有星球
	/// </summary>
	public bool CheckDead()
	{
		do {
			if (current > 0)
				break;

			//if (teamManager.sceneManager.nodeManager.CheckHaveNode((int)team))
			//	break;

			return true;
		} while(false);

		return false;
	}


    /// <summary>
    /// 判断队伍是否死亡
    /// 条件：如果是玩家，则当前人口为0且当前无产兵建筑
    /// </summary>
    public bool CheckPVEDead()
    {
        do
        {
            if (current > 0)
                break;
            //if(playerData.userId >0)
            //{
            //    if (teamManager.sceneManager.nodeManager.CheckHaveProduceNode((int)team))
            //        break;
            //}
            //else
            //{
            //    if (teamManager.sceneManager.nodeManager.CheckHaveNode((int)team))
            //        break;
            //}

            return true;
        } while (false);

        return false;
    }

    /// <summary>
    /// 设置基础属性值
    /// </summary>
 //   public void SetAttributeBase (TeamAttr attr, float num)
	//{
 //       AttributeObject entry = attribute [(int)attr];
	//	entry.baseNum = num;
	//	entry.Calculate ();
	//}

	/// <summary>
	/// 设置属性值，absolute为绝对值
	/// </summary>
	//public void SetAttribute (TeamAttr attr, float num, bool absolute)
	//{
 //       AttributeObject entry = attribute [(int)attr];

	//	if (absolute) {
	//		entry.addNum += num;
	//	} else {
	//		entry.addPercent += num;
	//	}

	//	// 隐身的话，设置隐身就是为1，取消就是0
	//	if (attr == TeamAttr.HideFly || attr == TeamAttr.HidePop || attr == TeamAttr.StopBeCapture || attr == TeamAttr.QuickMove) {
	//		entry.addNum = num > 0 ? 1 : 0;
	//	}

	//	entry.Calculate ();
	//}

	/// <summary>
	/// 获取属性
	/// </summary>
	//public float GetAttributeFloat (TeamAttr attr)
	//{
	//	return attribute [(int)attr].fixedNum;
	//}

	//public int GetAttributeInt (TeamAttr attr)
	//{
	//	return Convert.ToInt32 (GetAttributeFloat (attr));
	//}

	//public float GetAttributeBaseFloat (TeamAttr attr)
	//{
	//	return attribute [(int)attr].baseNum;
	//}
	//public int GetAttributeBaseInt (TeamAttr attr)
	//{
	//	return Convert.ToInt32 (GetAttributeBaseFloat (attr));
	//}

	/// <summary>
	/// 获取属性增益和减益状态
	/// </summary>
	//public int GetAttributeAD (TeamAttr attr)
	//{
 //       AttributeObject entry = attribute [(int)attr];

	//	return entry.fixedNum.CompareTo (entry.baseNum);
	//}

 //   /// <summary>
 //   /// 获取属性增益和减益状态
 //   /// </summary>
 //   public AttributeObject GetAttribute(TeamAttr attr)
 //   {
 //       return attribute[(int)attr];
 //   }

    /// <summary>
    /// 累积失去所有母体的时间点
    /// </summary>
    /// <param name="frame">Frame.</param>
    /// <param name="dt">Dt.</param>
    private void checkLoseAllMatrix(float dt)
	{
        //if (teamManager.sceneManager.nodeManager.CheckHaveNode((int)team))
        //{
        //    m_loseAllMatrixTime = 0.0f;
        //}
        //else
        //{
        //    m_loseAllMatrixTime += dt;
        //}
    }

	public float GetLoseAllMatrixTime(float dt = 0.0f)
	{
		//if (teamManager.sceneManager.nodeManager.CheckHaveNode((int)team))
		//{
		//    m_loseAllMatrixTime = 0.0f;
		//    return 0.0f;
		//}
		//m_loseAllMatrixTime += dt;
		//return m_loseAllMatrixTime;
		return 0f;
	}
}
