using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Plugin;
using Solarmax;




/// <summary>
/// 场景管理
/// </summary>
public class SceneManager : Lifecycle2
{
#if ROBOT

	public int BEGIN_ID = 1000000;

	public BattleSystem battleSystem;
#endif

	
	/// <summary>
	/// 队伍管理
	/// </summary>
	/// <value>The team mnager.</value>
	public TeamManager teamManager { get; set; }


	/// <summary>
	/// 地图信息
	/// </summary>
	public Dictionary<string, string> MapInfo = new Dictionary<string, string>();

	// 战斗持续时间
	float m_BattleTime = 0f;


	public int curProcduceIndex = 0;
	/// <summary>
	/// 战斗节凑调整系数
	/// </summary>
	public float battleScaleSpeed = 1;

	/// <summary>
	/// 战斗数据
	/// </summary>
	/// <value>The battle manager.</value>
	public BattleData battleData { get; set; }

	/// <summary>
	/// 初始化
	/// </summary>
	public SceneManager(BattleData bd)
	{
		//战斗数据引用
		battleData = bd;

		//组队管理
		teamManager = new TeamManager(this);
	}

	public bool Init()
	{
		do {
			if (!teamManager.Init())
				break;
			return true;
		} while (false);

		return false;
	}

	public void Tick(int frame, float interval)
	{
		teamManager.Tick(frame, interval);
		m_BattleTime += interval;
	}

	public void Destroy()
	{
		teamManager.Destroy();


		m_BattleTime = 0f;
		curProcduceIndex = 0;
	}


	/// <summary>
	/// 释放
	/// </summary>
	public void Release()
	{


	}

	/// <summary>
	/// 重置
	/// </summary>
	public void Reset()
	{
		// 设置战斗总时间
		m_BattleTime = 0f;
	}

	public bool GetBattleRate()
	{
		return (m_BattleTime >= 180f);
	}

	public float GetBattleTime()
	{
		return m_BattleTime;
	}

	/// <summary>
	/// 执行帧包
	/// </summary>
	/// <param name="node">Node.</param>
	public void RunFramePacket(FrameNode node)
	{
		if (node.msgList == null || node.msgList.Length == 0)
			return;
		for (int i = 0; i < node.msgList.Length; i++) {
			ExcutePacket(node.msgList[i] as Packet);
		}
	}

	void ExcutePacket(Packet packet)
	{
		if (packet.packet.type == 0) {
            //if (packet.packet.move.team == TEAM.Neutral)
            //    nodeManager.MoveTo(packet.packet.move, packet.team);
            //else
            //    nodeManager.MoveTo(packet.packet.move, packet.packet.move.team);
        }
		else if (packet.packet.type == 1) {
			// 释放技能
			//newSkillManager.OnCast(packet.packet.skill);
		}
		else if (packet.packet.type == 2) {
			// 投降包
			BattleSystem.Instance.OnPlayerGiveUp(packet.packet.giveup.team);
            BattleSystem.Instance.sceneManager.DestroyTeam(packet.packet.giveup.team, TEAM.Neutral);
        }
        else if (packet.packet.type == 3)
        {
            // 加速
            BattleSystem.Instance.sceneManager.teamManager.SetAllTeamMoveSpeed();
            BattleSystem.Instance.sceneManager.teamManager.SetAllTeamProduceSpeed();
            BattleSystem.Instance.sceneManager.teamManager.SetAllOccupySpeed();
        }
        else if (packet.packet.type == 4)
        {
            // 星球毁灭
            
        }
        //播放指定特效
        else if (packet.packet.type == 6)
        {
           
        }
        else if (packet.packet.type == 7)
        {
            
        }
        else if (packet.packet.type == 8)
        {
           
        }
        else if (packet.packet.type == 9)
        {
            
        }
        else if (packet.packet.type == 10)
        {
           
        }
        else if (packet.packet.type == 11)
        {
            
        }
        else if (packet.packet.type == 12)
        {
            
        }
        //清除场景内所有障碍节点及特效
        else if (packet.packet.type == 13)
        {
            //BattleSystem.Instance.sceneManager.nodeManager.RemoveALLBarriers();
        }
        //节点释放技能，无特效
        else if (packet.packet.type == 14)
        {
           
        }
        //节点释放问号星球技能，变成别的建筑
        else if (packet.packet.type == 15)
        {
            
        } 
        else if (packet.packet.type == 16) {
            
        }
    }


    /// <summary>
    /// 创建场景
    /// </summary>
    public void CreateScene(IList<int> usr ,bool isEditer = false, bool isHaveRandom = false )
	{
		MapConfig table = MapConfigProvider.Instance.GetData (battleData.matchId);

		if (table == null) {
			Debug.LogErrorFormat ("CreateScene-Load table is error!!! {0}", battleData.matchId);

			Release ();

#if !SERVER
			UISystem.Get ().HideAllWindow ();
			UISystem.Get ().ShowWindow ("LogoWindow");
#endif

			return;
		}

		CreateScene (isEditer, table, usr, isHaveRandom);
	}

	public void CreateScene (bool isEditer, MapConfig table, IList<int> usr = null, bool isHaveRandom = false )
	{
		battleData.currentTable = table;

		//创建星球
		CreateNode (table.mbcList);

		//创建障碍物
		CreateLines(table.mlcList);

        //创建动态阻挡
        CreateBarrierPoints(table.mbcList);

        //创建飞船
		CreateShip (table.mpcList);

        #if !SERVER
		//播放音乐
		if(!isEditer)
		{
			AudioManger.Get().PlayAudioBG("Wandering");
		}
        #endif
	}

	/// <summary>
	/// 创建障碍物
	/// </summary>
	public void CreateLines(List < MapLineConfig > lines)
	{
		//if (lines == null)
		//	return;

		//foreach(MapLineConfig node in lines)
		//{
  //          AddBarrierLines(node.point1, node.point2);
  //      }
	}

    void CreateBarrierPoints(List<MapBuildingConfig> building)//(List<MapBuildingConfig> building)
    {
        if (building == null)
            return;

        //foreach (MapBuildingConfig item in building)
        //{
        //    if (item.type == "BarrierPoint")
        //    {
        //        nodeManager.dynamicBarriers.Add(item);
        //    }
        //}
        //if (nodeManager.dynamicBarriers.Count < 2)
        //{
        //    nodeManager.dynamicBarriers.Clear();
        //    return;
        //}

        //for (int i = 0; i < nodeManager.dynamicBarriers.Count; i++)
        //{
        //    MapBuildingConfig point1 = nodeManager.dynamicBarriers[i];
        //    for (int j = i + 1; j < nodeManager.dynamicBarriers.Count; j++)
        //    {
        //        MapBuildingConfig point2 = nodeManager.dynamicBarriers[j];

        //        MapPlayerConfig pc1 = BattleSystem.Instance.battleData.currentTable.mpcList.Find(n => n.tag == point1.tag);
        //        MapPlayerConfig pc2 = BattleSystem.Instance.battleData.currentTable.mpcList.Find(n => n.tag == point2.tag);
        //        bool b1, b2, show;
        //        b1 = b2 = show = false;
        //        if (pc1 != null && pc1.ship > 0) b1 = true;
        //        if (pc2 != null && pc2.ship > 0) b2 = true;

        //        Vector3 p1, p2;
        //        p1 = p2 = Vector3.zero;
        //        p1.x = point1.x;
        //        p1.y = point1.y;
        //        p2.x = point2.x;
        //        p2.y = point2.y;
        //        float dist = Vector3.Distance(p1, p2);

        //        if (b1 == b2 && point1.fbpRange >= dist && point2.fbpRange >= dist)
        //            show = true;
        //        nodeManager.AddDynamicBarrierLines(point1.tag, point2.tag, show);
        //    }
        //}
    }

    /// <summary>
    /// 创建飞船
    /// </summary>
    /// <param name="players">Players.</param>
    void CreateShip(List < MapPlayerConfig > players)
	{
		if (players == null)
			return;

		//foreach(MapPlayerConfig item in players){
		//	if (null != item) 
		//	{
		//		// 增加初始数量上限，其实就是增加初始人口数
		//		//Team t = teamManager.GetTeam((TEAM)item.camption);
		//		int ship = item.ship;

		//		AddShip (
		//			item.tag,
		//			ship,
		//			item.camption,
		//			true
		//		);
		//	}
		//}
	}

    /// <summary>
    /// 创建星球
    /// </summary>
    void CreateNode(List < MapBuildingConfig > building)
	{
		if (building == null)
			return;

		//foreach (MapBuildingConfig item in building) {
		//	MapNodeConfig node = MapNodeConfigProvider.Instance.GetData (item.type, item.size);

		//	if (node == null)
		//		return;
  //          if (string.IsNullOrEmpty(item.aistrategy))
  //              item.aistrategy = "-1";

  //          var createNode = AddNode(
  //              node.id,
  //              item.camption, 	//阵营
		//		node.typeEnum, 	//星球类型
  //              node.aiWeight,
  //              node.aiUnitLost,
		//		item.x, 		//坐标x
		//		item.y, 		//坐标y
		//		node.size,		//缩放
		//		item.tag,		//星球标示
		//		node.createshipnum,	//生产数量
		//		node.createship,	//生产速度
		//		node.hp,			//血量上限
		//		node.food,		//人口
		//		node.attackrange,	//攻击范围
		//		node.attackspeed,	//攻击频率
		//		node.attackpower,	// 攻击力
  //              0f,
		//		item.orbit,		//公转类型
		//		item.orbitParam1,	//公转目标p1
		//		item.orbitParam2,	//公转目标p2
		//		item.orbitClockWise, //顺时针环绕
  //              node.nodesize,
  //              node.perfab,
  //              node.skills,
  //              item.fAngle,
  //              int.Parse(item.aistrategy),
  //              item.lasergunAngle,
  //              item.lasergunRange
		//	);

  //          //创建占领指定星球，获得胜利的标志
  //          if (createNode != null &&
  //              BattleSystem.Instance.battleData.winType == "occupy" &&
  //              (BattleSystem.Instance.battleData.winTypeParam1 == createNode.tag ||
  //              BattleSystem.Instance.battleData.winTypeParam1 == createNode.tag) ) {
  //              Object res = AssetManager.Get().GetResources("Effect_Aims");
  //              var go = GameObject.Instantiate(res) as GameObject;
  //              go.transform.SetParent(createNode.entity.GetGO().transform);
  //              var scale = (0.4f / node.size);
  //              go.transform.localScale = new Vector3(0.03f * scale, 0.03f * scale, 0.03f * scale);
  //              go.transform.localPosition = new Vector3(-1.7f * scale, 1.3f * scale, 0);
  //          }
  //      }
	}

	
	/// <summary>
	/// 获取类型
	/// </summary>
	/// <returns>The type.</returns>
	/// <param name="kind">Kind.</param>
	public string GetType(int kind)
	{
        string[] array = { string.Empty, "star", "castle", "teleport", "tower", "barrier", "barrierline", "master", "defense", "power", "BlackHole", "House", "Arsenal", "AircraftCarrier", 
                             "Lasercannon", "Attackship", "Lifeship", "Speedship", "Captureship", "AntiAttackship" , "AntiLifeship", "AntiSpeedship", "AntiCaptureship", "Magicstar",
                             "Hiddenstar", "FixedWarpDoor", "Clouds", "Inhibit", "Twist", "AddTower", "Sacrifice", "OverKill", "CloneTo", "Treasure", "UnknownStar", "Lasergun",
                             "Mirror", "BarrierPoint", "Gunturret"};

		return array[kind];
	}

	/// <summary>
	/// 删除星球
	/// </summary>
	/// <param name="tag">Tag.</param>
	public void RemoveNode(string tag)
	{
		
	}

	/// <summary>
	/// 给星球添加飞船
	/// </summary>
	/// <returns>The ship.</returns>
	/// <param name="node">Node.</param>
	/// <param name="count">Count.</param>
	/// <param name="team">Team.</param>
	/// <param name="noAnim">No animation.</param>
	//public void AddShip(Node node, int count, int team, bool noAnim=true ,bool normal = true)
	//{
	//	if (node == null)
	//		return;

	//	node.AddShip (team, count, noAnim, normal);
	//}

	/// <summary>
	/// 给星球添加飞船
	/// </summary>
	/// <param name="tag">Tag.</param>
	/// <param name="count">Count.</param>
	/// <param name="team">Team.</param>
	/// <param name="noAnim">If set to <c>true</c> no animation.</param>
	//public void AddShip(string tag, int count, int team, bool noAnim=true)
	//{
	//	Node node = nodeManager.GetNode (tag);

	//	if (node == null)
	//		return;

	//	AddShip (node, count, team, noAnim);
	//}

	

	/// <summary>
	/// 获取地图信息
	/// </summary>
	/// <returns>The map info.</returns>
	public Dictionary<string, string> GetMapInfo()
	{
		return MapInfo;
	}
		
	/// <summary>
	/// 添加生产公式参数
	/// </summary>
	/// <returns>The map info.</returns>
	public void AddProduce(float startTime, float rateProduce)
	{
		//ShipProduce sp = new ShipProduce ();
		//sp.startTime = startTime;
		//sp.produceRate = rateProduce;
		//produceRates.Add (sp);
		//BattleManager.Log ("Ship Produce Time-Rate:[{0},{1}) - {2}",minSec,maxSec,rateProduce);
	}

    public float GetbattleScaleSpeed()
	{
        return battleScaleSpeed;
	}

	public void DestroyTeam(TEAM team, TEAM win )
	{
		//List<Node> allNodes = nodeManager.GetUsefulNodeList();
		//for (int i = 0; i < allNodes.Count; ++i) 
		//{
		//	Node node = allNodes [i];
		//	if (node.team == team) 
		//	{
  //              node.BombShip (team, win, 1.0f);
		//		node.currentTeam = teamManager.GetTeam (TEAM.Neutral);
  //              if (node.capturingTeam != TEAM.Neutral && node.capturingTeam != team)
  //              {
  //                  //别的队伍正在捕获这个星球，立即捕获成功
  //                  node.hp = 0;
  //              }
  //              if (node.capturingTeam == TEAM.Neutral && node.occupiedTeam == TEAM.Neutral)
  //              {
  //                  //星球上没有别的队伍
  //                  node.hp = 0;
  //              }
  //              //特效
  //              EffectManager.Get ().AddHalo (node, node.currentTeam.color);
		//		// 音效
		//		AudioManger.Get ().PlayCapture (node.GetPosition ());
		//	} 
		//	else if (node.GetShipCount ((int)team) > 0)
		//	{
		//		node.BombShip (team, win, 1.0f);
		//	}
		//}

		//List<Ship> ships = shipManager.GetFlyShip ((TEAM)team);
		//for (int j = ships.Count - 1; j >= 0;) 
		//{
		//	if (ships [j].num == 1) 
		//	{
		//		ships [j].Bomb ();
		//		j--;
		//	}
		//	else
		//		ships [j].Bomb ();
		//}
	}

	/// <summary>
	/// 销毁队伍所有临时飞机
	/// </summary>
	/// <param name="team">Team.</param>
	public void DestroyTeamTemporary(TEAM team)
	{
		//List<Node> allNodes = nodeManager.GetUsefulNodeList();
		//for (int i = 0; i < allNodes.Count; ++i) 
		//{
		//	Node node = allNodes [i];
		//	int tempNum = node.GetTempShipCount ((int)team);
		//	if (node.GetTempShipCount ((int)team) > 0)
		//	{
		//		node.BombTempShipNum (team, tempNum);
		//	}
		//}

		//List<Ship> ships = shipManager.GetFlyShip ((TEAM)team);
		//for (int j = ships.Count - 1; j >= 0;) 
		//{
		//	if (ships [j].temNum == 0) 
		//	{
		//		j--;
		//	}
		//	else
		//		ships [j].Bomb ();
		//}
	}

	/// <summary>
	/// 管理星球的淡入淡出
	/// </summary>
	/// <param name="fadeIn">Format.</param>
	/// /// <param name="duration">Format.</param>
	public void FadePlanet(bool fadeIn, float duration)
	{
		//List<Node> usefulNodes = nodeManager.GetUsefulNodeList();
		//for (int i = 0; i < usefulNodes.Count; ++i) {
		//	var node = usefulNodes [i];
		//	if (node.entity != null) {
		//		node.entity.FadeEntity (fadeIn, duration);
		//	}
		//}
		//List<Node> barrierNodes = nodeManager.GetBarrierNodeList ();
		//for (int i = 0; i < barrierNodes.Count; ++i) {
		//	var node = barrierNodes [i];
		//	if (node.entity != null) {
		//		node.entity.FadeEntity (fadeIn, duration);
		//	}
		//}
	}

	/// <summary>
	/// 胜利效果
	/// </summary>
	public void ShowWinEffect(Team win, Color color)
	{
#if !SERVER
		//Node node;
		//List<Node> list = nodeManager.GetUsefulNodeList ();
		//for (int i = 0; i < list.Count; ++i)
		//{
		//	node = list [i];
		//	if(node.type != NodeType.Barrier && node.type != NodeType.BarrierLine)
		//	{
		//		EffectManager.Get ().AddHalo (node, color);
		//		node.entity.SetColor (color);
		//		node.entity.SetAlpha (1.0f);
		//	}

		//	// 销毁掉其他的所有飞船
		//	for (int j = 0; j < (int)TEAM.TeamMax; ++j)
		//	{
		//		TEAM t = (TEAM)j;
		//		Team team = teamManager.GetTeam (t);

		//		if (win == team)
		//			continue;
		//		if (win.IsFriend (team.groupID))
		//			continue;

		//		node.BombShip (t, win.team, 1.0f);
		//	}
		//}
#endif
	}

	public void SilentMode (bool status)
	{
		LoggerSystem.Instance.Info (string.Format ("========SilentMode : {0},, frame:{1}", status, BattleSystem.Instance.GetCurrentFrame()));


		battleData.silent = status;

		//nodeManager.SilentMode (status);
		//shipManager.SilentMode (status);
	}
}
