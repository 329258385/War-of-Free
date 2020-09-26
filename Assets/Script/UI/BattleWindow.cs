using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Solarmax;
using NetMessage;


public class BattleWindow : BaseWindow
{
	/// <summary>
	/// The background.
	/// </summary>
	public UITexture background = null;
    
	/// <summary>
	/// 飞船数量Label
	/// </summary>
	public UILabel populationLabel = null;
	public UILabel populationValueLabel = null;

	/// <summary>
	/// 投降字
	/// </summary>
	private float giveupCountTime = 0;
	private bool giveupForQuit = false;
	public UILabel giveupLabel = null;

	/// <summary>
	/// 进度条，滑块图，百分比字
	/// </summary>
    public GameObject ProcessAram = null;
	public UISprite percentleft = null;
	public UISprite percentright = null;
	public UISprite percetPic = null;
	public UILabel percentLabel = null;


	/// <summary>
	/// 触摸精度
	/// </summary>
	public float sensitive = 1;

	/// <summary>
	/// 新的分兵条，长按
	/// </summary>
	public GameObject newProgressGo = null;
	public UISprite newProgressPic = null;
	public UILabel newProgressLabel = null;
	private int newProgressCount = 0;
	private string newProgressTag = string.Empty;

	/// <summary>
	/// 进度条，滑块图宽
	/// </summary>
	private float lineTotalLength = 0;
	/// <summary>
	/// 进度条，滑块图起始移动位置
	/// </summary>
	private Vector3 percentZeroPos = Vector3.zero;

	/// <summary>
	/// 当前的百分比
	/// </summary>
	private float percent = 0;

	private float percentPicWidth = 0;

	private int reconnectCount;
	private int checkConnectionCount;

    public GameObject       player;
    public GameObject       player2V2;

    private GameObject[]    playerFrames = new GameObject[4];
    public GameObject[]     players;
    public GameObject[]     pvps;
    public UILabel          battleTime;
    public UILabel          battleCoundown;



	public UILabel popLable1;
	public UILabel popLableValue1;
	public UILabel popLableAdd;



    public Animator clockAni;
    public string clockAniStr;

    private int battleTimeMax = 0; // PVP最长时间
    private bool bProceesEnd = false;
    private int curAddSpeedSegment = 0;

    /// <summary>
    /// 队伍到空间下表映射关系维护
    /// </summary>
    Dictionary<int, int> mapTeam2Index = new Dictionary<int, int>();
	void Start()
	{
        battleCoundown.transform.parent.gameObject.SetActive(false);
		percent = 1;
		SetPercent ();
        bProceesEnd = false;
		// 设置drag参数的像素修正
		UIRoot nguiRoot = UISystem.Get ().GetNGUIRoot ();
		sensitive *= nguiRoot.pixelSizeAdjustment;

        curAddSpeedSegment = BattleSystem.Instance.sceneManager.curProcduceIndex;
		battleTimeMax = int.Parse(GameVariableConfigProvider.Instance.GetData (4)) * 60;
	}

	public override bool Init ()
	{
        base.Init();
		RegisterEvent (EventId.NoticeSelfTeam);
		RegisterEvent (EventId.OnBattleDisconnect);
		RegisterEvent (EventId.RequestUserResult);
		RegisterEvent (EventId.OnTouchBegin);
		RegisterEvent (EventId.OnTouchPause);
		RegisterEvent (EventId.OnTouchEnd);
		RegisterEvent (EventId.OnSelfDied);
		RegisterEvent (EventId.OnPopulationUp);
		RegisterEvent (EventId.OnPopulationDown);

        RegisterEvent (EventId.PlayerGiveUp);
		RegisterEvent (EventId.PlayerDead);

        RegisterEvent(EventId.OnPVPBattleAccelerate);
        RegisterEvent(EventId.OnPVPBattleBoom);
        return true;
	}

	/// <summary>
	/// 显示当前飞船数量
	/// </summary>
	public override void OnShow()
	{
        base.OnShow();
        percentPicWidth = percetPic.width;
		
		giveupForQuit = false;
		giveupLabel.text = LanguageDataProvider.GetValue( 418 );
		string giveupconfig = GameVariableConfigProvider.Instance.GetData (2);
		giveupCountTime = int.Parse (giveupconfig);

		// 设置分兵方式
		InitFightProgressOption ();

		Team team = BattleSystem.Instance.sceneManager.teamManager.GetTeam (BattleSystem.Instance.battleData.currentTeam);
		Color color = team.color;
		color.a = 0.7f;

		// 人口颜色
		populationLabel.color       = color;
		populationValueLabel.color  = color;

		populationValueLabel.text   = string.Format ("{0}/{1}", team.current, team.currentMax);

        ShowModeUI();
        ShowPlayerInfo();
		TimerProc ();
		UISystem.Get ().ShowWindow ("PopTextWindow");
        ProcessAram.GetComponent<UIEventListener>().onClick += OnSelectProcessAram;
        clockAniStr = "Battle_battleframe_clock1";
    }
    //public GameObject player;
    //public GameObject player2V2;

    void ShowModeUI()
    {
        player.SetActive(false);
        player2V2.SetActive(false);
        for ( int i = 0;  i < playerFrames.Length; i++ )
        {
            playerFrames[i] = null;
        }

        if(BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_2V2 )
        {
            int nLength = pvps.Length;
            for( int n = 0; n < nLength; n++ )
            {
                playerFrames[n] = pvps[n];
            }
            player2V2.SetActive(true);
        }
        else
        {
            int nLength = players.Length;
            for (int n = 0; n < nLength; n++)
            {
                playerFrames[n] = players[n];
            }
            player.SetActive(true);
        }
    }

    void ShowPlayerInfo()
    {
        for( int n = 0; n < playerFrames.Length; n++ )
        {
            if(playerFrames[n] != null )
                playerFrames[n].SetActive(false);
        }

        string mapId  = LocalPlayer.Get().battleMap;
        MapConfig map = MapConfigProvider.Instance.GetData(mapId);
        if( map == null )
        {
            return;
        }

        List<Team> ls = new List<Team>();
        if (BattleSystem.Instance.battleData.battleSubType == CooperationType.CT_2V2)
        {
            int indx = 0;
            for (int i = 1; i < map.player_count + 1; ++i)
            {
                Team teamTmp = BattleSystem.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
                if (teamTmp.playerData.userId == -1)
                    continue;

                if (indx >= 4)
                    break;

                ls.Add(teamTmp);
                indx++;
            }
            ls.Sort((a, b) => (a.groupID.CompareTo(b.groupID)));
        }
        else
        {
            int indx = 0;
            Team team = BattleSystem.Instance.sceneManager.teamManager.GetTeam(BattleSystem.Instance.battleData.currentTeam);
            for (int i = 1; i < map.player_count + 1; ++i)
            {
                Team teamTmp = BattleSystem.Instance.sceneManager.teamManager.GetTeam((TEAM)i);
                if (teamTmp.team == team.team)
                    continue;

                if (teamTmp.playerData.userId == -1)
                    continue;

                if (indx >= 3)
                    break;

                ls.Add(teamTmp);
                indx++;
            }
        }

        mapTeam2Index.Clear();
        for (int i = 0; i < ls.Count; ++i)
        {
            Team teamTmp = ls[i];
            playerFrames[i].SetActive(true);
            SetPlayerInfo(playerFrames[i], teamTmp);
            mapTeam2Index.Add((int)teamTmp.team, i);
        }
    }

    private void SetPlayerInfo( GameObject go, Team teamTmp )
    {
        Color col = teamTmp.color;
        col.a     = 1;
        UISprite playerBG   = go.GetComponent<UISprite>();
        UISprite playerIcon = go.transform.Find("icon").GetComponent<UISprite>();
        UILabel  playerName = go.transform.Find("name").GetComponent<UILabel>();
        UILabel playerSrc   = go.transform.Find("src").GetComponent<UILabel>();
        UISprite playerBei  = go.transform.Find("bei").GetComponent<UISprite>();
  
        playerBG.color      = col;
        playerName.color    = col;
        playerSrc.color     = col;
        playerBei.color     = col;
        playerSrc.text      = string.Format("{0}", teamTmp.playerData.score);
        playerName.text     = teamTmp.playerData.name;
        playerIcon.spriteName = teamTmp.playerData.icon;
    }


    private void OnSelectProcessAram(GameObject go)
    {
        Vector2 lefscr = UICamera.currentCamera.WorldToScreenPoint(percentleft.gameObject.transform.position);
        Vector2 rigscr = UICamera.currentCamera.WorldToScreenPoint(percentright.gameObject.transform.position);
        Vector2 curpos = UICamera.lastEventPosition;

        percent = (curpos.x - lefscr.x) / (rigscr.x - lefscr.x);
        if (percent < 0)
            percent = 0;
        if (percent > 1.0f)
            percent = 1.0f;

        percent = Mathf.Round(percent * 100) / 100;
        SetPercent();
    }


	
	public override void OnHide()
	{
        //GuideManager.ClearGuideData();
		UISystem.Get ().HideWindow ("PopTextWindow");
	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
		if (eventId == EventId.NoticeSelfTeam) {

			// 获取当前战斗中所有己方星球的ID
			if (BattleSystem.Instance.battleData.isReplay)
				return;	// 如果重播，则不提示这个窗口

			MapConfig map = MapConfigProvider.Instance.GetData (BattleSystem.Instance.battleData.matchId);
			Team selfTeam = BattleSystem.Instance.sceneManager.teamManager.GetTeam (BattleSystem.Instance.battleData.currentTeam);
			for (int i = 0; i < map.player_count; ++i) {
				Team team = BattleSystem.Instance.sceneManager.teamManager.GetTeam ((TEAM)(i + 1));

				//for (int j = 0; j < map.mpcList.Count; ++j) {
				//	MapPlayerConfig pi = map.mpcList[j];
				//	if (pi.camption == (int)team.team) {
				//		if (team == selfTeam) {

				//			Node n = BattleSystem.Instance.sceneManager.nodeManager.GetNode (pi.tag);
				//			EffectManager.Get ().ShowGuideEffect (n, true);
				//		} else if (selfTeam.IsFriend (team.groupID)) {

				//			Node n = BattleSystem.Instance.sceneManager.nodeManager.GetNode (pi.tag);
				//			EffectManager.Get ().ShowGuideEffect (n, false);
				//		}
				//	}
				//}
			}

			//TouchHandler.ShowingGuidEffect = true;
		} else if (eventId == EventId.OnBattleDisconnect) {
			// 战斗中掉线
			// 此时需要提示，并尝试重连接
			UISystem.Instance.ShowWindow ("ReconnectWindow");

		} else if (eventId == EventId.RequestUserResult) {
			// 玩家在战斗中

			NetMessage.ErrCode code = (NetMessage.ErrCode)args [0];
			if (code == NetMessage.ErrCode.EC_NeedResume) {
				NetSystem.Instance.helper.ReconnectResume (BattleSystem.Instance.GetCurrentFrame () / 5 + 1);
			} else {
				//  如果不是，则进入主页
				//BattleManager.Get ().Release ();
				BattleSystem.Instance.Reset ();
				UISystem.Get ().HideAllWindow ();
				UISystem.Get ().ShowWindow ("StartWindow");
            }
		} else if (eventId == EventId.OnTouchBegin) {
			//Node node = (Node)args [0];
			//ShowNewProgress1 (true, node);
		} else if (eventId == EventId.OnTouchPause) {
			//Node node = (Node)args [0];
			//PauseNewProgress1 (node);
		} else if (eventId == EventId.OnTouchEnd) {
			ShowNewProgress1 (false);
		} else if (eventId == EventId.OnSelfDied) {
			// 自己死亡，进入观看
			giveupLabel.text = LanguageDataProvider.GetValue(419);
			giveupForQuit = true;
		}

		if (eventId == EventId.OnPopulationUp) {
			
			//设置显示人口数量
			int current = (int)args [0];
			int max = (int)args [1];
			int pop = (int)args [2];

			populationValueLabel.text = string.Format ("{0} / {1}", current, max);
			popLableValue1.text = populationValueLabel.text;

			//设置变色lable
			popLable1.color = new Color (0x00, 0xFF, 0x00, 1f);
			popLableValue1.color = new Color (0x00, 0xFF, 0x00, 1f);


			//设置增加数量
			popLableAdd.text = string.Format("+{0}", pop);
			popLableAdd.color = new Color (0.2f, 1f, 0.2f, 1f);

			// 加字的偏移
			popLableAdd.transform.localPosition = popLableValue1.transform.localPosition + new Vector3 (popLableValue1.printedSize.x + 30, 0, 0);

		}

		if (eventId == EventId.OnPopulationDown) {

			//设置显示人口数量
			int current = (int)args [0];
			int max = (int)args [1];
			int pop = (int)args [2];

			populationValueLabel.text = string.Format ("{0} / {1}", current, max);
			popLableValue1.text = populationValueLabel.text;

			//设置变色lable
			popLable1.color = new Color (0xFF, 0x00, 0x00, 1f);
			popLableValue1.color = new Color (0xFF, 0x00, 0x00, 1f);

			//设置增加数量
			popLableAdd.text = string.Format("-{0}", pop);
			popLableAdd.color = new Color (1f, 0.2f, 0.2f, 1f);

			// 加字的偏移
			popLableAdd.transform.localPosition = popLableValue1.transform.localPosition + new Vector3 (popLableValue1.printedSize.x + 30, 0, 0);
		}

        if (eventId == EventId.PlayerGiveUp)
        {
            // 玩家放弃
            TEAM team = (TEAM)args[0];
            OperateTeamGiveUP((int)team);
        }

        if (eventId == EventId.PlayerDead)
        {
            // 玩家死亡
            TEAM team = (TEAM)args[0];
            OperateTeamDead((int)team);
        }

        if (eventId == EventId.OnPVPBattleAccelerate) {
            Tips.Make(LanguageDataProvider.GetValue(1116), 3.0f);
            clockAniStr = "Battle_battleframe_clock2";
            clockAni.Play("Battle_battleframe_clock2");
        }

        if (eventId == EventId.OnPVPBattleBoom) {
            Tips.Make(LanguageDataProvider.GetValue(1117), 3.0f);
            clockAniStr = "Battle_battleframe_clock3";
            clockAni.Play("Battle_battleframe_clock3");
        }
    }

    float fUpdateBattleTime = 0;
	void Update()
	{
		if (popLable1.alpha > 0) {

			popLableValue1.alpha = popLable1.alpha = (popLable1.alpha-Time.deltaTime * 0.5f);

			if (popLable1.alpha < 0f)
				popLableValue1.alpha = popLable1.alpha = 0f;
		}

		if (popLableAdd.alpha > 0) {
			popLableAdd.alpha -= Time.deltaTime * 0.5f;

			if (popLableAdd.alpha < 0f)
				popLableAdd.alpha = 0f;
		}

        fUpdateBattleTime += Time.deltaTime;
        if (fUpdateBattleTime > 0.5f)
        {
            fUpdateBattleTime = 0;
            UpdateBattleTime();
            UpdateBattleTimeColor();
        }
    }

    

    private int lastTimeSeconds = -1;
   
    void UpdateBattleTime()
    {
		int now = Mathf.RoundToInt(battleTimeMax - BattleSystem.Instance.sceneManager.GetBattleTime ());
        if (now == lastTimeSeconds)
        {
            if (lastTimeSeconds <= 0 )
                PlayTimeWillEnd(false);
            return;
        }
		
		lastTimeSeconds = now;

        if (lastTimeSeconds < 0)
        {
            lastTimeSeconds = 0;
        }
			
		if (lastTimeSeconds > 10) {
			//添加全部失去星球。30秒倒计时逻辑
			Team team = BattleSystem.Instance.sceneManager.teamManager.GetTeam (BattleSystem.Instance.battleData.currentTeam);
			float loseAllMatrixTime = team.GetLoseAllMatrixTime ();
			if (0.0f == loseAllMatrixTime || team.current == 0) 
			{
				battleTime.text = string.Format ("{0:D2}:{1:D2}", lastTimeSeconds / 60, lastTimeSeconds % 60);
				battleTime.transform.parent.gameObject.SetActive(true);
                clockAni.Play(clockAniStr);
                battleCoundown.transform.parent.gameObject.SetActive(false);
                bProceesEnd = false;
            }
            else 
			{
				int survivorTimeMax = int.Parse (GameVariableConfigProvider.Instance.GetData(6));
				int residueTime = survivorTimeMax - (int)loseAllMatrixTime;
				battleCoundown.text = residueTime.ToString();
				if ( !bProceesEnd )
					PlayTimeWillEnd ( true );
				bProceesEnd = true;
			}
		} else {
			//battleTime.text = lastTimeSeconds.ToString ();
            battleCoundown.text = lastTimeSeconds.ToString();
            if ( !bProceesEnd )
			    PlayTimeWillEnd ( true );
            bProceesEnd = true;
		}
    }
	private void TimerProc()
	{
		Team team =  BattleSystem.Instance.sceneManager.teamManager.GetTeam (BattleSystem.Instance.battleData.currentTeam);

		populationValueLabel.text = string.Format ("{0} / {1}", team.current, team.currentMax);
		popLableValue1.text = populationValueLabel.text;
		Invoke ("TimerProc", 0.5f);
	}

	Vector2 totalDrag;
	private void OnProgressDragStart (GameObject go)
	{
		totalDrag = Vector2.zero;
	}

	private void OnProgressDragEnd (GameObject go)
	{
		float x = totalDrag.x * sensitive / lineTotalLength;

		if (x <= -0.05f || x >= 0.05f) {
			int a = (int)(x / 0.1f);
			int b = (int)((x % 0.1f) / 0.05f);
			x = (a + b) * 0.1f;

			totalDrag.x -= x * lineTotalLength;
		}
		else
		{
			x = 0;
			return;
		}

		percent += x;

		if (percent < 0)
			percent = 0;

		if (percent > 1)
			percent = 1;

		percent = Mathf.Round(percent * 100) / 100;

		SetPercent ();
	}

	private void OnProgressDrag(GameObject go, Vector2 delta)
	{
        //totalDrag   += delta;
        OnSelectProcessAram(null);
    }

	public void OnCloseClick()
	{
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("CustomSelectWindowNew");
	}

	private void SetPercent()
	{
		if (LocalSettingStorage.Get ().fightOption == 1) {
			newProgressPic.fillAmount = percent;
			newProgressLabel.text = string.Format ("{0}%", Mathf.RoundToInt (percent * 100));
		} else {
			float length = lineTotalLength * percent;
			Vector3 pos = percentZeroPos;
			pos.x += length;
			int width = (int)((lineTotalLength * percent) + 2 - percentPicWidth / 2);
			percentleft.width = width;
			width = (int)((lineTotalLength * (1 - percent)) + 2 - percentPicWidth / 2);
			percentright.width = width;
			percentleft.gameObject.SetActive (true);
			percentright.gameObject.SetActive (true);
			if (percent == 0)
				percentleft.gameObject.SetActive (false);
			else if (percent == 1)
				percentright.gameObject.SetActive (false);
			//percentleft.fillAmount = percent - 0.06f;
			//percetright.fillAmount = 1f - percent - 0.06f;
			percetPic.transform.localPosition = pos;
			// 设置百分比数字
			percentLabel.text = string.Format ("{0}%", Mathf.RoundToInt (percent * 100));
		}

		// 设置游戏内飞船移动百分比
		BattleSystem.Instance.battleData.sliderRate = percent;
	}

	private void GetBattleMapIndex (string mapId, out int index, out int total)
	{
		List<string> mapList = new List<string> ();
		Dictionary<string, MapConfig> mapDict = MapConfigProvider.Instance.GetAllData ();
		foreach (var i in mapDict) {
			if (i.Value.vertical == true) {
				mapList.Add ((string)i.Key);
			}
		}
		index = 0;
		total = mapList.Count;
		for (int i = 0; i < total; ++i) {
			if (mapId.Equals (mapList [i])) {
				index = i;
				break;
			}
		}
		mapList.Clear ();
	}

	/// <summary>
	/// 设置背景图
	/// </summary>
	/// <param name="mapId">Map identifier.</param>
	public static void SetBattleBg (int mapIndex, int mapTotalCount)
	{
		//GameObject bgParent = GameObject.Find ("Battle/BG");
		//float x = ( -78.66f) / mapTotalCount;
		//x *= mapIndex;
		//bgParent.transform.localPosition = new Vector3 (x, 0, 0);
	}

	

	int curSelectSkill = 0;
	bool isPressSkill = false;
	float pressTime = 0f;

	public void OnPressSkill(int index)
	{
		Debug.Log ("OnPressSkill:" + index.ToString ());
		curSelectSkill = index;
		isPressSkill = true;
		pressTime = 0f;
	}

	/// <summary>
	/// 初始化分兵状态
	/// </summary>
	private void InitFightProgressOption()
	{
		newProgressGo.SetActive (false);


		int option = LocalSettingStorage.Get ().fightOption;
		if (option == 1) {
			// 长按
			ShowNewProgress1 (false);

		} else {
			// 普通进度条
			// 进度条滑块绑定事件
			UIEventListener listener = percetPic.gameObject.GetComponent<UIEventListener> ();
			listener.onDrag = OnProgressDrag;
			listener.onDragStart = OnProgressDragStart;

			// line total Length
			lineTotalLength = 1200;

			percentZeroPos = new Vector3 (percetPic.transform.localPosition.x - (lineTotalLength/2),
				percetPic.transform.localPosition.y, 0);
		}
	}

	/// <summary>
	/// 新的分兵方式，长按
	/// </summary>
	private void ShowNewProgress1(bool show/*, Node node = null*/)
	{
		//if (LocalSettingStorage.Get ().fightOption != 1)
		//	return;

		//if (newProgressGo == null)
		//	return;

		//if (show) {
		//	newProgressGo.gameObject.SetActive (true);
		//	Vector3 pos = Camera.main.WorldToScreenPoint (node.GetPosition ());
		//	pos.x -= Screen.width / 2;
		//	pos.y -= Screen.height / 2;
		//	newProgressGo.transform.localPosition = pos;
		//	newProgressPic.transform.localScale = Vector3.one * node.GetScale ();

		//	SetPercent ();

		//	if (!IsInvoking ("UpdateNewProgress1")) {
		//		InvokeRepeating ("UpdateNewProgress1", 0.1f, 0.05f);
		//	}

		//	newProgressTag = node.tag;

		//} else {
		//	newProgressGo.SetActive (false);
		//	CancelInvoke ("UpdateNewProgress1");
		//	newProgressTag = string.Empty;
		//	newProgressCount = 0;
		//	percent = 1.0f;
		//}
	}

	//private void PauseNewProgress1 (Node node)
	//{
	//	if (LocalSettingStorage.Get ().fightOption != 1)
	//		return;
	//	if (newProgressTag.Equals (node.tag) && IsInvoking ("UpdateNewProgress1")) {
	//		CancelInvoke ("UpdateNewProgress1");
	//	}
	//}

	private void UpdateNewProgress1()
	{
		if (LocalSettingStorage.Get ().fightOption != 1)
			return;
		
		if (newProgressCount >= 40) {
			newProgressCount = 0;
		}

		percent = 1 - newProgressCount * 0.025f;

		SetPercent ();

		newProgressCount++;
	}

	public void GiveUpOnClicked()
	{
		if (!giveupForQuit) {
			UISystem.Get ().ShowWindow ("CommonDialogWindow");
			EventSystem.Instance.FireEvent (EventId.OnCommonDialog, 
				3, LanguageDataProvider.GetValue (801), new EventDelegate (GiveUp), null, Mathf.CeilToInt(giveupCountTime - BattleSystem.Instance.sceneManager.GetBattleTime()));

		} else {
			// 退出
			BattleSystem.Instance.OnPlayerDirectQuit ();
		}
	}

	void GiveUp()
	{
		BattleSystem.Instance.PlayerGiveUp ();
	}


    private int nPlayEffectTimes = 0;
    private void UpdateBattleTimeColor( )
    {
        int curSegment = BattleSystem.Instance.sceneManager.curProcduceIndex;
        if( curSegment != curAddSpeedSegment && curSegment > curAddSpeedSegment )
        {
            curAddSpeedSegment = curSegment;
            if( curSegment == 1 )
                battleTime.color = new Color(1, 1, 0);
            if (curSegment == 2)
                battleTime.color = new Color(1, 0, 0);
           
            PlayTimeEffectOut();
        }

        if( nPlayEffectTimes == 1 )
        {
            PlayTimeEffectIn();
        }
    }


    private void PlayTimeEffectOut()
    {
        nPlayEffectTimes = 0;
        TweenScale ts = battleTime.gameObject.GetComponent<TweenScale>();
        if (ts == null)
        {
            ts = battleTime.gameObject.AddComponent<TweenScale>();
        }

        ts.ResetToBeginning();
        ts.from = Vector3.one;
        ts.to = Vector3.one * 1.5f;
        ts.duration = 0.5f;
        ts.SetOnFinished(() =>
        {
            nPlayEffectTimes++;
        });
        ts.Play(true);
    }

    private void PlayTimeEffectIn()
    {
        nPlayEffectTimes++;
        TweenScale ts = battleTime.gameObject.GetComponent<TweenScale>();
        if (ts == null)
        {
            ts = battleTime.gameObject.AddComponent<TweenScale>();
        }

        ts.ResetToBeginning();
        ts.onFinished.Clear();
        ts.from     = Vector3.one * 1.5f;
        ts.to       = Vector3.one;
        ts.duration = 0.5f;
        ts.Play(true);
    }

	/// <summary>
	/// 倒计时快结束时时间动画
	/// </summary>
	private void PlayTimeWillEnd ( bool bStart )
	{
        if (bStart) { 
            battleTime.transform.parent.gameObject.SetActive(false);
            battleCoundown.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            battleCoundown.transform.parent.gameObject.SetActive(false);
        }
	}

    /// <summary>
    /// 处理队伍投降, 现在只处理
    /// </summary>
    private void OperateTeamGiveUP(int team)
    {
        int idex        = -1;
        Color32 color   = new Color32(0xcc, 0xcc, 0xcc, 0xFF);
        bool ret        = mapTeam2Index.TryGetValue( team, out idex );
        if( ret && idex >= 0 && idex < playerFrames.Length )
        {
            GameObject go       = playerFrames[idex];
            UISprite playerFlag = go.transform.Find("flag").GetComponent<UISprite>();
            GameObject effect   = go.transform.Find("Effect_Surrender").gameObject;
            UILabel nameFlag    = effect.transform.Find("pos1/flagname").GetComponent<UILabel>();
            UILabel dwonFlag    = effect.transform.Find("pos2/flagdown").GetComponent<UILabel>();

            playerFlag.gameObject.SetActive(true);
            nameFlag.gameObject.SetActive(true);
            nameFlag.text       = LanguageDataProvider.GetValue(803);
            dwonFlag.text       = LanguageDataProvider.GetValue(803);
            effect.SetActive(true);
        }
    }

    /// <summary>
    /// 处理队伍死亡
    /// </summary>
    private void OperateTeamDead(int team)
    {
        int idex = -1;
        Color32 color = new Color32(0xcc, 0xcc, 0xcc, 0xFF);
        bool ret = mapTeam2Index.TryGetValue(team, out idex);
        if (ret && idex >= 0 && idex < playerFrames.Length)
        {
            GameObject go       = playerFrames[idex];
            UISprite playerBG   = go.GetComponent<UISprite>();
            UILabel  playerName = go.transform.Find("name").GetComponent<UILabel>();
            UISprite playerBei  = go.transform.Find("bei").GetComponent<UISprite>();
            
            UISprite playerFlag = go.transform.Find("flag").GetComponent<UISprite>();
            GameObject effect   = go.transform.Find("Effect_Surrender").gameObject;
            UILabel nameFlag    = effect.transform.Find("pos1/flagname").GetComponent<UILabel>();
            UILabel dwonFlag    = effect.transform.Find("pos2/flagdown").GetComponent<UILabel>();

            playerBG.color      = color;
            playerName.color    = color;
            playerBei.color     = color;
            nameFlag.gameObject.SetActive(true);
            nameFlag.text       = LanguageDataProvider.GetValue(802);
            playerFlag.enabled  = false;
            dwonFlag.text       = LanguageDataProvider.GetValue(802);
            effect.SetActive(true);
        }
    }
}

